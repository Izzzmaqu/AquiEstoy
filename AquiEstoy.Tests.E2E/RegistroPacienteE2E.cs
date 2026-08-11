using AquiEstoy.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace AquiEstoy.Tests.E2E;

/// <summary>
/// Flujo critico de punta a punta: login staff -> registrar paciente -> ver el caso
/// en la lista -> abrir el chat y comprobar que el hub conecta de verdad.
///
/// Requiere AquiEstoy.API (https://localhost:7139) y AquiEstoy.Web (https://localhost:7165)
/// corriendo en Development, que es donde se ejecuta el seeder de catalogos.
/// </summary>
[TestFixture]
public class RegistroPacienteE2E : PageTest
{
    // Sufijo unico por corrida: Usuarios.Email tiene indice unico, un valor fijo
    // haria fallar la segunda ejecucion.
    private readonly string _sufijo = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");

    private string EmailPrueba => $"e2e.paciente.{_sufijo}@test.local";
    private string NombrePrueba => "PacienteE2E";
    private string ApellidosPrueba => $"Prueba{_sufijo}";

    private readonly List<string> _erroresDeConsola = new();

    public override BrowserNewContextOptions ContextOptions() => new()
    {
        BaseURL = TestConfig.WebBaseUrl,
        // Certificado de desarrollo autofirmado en localhost.
        IgnoreHTTPSErrors = true
    };

    [SetUp]
    public void CapturarConsola()
    {
        Page.Console += (_, msg) =>
        {
            if (msg.Type == "error") _erroresDeConsola.Add(msg.Text);
        };
    }

    [Test]
    public async Task RegistrarPaciente_CreaUsuarioCasoYConversacion_YElChatConecta()
    {
        // ---------- 1. Login como staff ----------
        await Page.GotoAsync("/Account/Login");

        await Page.FillAsync("input[name='Email']", TestConfig.StaffEmail);
        await Page.FillAsync("input[name='Password']", TestConfig.StaffPassword);
        await Page.ClickAsync("button[type='submit']");

        await Page.WaitForURLAsync(url => !url.Contains("/Account/Login"),
            new() { Timeout = 15000 });

        Assert.That(Page.Url, Does.Not.Contain("/Account/Login"),
            "El login con el usuario staff sembrado deberia haber redirigido fuera de la pantalla de login.");

        // ---------- 2. Ir a Usuarios y abrir el formulario ----------
        // La ruta por defecto es {controller=Account}/{action=Login}/{id?}: el action
        // por defecto es Login, no Index, asi que "/Usuarios" a secas da 404.
        await Page.GotoAsync("/Usuarios/Index");

        await Page.ClickAsync("button[data-bs-target='#modalNuevoPaciente']");
        await Page.WaitForSelectorAsync("#modalNuevoPaciente.show");

        // ---------- 3. Llenar y enviar ----------
        var modal = Page.Locator("#modalNuevoPaciente");

        await modal.Locator("input[name='Identificacion']").FillAsync($"9-{_sufijo[..4]}-{_sufijo[4..8]}");
        await modal.Locator("input[name='Nombre']").FillAsync(NombrePrueba);
        await modal.Locator("input[name='Apellidos']").FillAsync(ApellidosPrueba);
        await modal.Locator("input[name='Telefono']").FillAsync("8888-0000");
        await modal.Locator("input[name='Email']").FillAsync(EmailPrueba);
        await modal.Locator("input[name='Password']").FillAsync("Prueba123!");

        await modal.Locator("button[type='submit']").ClickAsync();

        // ---------- 4. Confirmacion en la UI ----------
        await Page.WaitForURLAsync("**/Usuarios/Index", new() { Timeout = 15000 });

        var alertaExito = Page.Locator(".alert-success");
        await Expect(alertaExito).ToBeVisibleAsync(new() { Timeout = 10000 });
        await Expect(alertaExito).ToContainTextAsync(NombrePrueba);

        // El paciente aparece en la tabla
        await Expect(Page.Locator($"tr[data-paciente-email='{EmailPrueba}']")).ToBeVisibleAsync();

        // ---------- 5. El caso aparece en la lista de Casos ----------
        await Page.GotoAsync("/Casos/Index");

        var filaCaso = Page.Locator("#tablaCasos tbody tr")
            .Filter(new() { HasText = ApellidosPrueba });

        await Expect(filaCaso).ToBeVisibleAsync(new() { Timeout = 10000 });

        // ---------- 6. El chat de ese caso conecta al hub ----------
        await filaCaso.GetByRole(AriaRole.Link, new() { Name = "Chat" }).ClickAsync();

        await Page.WaitForURLAsync("**/ChatSeguro**", new() { Timeout = 15000 });

        // Se comprueba el estado real de la conexion SignalR, no que el input exista:
        // el input es HTML estatico y estaria habilitado aunque el hub estuviera caido.
        await Page.WaitForFunctionAsync(
            "() => window.chatConnection && window.chatConnection.state === 'Connected'",
            null,
            new() { Timeout = 20000 });

        var estado = await Page.EvaluateAsync<string>("() => window.chatConnection.state");
        Assert.That(estado, Is.EqualTo("Connected"),
            "El hub de SignalR en la API deberia estar conectado.");

        var erroresDeConexion = _erroresDeConsola
            .Where(e => e.Contains("SignalR", StringComparison.OrdinalIgnoreCase)
                        || e.Contains("WebSocket", StringComparison.OrdinalIgnoreCase))
            .ToList();

        Assert.That(erroresDeConexion, Is.Empty,
            "No deberia haber errores de conexion en consola: " + string.Join(" | ", erroresDeConexion));

        // ---------- 7. Verificacion en BD: las tres entidades existen y estan enlazadas ----------
        await using var db = CrearDbContext();

        var usuario = await db.Usuarios.AsNoTracking()
            .SingleOrDefaultAsync(u => u.Email == EmailPrueba);

        Assert.That(usuario, Is.Not.Null, "Deberia existir el Usuario creado por el flujo.");

        var caso = await db.Casos.AsNoTracking()
            .SingleOrDefaultAsync(c => c.PacienteId == usuario!.Id);

        Assert.That(caso, Is.Not.Null, "Deberia existir el Caso asociado al paciente.");

        var conversacion = await db.Conversaciones.AsNoTracking()
            .SingleOrDefaultAsync(c => c.CasoId == caso!.Id);

        Assert.That(conversacion, Is.Not.Null, "Deberia existir la Conversacion asociada al caso.");
        Assert.That(conversacion!.PacienteId, Is.EqualTo(usuario!.Id));

        // Los IDs de catalogo deben apuntar a filas reales, no a valores inventados.
        Assert.That(await db.NivelesSeveridad.AnyAsync(n => n.Id == caso!.NivelSeveridadId), Is.True,
            "El NivelSeveridadId del caso debe existir en el catalogo.");
        Assert.That(await db.Profesionales.AnyAsync(p => p.Id == conversacion.ProfesionalId), Is.True,
            "El ProfesionalId de la conversacion debe existir en la tabla Profesionales.");
    }

