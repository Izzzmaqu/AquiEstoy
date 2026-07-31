using AquiEstoy.Application.DTOs.Casos;
using AquiEstoy.Application.DTOs.FactoresRiesgo;
using AquiEstoy.Application.Interfaces;
using AquiEstoy.Domain.Entities;
using AquiEstoy.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AquiEstoy.Infrastructure.Services
{
    public class CasoService : ICasoService
    {
        private readonly AquiEstoyDbContext _context;

        public CasoService(AquiEstoyDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CasoListDto>> ListarCasosAsync(CasoFiltroDto filtro)
        {
            var query = CasosConIncludesDeLista().AsNoTracking();

            if (filtro.EstadoCasoId.HasValue)
                query = query.Where(c => c.EstadoCasoId == filtro.EstadoCasoId.Value);

            if (filtro.NivelSeveridadId.HasValue)
                query = query.Where(c => c.NivelSeveridadId == filtro.NivelSeveridadId.Value);

            if (filtro.ProvinciaId.HasValue)
                query = query.Where(c => c.Paciente.ProvinciaId == filtro.ProvinciaId.Value);

            if (filtro.ProfesionalId.HasValue)
                query = query.Where(c => c.ProfesionalId == filtro.ProfesionalId.Value);

            var casos = await query
                .OrderByDescending(c => c.FechaApertura)
                .ToListAsync();

            return casos.Select(MapToListDto);
        }

        public async Task<CasoDetalleDto?> ObtenerDetalleAsync(int id)
        {
            var caso = await CasosConIncludesDeLista()
                .Include(c => c.FactoresRiesgo).ThenInclude(fr => fr.FactorRiesgo).ThenInclude(f => f.Categoria)
                .Include(c => c.HistorialSeveridad).ThenInclude(h => h.SeveridadAnterior)
                .Include(c => c.HistorialSeveridad).ThenInclude(h => h.SeveridadNueva)
                .Include(c => c.HistorialSeveridad).ThenInclude(h => h.CambiadoPor)
                .Include(c => c.Adjuntos).ThenInclude(a => a.SubidoPor)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            return caso == null ? null : MapToDetalleDto(caso);
        }

        public async Task<CasoListDto> CrearCasoAsync(CrearCasoDto dto)
        {
            if (!await _context.Usuarios.AnyAsync(u => u.Id == dto.PacienteId))
                throw new ArgumentException($"No existe un Usuario (paciente) con Id {dto.PacienteId}.");

            if (dto.ProfesionalId.HasValue &&
                !await _context.Profesionales.AnyAsync(p => p.Id == dto.ProfesionalId.Value))
                throw new ArgumentException($"No existe un Profesional con Id {dto.ProfesionalId.Value}.");

            if (!await _context.NivelesSeveridad.AnyAsync(n => n.Id == dto.NivelSeveridadId))
                throw new ArgumentException($"No existe un NivelSeveridad con Id {dto.NivelSeveridadId}.");

            if (!await _context.EstadosCaso.AnyAsync(e => e.Id == dto.EstadoCasoId))
                throw new ArgumentException($"No existe un EstadoCaso con Id {dto.EstadoCasoId}.");

            var actorId = dto.UsuarioActorId ?? dto.PacienteId;
            if (actorId != dto.PacienteId && !await _context.Usuarios.AnyAsync(u => u.Id == actorId))
                throw new ArgumentException($"No existe un Usuario (actor) con Id {actorId}.");

            var caso = new Caso
            {
                PacienteId = dto.PacienteId,
                ProfesionalId = dto.ProfesionalId,
                NivelSeveridadId = dto.NivelSeveridadId,
                EstadoCasoId = dto.EstadoCasoId,
                Descripcion = dto.Descripcion,
                Notas = dto.Notas,
                HistorialSeveridad = new List<HistorialSeveridad>
                {
                    new HistorialSeveridad
                    {
                        SeveridadAnterId = null,
                        SeveridadNuevaId = dto.NivelSeveridadId,
                        CambiadoPorId = actorId,
                        Motivo = "Registro inicial del caso"
                    }
                }
            };

            _context.Casos.Add(caso);
            await _context.SaveChangesAsync();

            return await RecargarListDtoAsync(caso.Id);
        }

        public async Task<CasoListDto?> EditarCasoAsync(int id, EditarCasoDto dto)
        {
            var caso = await _context.Casos.FindAsync(id);
            if (caso == null) return null;

            if (!await _context.Usuarios.AnyAsync(u => u.Id == dto.UsuarioActorId))
                throw new ArgumentException($"No existe un Usuario (actor) con Id {dto.UsuarioActorId}.");

            if (dto.ProfesionalId.HasValue &&
                !await _context.Profesionales.AnyAsync(p => p.Id == dto.ProfesionalId.Value))
                throw new ArgumentException($"No existe un Profesional con Id {dto.ProfesionalId.Value}.");

            if (!await _context.NivelesSeveridad.AnyAsync(n => n.Id == dto.NivelSeveridadId))
                throw new ArgumentException($"No existe un NivelSeveridad con Id {dto.NivelSeveridadId}.");

            if (!await _context.EstadosCaso.AnyAsync(e => e.Id == dto.EstadoCasoId))
                throw new ArgumentException($"No existe un EstadoCaso con Id {dto.EstadoCasoId}.");

            var severidadAnteriorId = caso.NivelSeveridadId;
            var severidadCambio = severidadAnteriorId != dto.NivelSeveridadId;

            caso.ProfesionalId = dto.ProfesionalId;
            caso.NivelSeveridadId = dto.NivelSeveridadId;
            caso.EstadoCasoId = dto.EstadoCasoId;
            caso.Descripcion = dto.Descripcion;
            caso.Notas = dto.Notas;

            if (severidadCambio)
            {
                _context.HistorialesSeveridad.Add(new HistorialSeveridad
                {
                    CasoId = caso.Id,
                    SeveridadAnterId = severidadAnteriorId,
                    SeveridadNuevaId = dto.NivelSeveridadId,
                    CambiadoPorId = dto.UsuarioActorId,
                    Motivo = dto.MotivoCambioSeveridad ?? "Actualización de caso"
                });
            }

            await _context.SaveChangesAsync();

            return await RecargarListDtoAsync(id);
        }

        public async Task<CasoListDto?> CerrarCasoAsync(int id, CerrarCasoDto dto)
        {
            var caso = await _context.Casos.FindAsync(id);
            if (caso == null) return null;

            if (!await _context.Usuarios.AnyAsync(u => u.Id == dto.UsuarioActorId))
                throw new ArgumentException($"No existe un Usuario (actor) con Id {dto.UsuarioActorId}.");

            var estadoCerrado = await _context.EstadosCaso
                .SingleOrDefaultAsync(e => e.Nombre.ToUpper() == "CERRADO");

            if (estadoCerrado == null)
                throw new InvalidOperationException(
                    "No existe un EstadoCaso con Nombre 'Cerrado'. Verifique el catálogo de EstadosCaso.");

            caso.EstadoCasoId = estadoCerrado.Id;
            caso.FechaCierre = DateTime.UtcNow;
            if (dto.Notas != null)
                caso.Notas = dto.Notas;

            await _context.SaveChangesAsync();

            return await RecargarListDtoAsync(id);
        }

        public async Task<CasoFactorRiesgoDto?> RegistrarFactorRiesgoAsync(int casoId, RegistrarFactorRiesgoDto dto)
        {
            if (!await _context.Casos.AnyAsync(c => c.Id == casoId))
                return null;

            var factor = await _context.FactoresRiesgo
                .Include(f => f.Categoria)
                .FirstOrDefaultAsync(f => f.Id == dto.FactorRiesgoId);

            if (factor == null)
                throw new ArgumentException($"No existe un FactorRiesgo con Id {dto.FactorRiesgoId}.");

            var casoFactor = new CasoFactorRiesgo
            {
                CasoId = casoId,
                FactorRiesgoId = dto.FactorRiesgoId,
                Observacion = dto.Observacion
            };

            _context.CasoFactoresRiesgo.Add(casoFactor);
            await _context.SaveChangesAsync();

            return new CasoFactorRiesgoDto
            {
                Id = casoFactor.Id,
                FactorRiesgoId = factor.Id,
                FactorRiesgoNombre = factor.Nombre,
                CategoriaNombre = factor.Categoria.Nombre,
                Observacion = casoFactor.Observacion,
                FechaRegistro = casoFactor.FechaRegistro
            };
        }

        public async Task<IEnumerable<FactorRiesgoDto>> ListarFactoresRiesgoAsync()
        {
            var factores = await _context.FactoresRiesgo
                .AsNoTracking()
                .Include(f => f.Categoria)
                .OrderBy(f => f.Categoria.Nombre)
                .ThenBy(f => f.Nombre)
                .ToListAsync();

            return factores.Select(f => new FactorRiesgoDto
            {
                Id = f.Id,
                Nombre = f.Nombre,
                Descripcion = f.Descripcion,
                PesoRiesgo = f.PesoRiesgo,
                CategoriaId = f.CategoriaId,
                CategoriaNombre = f.Categoria.Nombre
            });
        }

        private IQueryable<Caso> CasosConIncludesDeLista() =>
            _context.Casos
                .Include(c => c.Paciente).ThenInclude(p => p.Provincia)
                .Include(c => c.Profesional!).ThenInclude(p => p.Usuario)
                .Include(c => c.NivelSeveridad)
                .Include(c => c.EstadoCaso);

        private async Task<CasoListDto> RecargarListDtoAsync(int id)
        {
            var caso = await CasosConIncludesDeLista()
                .AsNoTracking()
                .FirstAsync(c => c.Id == id);

            return MapToListDto(caso);
        }

        private static CasoListDto MapToListDto(Caso c) => new()
        {
            Id = c.Id,
            PacienteId = c.PacienteId,
            PacienteNombreCompleto = $"{c.Paciente.Nombre} {c.Paciente.Apellidos}",
            PacienteProvinciaId = c.Paciente.ProvinciaId,
            PacienteProvinciaNombre = c.Paciente.Provincia?.Nombre,
            ProfesionalId = c.ProfesionalId,
            ProfesionalNombreCompleto = c.Profesional != null
                ? $"{c.Profesional.Usuario.Nombre} {c.Profesional.Usuario.Apellidos}"
                : null,
            NivelSeveridadId = c.NivelSeveridadId,
            NivelSeveridadNombre = c.NivelSeveridad.Nombre,
            NivelSeveridadColor = c.NivelSeveridad.Color,
            EstadoCasoId = c.EstadoCasoId,
            EstadoCasoNombre = c.EstadoCaso.Nombre,
            FechaApertura = c.FechaApertura,
            FechaCierre = c.FechaCierre,
            Descripcion = c.Descripcion
        };

        private static CasoDetalleDto MapToDetalleDto(Caso c)
        {
            var listDto = MapToListDto(c);

            return new CasoDetalleDto
            {
                Id = listDto.Id,
                PacienteId = listDto.PacienteId,
                PacienteNombreCompleto = listDto.PacienteNombreCompleto,
                PacienteProvinciaId = listDto.PacienteProvinciaId,
                PacienteProvinciaNombre = listDto.PacienteProvinciaNombre,
                ProfesionalId = listDto.ProfesionalId,
                ProfesionalNombreCompleto = listDto.ProfesionalNombreCompleto,
                NivelSeveridadId = listDto.NivelSeveridadId,
                NivelSeveridadNombre = listDto.NivelSeveridadNombre,
                NivelSeveridadColor = listDto.NivelSeveridadColor,
                EstadoCasoId = listDto.EstadoCasoId,
                EstadoCasoNombre = listDto.EstadoCasoNombre,
                FechaApertura = listDto.FechaApertura,
                FechaCierre = listDto.FechaCierre,
                Descripcion = listDto.Descripcion,
                Notas = c.Notas,
                FactoresRiesgo = c.FactoresRiesgo
                    .OrderByDescending(fr => fr.FechaRegistro)
                    .Select(fr => new CasoFactorRiesgoDto
                    {
                        Id = fr.Id,
                        FactorRiesgoId = fr.FactorRiesgoId,
                        FactorRiesgoNombre = fr.FactorRiesgo.Nombre,
                        CategoriaNombre = fr.FactorRiesgo.Categoria.Nombre,
                        Observacion = fr.Observacion,
                        FechaRegistro = fr.FechaRegistro
                    }).ToList(),
                HistorialSeveridad = c.HistorialSeveridad
                    .OrderByDescending(h => h.FechaCambio)
                    .Select(h => new HistorialSeveridadDto
                    {
                        Id = h.Id,
                        SeveridadAnteriorId = h.SeveridadAnterId,
                        SeveridadAnteriorNombre = h.SeveridadAnterior?.Nombre,
                        SeveridadNuevaId = h.SeveridadNuevaId,
                        SeveridadNuevaNombre = h.SeveridadNueva.Nombre,
                        CambiadoPorId = h.CambiadoPorId,
                        CambiadoPorNombreCompleto = $"{h.CambiadoPor.Nombre} {h.CambiadoPor.Apellidos}",
                        FechaCambio = h.FechaCambio,
                        Motivo = h.Motivo
                    }).ToList(),
                Adjuntos = c.Adjuntos
                    .OrderByDescending(a => a.FechaSubida)
                    .Select(a => new AdjuntoCasoDto
                    {
                        Id = a.Id,
                        NombreArchivo = a.NombreArchivo,
                        RutaArchivo = a.RutaArchivo,
                        TipoArchivo = a.TipoArchivo,
                        SubidoPorId = a.SubidoPorId,
                        SubidoPorNombreCompleto = $"{a.SubidoPor.Nombre} {a.SubidoPor.Apellidos}",
                        FechaSubida = a.FechaSubida
                    }).ToList()
            };
        }
    }
}
