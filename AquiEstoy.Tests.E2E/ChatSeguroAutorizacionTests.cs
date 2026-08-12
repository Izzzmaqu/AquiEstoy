using System.Net;
using AquiEstoy.Application.DTOs.Usuarios;
using AquiEstoy.Application.Services;
using AquiEstoy.Infrastructure.Data;
using AquiEstoy.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace AquiEstoy.Tests.E2E;

/// <summary>
/// Regresion: HomeController.ChatSeguro solo tenia [Authorize] (cualquier usuario
/// autenticado), sin restringir por rol. Como AquiEstoy.Web usa una unica cookie de
/// sesion por navegador (Web/Program.cs), si un profesional dejaba esta pantalla
/// abierta en una pestana y en otra pestana del MISMO navegador alguien iniciaba
/// sesion como paciente, recargar la primera pestana heredaba en silencio la
/// identidad del paciente: el usuarioId embebido en la vista pasaba a ser el del
/// paciente sin que nadie volviera a iniciar sesion ahi, y el chat mostraba "Tu" en
/// el mensaje ajeno porque isMe comparaba contra un usuarioId que ya no era el del
/// profesional.
///
/// La correccion no es "permitir dos identidades simultaneas en el mismo navegador"
/// (imposible con una sola cookie de sesion, y no es el problema real): es que un
/// paciente autenticado NUNCA debe poder resolver esta pantalla, sin importar de que
/// pestana venga la cookie. Este test verifica esa restriccion de rol directamente,
/// sin pasar por SignalR ni por dos pestanas: si algun dia alguien vuelve a dejar
/// ChatSeguro con [Authorize] a secas, este test debe fallar.
///
/// Requiere AquiEstoy.Web corriendo en https://localhost:7165 y SQL Server accesible
/// para el registro directo del paciente de prueba.
/// </summary>
[TestFixture]
public class ChatSeguroAutorizacionTests
{
    private readonly string _sufijo = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
    private string EmailPrueba => $"chatauth.{_sufijo}@test.local";
    private const string PasswordPrueba = "Prueba123!";

    private HttpClient _http = null!;

    [SetUp]
    public void CrearCliente()
    {
        _http = new HttpClient(new HttpClientHandler
        {
            // Certificado de desarrollo autofirmado en localhost.
            ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
            UseCookies = true,
            CookieContainer = new CookieContainer(),
            // Se inspecciona el Location del redirect en vez de seguirlo.
            AllowAutoRedirect = false
        })
        {
            BaseAddress = new Uri(TestConfig.WebBaseUrl)
        };
    }

    [Test]
    public async Task Paciente_NoPuedeResolverChatSeguroDelProfesional()
    {
        var conversacionId = await RegistrarPacienteYObtenerConversacionAsync();

        // Login contra la capa Web (cookie de sesion real vía AccountController,
        // no el login JSON de la API). El formulario no exige antiforgery token.
        var loginResponse = await _http.PostAsync("/Account/Login", new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                ["Email"] = EmailPrueba,
                ["Password"] = PasswordPrueba
            }));

        Assert.That(loginResponse.StatusCode, Is.EqualTo(HttpStatusCode.Redirect),
            "El login deberia redirigir tras autenticar correctamente.");
        Assert.That(loginResponse.Headers.Location?.ToString(), Does.Contain("PacienteDashboard"),
            "Un paciente deberia terminar en su propio dashboard: confirma que el login si funciono " +
            "y que la prueba esta atacando con una sesion de paciente valida, no con un login fallido.");

        // La pantalla de chat del PROFESIONAL, pedida con la cookie de sesion de un PACIENTE.
        var chatResponse = await _http.GetAsync($"/Home/ChatSeguro?conversacionId={conversacionId}");

        Assert.That(chatResponse.StatusCode, Is.EqualTo(HttpStatusCode.Redirect),
            "Un paciente autenticado no deberia poder resolver ChatSeguro: con solo [Authorize] " +
            "esto devolvia 200 y servia el chat con la identidad del paciente cargada en la vista " +
            "del profesional, que es la causa raiz del bug de 'Tu' mal atribuido.");
        Assert.That(chatResponse.Headers.Location?.ToString(), Does.Contain("AccessDenied"),
            "Debe rechazarse por rol (AccessDenied), no servir el chat con la identidad equivocada.");
    }

    private async Task<int> RegistrarPacienteYObtenerConversacionAsync()
    {
        await using var context = CrearDbContext();

        var casoService = new CasoService(context);
        var registroService = new RegistroPacienteService(context, casoService, new BCryptPasswordHasher());

        var registrado = await registroService.RegistrarAsync(new RegistrarPacienteDto
        {
            Identificacion = $"8-{_sufijo[..4]}-{_sufijo[4..8]}",
            Nombre = "ChatAuth",
            Apellidos = $"Prueba{_sufijo}",
            Email = EmailPrueba,
            Password = PasswordPrueba,
            Telefono = "8888-0003"
        });

        return registrado.ConversacionId;
    }

    [TearDown]
    public async Task Limpiar()
    {
        _http?.Dispose();

        await using var db = CrearDbContext();

        var usuario = await db.Usuarios.SingleOrDefaultAsync(u => u.Email == EmailPrueba);
        if (usuario == null) return;

        var casoIds = await db.Casos.Where(c => c.PacienteId == usuario.Id)
            .Select(c => c.Id).ToListAsync();

        var conversacionIds = await db.Conversaciones
            .Where(c => c.PacienteId == usuario.Id || casoIds.Contains(c.CasoId))
            .Select(c => c.Id).ToListAsync();

        db.Mensajes.RemoveRange(db.Mensajes.Where(m => conversacionIds.Contains(m.ConversacionId)));
        db.Conversaciones.RemoveRange(db.Conversaciones.Where(c => conversacionIds.Contains(c.Id)));
        db.HistorialesSeveridad.RemoveRange(db.HistorialesSeveridad.Where(h => casoIds.Contains(h.CasoId)));
        db.CasoFactoresRiesgo.RemoveRange(db.CasoFactoresRiesgo.Where(f => casoIds.Contains(f.CasoId)));
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
