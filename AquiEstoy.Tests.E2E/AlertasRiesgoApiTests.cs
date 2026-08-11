using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AquiEstoy.Application.DTOs.Usuarios;
using AquiEstoy.Application.Services;
using AquiEstoy.Infrastructure.Data;
using AquiEstoy.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace AquiEstoy.Tests.E2E;

/// <summary>
/// Confirma que POST /api/alertas-riesgo/evaluar con un CasoId real crea una fila
/// en Alertas (CasoId, TipoAlertaId de "Riesgo Crítico", Descripcion con el mensaje).
///
/// El CasoId se obtiene con RegistroPacienteService directo contra la BD (mismo
/// patrón que TransaccionRegistroTests), porque AquiEstoy.API aún no expone un
/// endpoint HTTP de registro de pacientes en esta rama; el endpoint bajo prueba
/// (api/alertas-riesgo/evaluar) sí se llama contra AquiEstoy.API real.
///
/// Requiere AquiEstoy.API corriendo en https://localhost:7139.
/// </summary>
[TestFixture]
public class AlertasRiesgoApiTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly string _sufijo = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
    private string EmailPrueba => $"alertas.api.{_sufijo}@test.local";

    private HttpClient _http = null!;

    [SetUp]
    public void CrearCliente()
    {
        _http = new HttpClient(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        })
        {
            BaseAddress = new Uri(TestConfig.ApiBaseUrl)
        };
    }

    [Test]
    public async Task Evaluar_ConPuntajeAlto_CreaAlertaDeRiesgoCriticoParaElCaso()
    {
        var casoId = await CrearCasoDePruebaAsync();

        var respuesta = await _http.PostAsJsonAsync("api/alertas-riesgo/evaluar", new
        {
            casoId,
            mensaje = "Mensaje de prueba con alto contenido de riesgo.",
            puntajeRiesgo = 0.9
        });

        Assert.That(respuesta.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var resultado = await respuesta.Content.ReadFromJsonAsync<EvaluarRiesgoResponseApi>(JsonOptions);

        Assert.That(resultado, Is.Not.Null);
        Assert.That(resultado!.NivelRiesgo, Is.EqualTo("Critico"));
        Assert.That(resultado.MostrarAlerta, Is.True);
        Assert.That(resultado.AlertaId, Is.Not.Null);

        await using var db = CrearDbContext();

        var alerta = await db.Alertas
            .Include(a => a.TipoAlerta)
            .SingleOrDefaultAsync(a => a.Id == resultado.AlertaId!.Value);

        Assert.That(alerta, Is.Not.Null, "La API respondió AlertaId pero no existe la fila en Alertas.");
        Assert.That(alerta!.CasoId, Is.EqualTo(casoId));
        Assert.That(alerta.TipoAlerta.Nombre, Is.EqualTo("Riesgo Crítico"));
        Assert.That(alerta.Descripcion, Is.Not.Null.And.Not.Empty);
        Assert.That(alerta.Atendida, Is.False);
    }

    private async Task<int> CrearCasoDePruebaAsync()
    {
        await using var context = CrearDbContext();

        var casoService = new CasoService(context);
        var registroService = new RegistroPacienteService(context, casoService, new BCryptPasswordHasher());

        var registrado = await registroService.RegistrarAsync(new RegistrarPacienteDto
        {
            Identificacion = $"8-{_sufijo[..4]}-{_sufijo[4..8]}",
            Nombre = "AlertasApi",
            Apellidos = $"Prueba{_sufijo}",
            Email = EmailPrueba,
            Password = "Prueba123!",
            Telefono = "8888-0002"
        });

        return registrado.CasoId;
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

        db.Alertas.RemoveRange(db.Alertas.Where(a => casoIds.Contains(a.CasoId)));
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

    private sealed record EvaluarRiesgoResponseApi(
        int? AlertaId,
        string NivelRiesgo,
        bool MostrarAlerta,
        string Mensaje,
        List<LineaAyudaApi> LineasAyuda);

    private sealed record LineaAyudaApi(int Id, string Nombre, string Telefono, string? Descripcion);
}
