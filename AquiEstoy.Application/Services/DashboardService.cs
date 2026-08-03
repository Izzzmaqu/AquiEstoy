using AquiEstoy.Application.DTOs.Dashboard;
using AquiEstoy.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AquiEstoy.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IAquiEstoyDbContext _context;

    public DashboardService(IAquiEstoyDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardResumenDto> ObtenerResumenAsync()
    {
        var rolPaciente = await _context.Roles
            .FirstOrDefaultAsync(r => r.Nombre == "Paciente");

        var totalPacientes = rolPaciente == null
            ? 0
            : await _context.Usuarios.CountAsync(u => u.RolId == rolPaciente.Id);

        var totalCasos = await _context.Casos.CountAsync();

        // El estado del caso es la fuente de verdad para el conteo: EditarCasoAsync
        // puede cambiarlo sin tocar FechaCierre.
        var casosCerrados = await _context.Casos
            .CountAsync(c => c.EstadoCaso.Nombre.ToUpper() == "CERRADO");

        var casosPorSeveridad = await _context.Casos
            .GroupBy(c => new
            {
                c.NivelSeveridadId,
                c.NivelSeveridad.Nombre,
                c.NivelSeveridad.Color
            })
            .Select(g => new SeveridadConteoDto
            {
                NivelSeveridadId = g.Key.NivelSeveridadId,
                Nombre = g.Key.Nombre,
                Color = g.Key.Color,
                Cantidad = g.Count()
            })
            .ToListAsync();

        return new DashboardResumenDto
        {
            TotalPacientes = totalPacientes,
            TotalCasos = totalCasos,
            CasosAbiertos = totalCasos - casosCerrados,
            CasosCerrados = casosCerrados,
            CasosPorSeveridad = casosPorSeveridad
                .OrderBy(s => s.NivelSeveridadId)
                .ToList()
        };
    }
}
