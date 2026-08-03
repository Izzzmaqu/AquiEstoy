using AquiEstoy.Application.DTOs.Catalogos;
using AquiEstoy.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AquiEstoy.Application.Services;

public class CatalogoService : ICatalogoService
{
    private readonly IAquiEstoyDbContext _context;

    public CatalogoService(IAquiEstoyDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<EstadoCasoDto>> ListarEstadosCasoAsync() =>
        await _context.EstadosCaso
            .AsNoTracking()
            .OrderBy(e => e.Id)
            .Select(e => new EstadoCasoDto { Id = e.Id, Nombre = e.Nombre })
            .ToListAsync();

    public async Task<IEnumerable<NivelSeveridadDto>> ListarNivelesSeveridadAsync() =>
        await _context.NivelesSeveridad
            .AsNoTracking()
            .OrderBy(n => n.Id)
            .Select(n => new NivelSeveridadDto { Id = n.Id, Nombre = n.Nombre, Color = n.Color })
            .ToListAsync();
}
