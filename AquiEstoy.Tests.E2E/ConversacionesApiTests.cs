using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AquiEstoy.Domain.Entities;
using AquiEstoy.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace AquiEstoy.Tests.E2E;

/// <summary>
/// Regresion: Casos.Id y Conversaciones.Id son secuencias identity independientes
/// que se solapan. Cuando el endpoint de mensajes aceptaba cualquiera de los dos
/// ("c.Id == x || c.CasoId == x"), pedir una conversacion podia devolver los
/// mensajes privados de OTRO paciente.
///
/// Requiere AquiEstoy.API corriendo en https://localhost:7139.
/// </summary>
[TestFixture]
public class ConversacionesApiTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly string _sufijo = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
    private string EmailPrueba => $"conv.api.{_sufijo}@test.local";

    private HttpClient _http = null!;

    /// <summary>
    /// Id del Caso señuelo creado por <see cref="ForzarDesfaseEntreCasosYConversacionesAsync"/>,
    /// si hizo falta uno. Se limpia en el TearDown junto con el resto.
    /// </summary>
    private int? _casoSenueloId;

    [SetUp]
    public void CrearCliente()
    {
        _http = new HttpClient(new HttpClientHandler
        {
            // Certificado de desarrollo autofirmado en localhost.
            ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        })
        {
            BaseAddress = new Uri(TestConfig.ApiBaseUrl)
        };
    }

    [Test]
    public async Task PasarUnCasoId_NoDevuelveLaConversacionDeOtroPaciente()
    {
        // Cada registro de paciente crea un Caso y una Conversacion juntos (1 a 1), asi
        // que si nada mas desalinea las dos secuencias identity, CasoId y ConversacionId
        // del mismo registro coinciden numericamente segun el estado acumulado de la BD
        // (no por azar dentro de esta corrida, sino por el historial de la BD). Eso
        // volvia el test intermitente. Forzamos aqui, de forma deterministica, que las
        // dos secuencias NO esten alineadas antes de registrar al paciente de prueba.
        await using (var db = CrearDbContext())
        {
            await ForzarDesfaseEntreCasosYConversacionesAsync(db);
        }

        var registrado = await RegistrarPacienteAsync();

        Assert.That(registrado.CasoId, Is.Not.EqualTo(registrado.ConversacionId),
            "El escenario pierde sentido si ambos Id coinciden: es justo el solapamiento que se quiere cubrir. " +
            "Si esto falla, el desfase forzado arriba tiene un error.");

        // El Id de la conversacion sí resuelve.
        var porConversacion = await _http.GetAsync($"api/conversaciones/{registrado.ConversacionId}/mensajes");
        Assert.That(porConversacion.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        // El Id del caso NO debe resolver como si fuera el de la conversacion.
        var porCaso = await _http.GetAsync($"api/conversaciones/{registrado.CasoId}/mensajes");

        await using var db2 = CrearDbContext();
        var existeConversacionConEseId = await db2.Conversaciones
            .AnyAsync(c => c.Id == registrado.CasoId);

        if (!existeConversacionConEseId)
        {
            Assert.That(porCaso.StatusCode, Is.EqualTo(HttpStatusCode.NotFound),
                "Un CasoId que no existe como ConversacionId debe dar 404, no resolver por CasoId.");
            return;
        }

        // Si por casualidad existe una conversacion con ese numero, es de otro caso:
        // lo que nunca puede pasar es que devuelva mensajes ajenos al Id pedido.
        Assert.That(porCaso.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var mensajes = await porCaso.Content.ReadFromJsonAsync<List<MensajeApi>>(JsonOptions) ?? new();

        Assert.That(mensajes.All(m => m.ConversacionId == registrado.CasoId), Is.True,
            "El endpoint devolvió mensajes de una conversación distinta a la solicitada.");
    }

    /// <summary>
    /// Si el próximo Id de Casos y el próximo Id de Conversaciones coincidirían (las
    /// secuencias están sincronizadas), inserta un Caso señuelo sin Conversacion para
    /// consumir un Id de la secuencia de Casos y desalinearlas de forma permanente.
    /// No requiere borrar nada para "reservar" el hueco: el identity de SQL Server
    /// nunca reutiliza valores, así que el desfase se mantiene aunque el señuelo se
    /// borre después en el TearDown.
    /// </summary>
    private async Task ForzarDesfaseEntreCasosYConversacionesAsync(AquiEstoyDbContext db)
    {
        var ultimoCasoId = await ObtenerUltimoCasoIdAsync(db);
        var ultimaConversacionId = await ObtenerUltimaConversacionIdAsync(db);

        if (ultimoCasoId != ultimaConversacionId) return;

        var pacienteExistente = await db.Usuarios.OrderBy(u => u.Id).FirstAsync();
        var nivelSeveridad = await db.NivelesSeveridad.OrderBy(n => n.Id).FirstAsync();
        var estadoAbierto = await db.EstadosCaso.SingleAsync(e => e.Nombre.ToUpper() == "ABIERTO");

        var casoSenuelo = new Caso
        {
            PacienteId = pacienteExistente.Id,
            NivelSeveridadId = nivelSeveridad.Id,
            EstadoCasoId = estadoAbierto.Id,
            Descripcion = "Señuelo de prueba (ConversacionesApiTests): fuerza desfase Casos/Conversaciones."
        };

        db.Casos.Add(casoSenuelo);
        await db.SaveChangesAsync();

        _casoSenueloId = casoSenuelo.Id;
    }

    private static Task<long> ObtenerUltimoCasoIdAsync(AquiEstoyDbContext db) =>
        ObtenerIdentActualAsync(db, "SELECT IDENT_CURRENT('Casos') AS Value");

    private static Task<long> ObtenerUltimaConversacionIdAsync(AquiEstoyDbContext db) =>
        ObtenerIdentActualAsync(db, "SELECT IDENT_CURRENT('Conversaciones') AS Value");

    private static async Task<long> ObtenerIdentActualAsync(AquiEstoyDbContext db, string sql)
    {
        var valor = await db.Database.SqlQueryRaw<decimal?>(sql).FirstAsync();
        return valor.HasValue ? (long)valor.Value : 0L;
    }

    private async Task<PacienteRegistradoApi> RegistrarPacienteAsync()
    {
        var respuesta = await _http.PostAsJsonAsync("api/usuarios/registrar-paciente", new
        {
            identificacion = $"8-{_sufijo[..4]}-{_sufijo[4..8]}",
            nombre = "ConvApi",
            apellidos = $"Prueba{_sufijo}",
            email = EmailPrueba,
            password = "Prueba123!",
            telefono = "8888-0001"
        });

        respuesta.EnsureSuccessStatusCode();

        return await respuesta.Content.ReadFromJsonAsync<PacienteRegistradoApi>(JsonOptions)
               ?? throw new InvalidOperationException("La API no devolvió el paciente registrado.");
    }

    [TearDown]
    public async Task Limpiar()
    {
        _http?.Dispose();

        await using var db = CrearDbContext();

        if (_casoSenueloId.HasValue)
        {
            db.Casos.RemoveRange(db.Casos.Where(c => c.Id == _casoSenueloId.Value));
        }

        var usuario = await db.Usuarios.SingleOrDefaultAsync(u => u.Email == EmailPrueba);
        if (usuario == null)
        {
            await db.SaveChangesAsync();
            return;
        }

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

    private sealed record PacienteRegistradoApi(int UsuarioId, int CasoId, int ConversacionId);

    private sealed record MensajeApi(int Id, int ConversacionId, int RemitenteId);
}
