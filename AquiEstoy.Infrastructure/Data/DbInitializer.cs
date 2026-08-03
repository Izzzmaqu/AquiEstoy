using AquiEstoy.Application.Interfaces;
using AquiEstoy.Application.Services;
using AquiEstoy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AquiEstoy.Infrastructure.Data;

/// <summary>
/// Siembra los catálogos mínimos que el flujo de registro necesita.
/// Es idempotente: cada bloque comprueba antes de insertar, así que se puede
/// ejecutar en cada arranque sin duplicar filas.
///
/// Ningún Id se asume ni se hardcodea: todos son columnas identity y se resuelven
/// leyendo la fila recién insertada (o la ya existente).
/// </summary>
public static class DbInitializer
{
    public const string RolAdmin = "Admin";
    public const string RolProfesional = "Profesional";
    public const string RolPaciente = "Paciente";

    /// <summary>Credenciales del usuario staff de demo, usadas también por el test E2E.</summary>
    public const string EmailProfesionalDemo = "profesional@aquiestoy.com";
    public const string PasswordProfesionalDemo = "Demo1234!";

    public static async Task SeedAsync(AquiEstoyDbContext context, IPasswordHasher passwordHasher)
    {
        await SeedRolesAsync(context);
        await SeedNivelesSeveridadAsync(context);
        await SeedEspecialidadesAsync(context);
        await SeedProfesionalDemoAsync(context, passwordHasher);
        await SeedLineasAyudaAsync(context);
        await SeedTiposAlertaAsync(context);
    }

    private static async Task SeedRolesAsync(AquiEstoyDbContext context)
    {
        var deseados = new[]
        {
            new Rol { Nombre = RolAdmin, Descripcion = "Acceso administrativo completo" },
            new Rol { Nombre = RolProfesional, Descripcion = "Profesional de salud mental que atiende casos" },
            new Rol { Nombre = RolPaciente, Descripcion = "Paciente registrado en la plataforma" }
        };

        var existentes = await context.Roles.Select(r => r.Nombre).ToListAsync();

        var faltantes = deseados.Where(r => !existentes.Contains(r.Nombre)).ToList();
        if (faltantes.Count == 0) return;

        context.Roles.AddRange(faltantes);
        await context.SaveChangesAsync();
    }

    private static async Task SeedNivelesSeveridadAsync(AquiEstoyDbContext context)
    {
        var deseados = new[]
        {
            new NivelSeveridad { Nombre = "Bajo", Color = "#28a745", Descripcion = "Sin riesgo inmediato. Seguimiento de rutina." },
            new NivelSeveridad { Nombre = "Moderado", Color = "#ffc107", Descripcion = "Requiere seguimiento cercano." },
            new NivelSeveridad { Nombre = "Alto", Color = "#fd7e14", Descripcion = "Riesgo elevado. Intervención prioritaria." },
            new NivelSeveridad { Nombre = "Crítico", Color = "#dc3545", Descripcion = "Riesgo inminente. Atención inmediata." }
        };

        var existentes = await context.NivelesSeveridad.Select(n => n.Nombre).ToListAsync();

        var faltantes = deseados.Where(n => !existentes.Contains(n.Nombre)).ToList();
        if (faltantes.Count == 0) return;

        context.NivelesSeveridad.AddRange(faltantes);
        await context.SaveChangesAsync();
    }

    private static async Task SeedEspecialidadesAsync(AquiEstoyDbContext context)
    {
        if (await context.Especialidades.AnyAsync()) return;

        context.Especialidades.AddRange(
            new Especialidad { Nombre = "Psicología Clínica" },
            new Especialidad { Nombre = "Psiquiatría" },
            new Especialidad { Nombre = "Trabajo Social" });

        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Crea el usuario staff de demo y su registro de Profesional. Sin esto no hay
    /// ningún ProfesionalId real al que enlazar la Conversacion del registro.
    /// </summary>
    private static async Task SeedProfesionalDemoAsync(AquiEstoyDbContext context, IPasswordHasher passwordHasher)
    {
        var usuario = await context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == EmailProfesionalDemo);

        if (usuario == null)
        {
            var rolProfesional = await context.Roles.FirstAsync(r => r.Nombre == RolProfesional);

            usuario = new Usuario
            {
                Identificacion = "1-0000-0000",
                Nombre = "Ana",
                Apellidos = "Solís Mora",
                Email = EmailProfesionalDemo,
                PasswordHash = passwordHasher.Hash(PasswordProfesionalDemo),
                Telefono = "2222-0000",
                RolId = rolProfesional.Id,
                Activo = true,
                FechaRegistro = DateTime.UtcNow
            };

            context.Usuarios.Add(usuario);
            await context.SaveChangesAsync();
        }

        if (await context.Profesionales.AnyAsync(p => p.UsuarioId == usuario.Id)) return;

        var especialidad = await context.Especialidades.OrderBy(e => e.Id).FirstAsync();

        context.Profesionales.Add(new Profesional
        {
            UsuarioId = usuario.Id,
            EspecialidadId = especialidad.Id,
            NumColegiado = "CPPCR-0001",
            Disponible = true
        });

        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Líneas de ayuda reales de Costa Rica (fuente: MEP Costa Rica).
    /// Estos números se muestran a personas en crisis: no cambiar sin verificar.
    /// </summary>
    private static async Task SeedLineasAyudaAsync(AquiEstoyDbContext context)
    {
        var deseadas = new[]
        {
            new LineaAyuda
            {
                Nombre = "Emergencias 911",
                Telefono = "911",
                Descripcion = "Disponible 24/7",
                Activa = true
            },
            new LineaAyuda
            {
                Nombre = "Línea Aquí Estoy (MEP - estudiantes)",
                Telefono = "2459-1598 / 2459-1599",
                Descripcion = "Lunes a viernes, 7:00 a.m. a 3:00 p.m.",
                Activa = true
            },
            new LineaAyuda
            {
                Nombre = "Línea Aquí Estoy (Colegio de Profesionales en Psicología de Costa Rica)",
                Telefono = "800-2737869 (800-AQESTOY)",
                Descripcion = "Apoyo en crisis de ansiedad, depresión y riesgo suicida. " +
                              "Lunes a viernes 2:00pm-10:00pm, sábados 9:00am-4:00pm",
                Activa = true
            }
        };

        var existentes = await context.LineasAyuda.Select(l => l.Nombre).ToListAsync();

        var faltantes = deseadas.Where(l => !existentes.Contains(l.Nombre)).ToList();
        if (faltantes.Count == 0) return;

        context.LineasAyuda.AddRange(faltantes);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Tipos de alerta para el módulo de evaluación de riesgo: uno por nivel calculado
    /// (ver AlertaRiesgoService.CalcularNivel), para poder filtrar/reportar por nivel.
    /// </summary>
    private static async Task SeedTiposAlertaAsync(AquiEstoyDbContext context)
    {
        var deseados = AlertaRiesgoService.NombresTiposAlertaPorNivel.Values
            .Select(nombre => new TipoAlerta { Nombre = nombre })
            .ToArray();

        var existentes = await context.TiposAlerta.Select(t => t.Nombre).ToListAsync();

        var faltantes = deseados.Where(t => !existentes.Contains(t.Nombre)).ToList();
        if (faltantes.Count == 0) return;

        context.TiposAlerta.AddRange(faltantes);
        await context.SaveChangesAsync();
    }
}
