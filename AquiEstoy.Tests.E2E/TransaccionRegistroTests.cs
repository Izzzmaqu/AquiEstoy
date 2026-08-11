using AquiEstoy.Application.DTOs.Casos;
using AquiEstoy.Application.DTOs.FactoresRiesgo;
using AquiEstoy.Application.DTOs.Usuarios;
using AquiEstoy.Application.Interfaces;
using AquiEstoy.Application.Services;
using AquiEstoy.Infrastructure;
using AquiEstoy.Infrastructure.Data;
using AquiEstoy.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace AquiEstoy.Tests.E2E;

/// <summary>
/// Prueba de atomicidad (requisito ACID del curso): si algo falla despues de crear
/// el Usuario, la transaccion debe revertirlo y no dejar registros huerfanos.
///
/// Se ataca la BD directamente, sin navegador: solo requiere SQL Server accesible.
/// </summary>
[TestFixture]
public class TransaccionRegistroTests
{
    private readonly string _sufijo = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
    private string EmailPrueba => $"acid.rollback.{_sufijo}@test.local";

    [Test]
    public async Task SiFallaLaCreacionDelCaso_NoQuedaUsuarioHuerfano()
    {
        await using var context = CrearDbContext();

        var usuariosAntes = await context.Usuarios.CountAsync();
        var casosAntes = await context.Casos.CountAsync();
        var conversacionesAntes = await context.Conversaciones.CountAsync();

        // CasoService falso que revienta justo despues de que el Usuario ya se inserto.
        var servicio = new RegistroPacienteService(
            context,
            new CasoServiceQueFalla(),
            new BCryptPasswordHasher());

        var dto = new RegistrarPacienteDto
        {
            Identificacion = "9-9999-9999",
            Nombre = "Rollback",
            Apellidos = "Prueba ACID",
            Email = EmailPrueba,
            Password = "Prueba123!",
            Telefono = "8888-9999"
        };

        var excepcion = Assert.ThrowsAsync<InvalidOperationException>(
            async () => await servicio.RegistrarAsync(dto));

        // La excepcion tiene que ser la simulada por CasoServiceQueFalla, no cualquier otra.
        // Si llega la de SqlServerRetryingExecutionStrategy ("does not support user-initiated
        // transactions"), la transaccion ni siquiera se abrio: no habria nada que revertir y
        // las comprobaciones de abajo pasarian por el motivo equivocado, dejando pasar
        // justo la regresion que esta prueba existe para detectar.
        Assert.That(excepcion!.Message, Does.Contain("Fallo simulado"),
            "Se esperaba el fallo simulado dentro de la transaccion, pero la excepcion fue otra: "
            + excepcion.Message);

        // Contexto nuevo: se comprueba lo que realmente quedo persistido, no la cache.
        await using var verificacion = CrearDbContext();

        var usuario = await verificacion.Usuarios
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Email == EmailPrueba);

        Assert.That(usuario, Is.Null,
            "El Usuario no deberia haberse persistido: la transaccion tenia que revertirlo.");

        Assert.Multiple(async () =>
        {
            Assert.That(await verificacion.Usuarios.CountAsync(), Is.EqualTo(usuariosAntes),
                "El total de usuarios deberia ser el mismo que antes del intento fallido.");
            Assert.That(await verificacion.Casos.CountAsync(), Is.EqualTo(casosAntes),
                "No deberia haberse creado ningun Caso.");
            Assert.That(await verificacion.Conversaciones.CountAsync(), Is.EqualTo(conversacionesAntes),
                "No deberia haberse creado ninguna Conversacion.");
        });
    }

    /// <summary>Limpieza defensiva por si la transaccion no hubiera revertido.</summary>
    [TearDown]
    public async Task Limpiar()
    {
        await using var context = CrearDbContext();

        var usuario = await context.Usuarios.SingleOrDefaultAsync(u => u.Email == EmailPrueba);
        if (usuario == null) return;

        var casoIds = await context.Casos.Where(c => c.PacienteId == usuario.Id)
            .Select(c => c.Id).ToListAsync();

        context.Conversaciones.RemoveRange(
            context.Conversaciones.Where(c => c.PacienteId == usuario.Id));
        context.HistorialesSeveridad.RemoveRange(
            context.HistorialesSeveridad.Where(h => casoIds.Contains(h.CasoId)));
        context.Casos.RemoveRange(context.Casos.Where(c => casoIds.Contains(c.Id)));
        context.Usuarios.Remove(usuario);

        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Usa la misma configuracion que la aplicacion real (UseAquiEstoySqlServer, con
    /// EnableRetryOnFailure incluido). Antes esta prueba llamaba a UseSqlServer a secas:
    /// sin la estrategia de reintentos, una transaccion manual mal envuelta funcionaba
    /// aqui y reventaba en produccion, que es justo la regresion que debe detectar.
    /// </summary>
    private static AquiEstoyDbContext CrearDbContext()
    {
        var options = new DbContextOptionsBuilder<AquiEstoyDbContext>()
            .UseAquiEstoySqlServer(TestConfig.ConnectionString)
            .Options;

        return new AquiEstoyDbContext(options);
    }

    /// <summary>Simula un fallo en mitad de la transaccion, con el Usuario ya insertado.</summary>
    private sealed class CasoServiceQueFalla : ICasoService
    {
        public Task<CasoListDto> CrearCasoAsync(CrearCasoDto dto) =>
            throw new InvalidOperationException("Fallo simulado para probar el rollback.");

        public Task<IEnumerable<CasoListDto>> ListarCasosAsync(CasoFiltroDto filtro) =>
            throw new NotSupportedException();

        public Task<CasoDetalleDto?> ObtenerDetalleAsync(int id) =>
            throw new NotSupportedException();

        public Task<CasoListDto?> EditarCasoAsync(int id, EditarCasoDto dto) =>
            throw new NotSupportedException();

        public Task<CasoListDto?> CerrarCasoAsync(int id, CerrarCasoDto dto) =>
            throw new NotSupportedException();

        public Task<CasoFactorRiesgoDto?> RegistrarFactorRiesgoAsync(int casoId, RegistrarFactorRiesgoDto dto) =>
            throw new NotSupportedException();

        public Task<IEnumerable<FactorRiesgoDto>> ListarFactoresRiesgoAsync() =>
            throw new NotSupportedException();
    }
}