    /// <summary>Borra los datos de prueba en orden inverso de FK.</summary>
    [TearDown]
    public async Task LimpiarDatosDePrueba()
    {
        await using var db = CrearDbContext();

        var usuario = await db.Usuarios.SingleOrDefaultAsync(u => u.Email == EmailPrueba);
        if (usuario == null) return;

        var casoIds = await db.Casos.Where(c => c.PacienteId == usuario.Id)
            .Select(c => c.Id).ToListAsync();

        var conversacionIds = await db.Conversaciones
            .Where(c => c.PacienteId == usuario.Id || casoIds.Contains(c.CasoId))
            .Select(c => c.Id).ToListAsync();

        db.Mensajes.RemoveRange(
            db.Mensajes.Where(m => conversacionIds.Contains(m.ConversacionId)));

        db.Conversaciones.RemoveRange(
            db.Conversaciones.Where(c => conversacionIds.Contains(c.Id)));

        db.HistorialesSeveridad.RemoveRange(
            db.HistorialesSeveridad.Where(h => casoIds.Contains(h.CasoId)));

        db.CasoFactoresRiesgo.RemoveRange(
            db.CasoFactoresRiesgo.Where(f => casoIds.Contains(f.CasoId)));

        db.Casos.RemoveRange(db.Casos.Where(c => casoIds.Contains(c.Id)));

        db.Usuarios.Remove(usuario);

        await db.SaveChangesAsync();
    }

    private static AquiEstoyDbContext CrearDbContext()
    {
        var options = new DbContextOptionsBuilder<AquiEstoyDbContext>()
            .UseSqlServer(TestConfig.ConnectionString)
            .Options;

        return new AquiEstoyDbContext(options);
    }
}
