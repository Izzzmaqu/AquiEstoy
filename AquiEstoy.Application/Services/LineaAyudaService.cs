using AquiEstoy.Application.DTOs.LineasAyuda;
using AquiEstoy.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AquiEstoy.Application.Services;

public class LineaAyudaService : ILineaAyudaService
{
    private readonly IAquiEstoyDbContext _context;

    public LineaAyudaService(IAquiEstoyDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<LineaAyudaDto>> ListarActivasAsync()
    {
        return await _context.LineasAyuda
            .AsNoTracking()
            .Where(l => l.Activa)
            .OrderBy(l => l.Id)
            .Select(l => new LineaAyudaDto
            {
                Id = l.Id,
                Nombre = l.Nombre,
                Telefono = l.Telefono,
                Descripcion = l.Descripcion
            })
            .ToListAsync();
    }
}
