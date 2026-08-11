using AquiEstoy.Application.DTOs.Casos;
using AquiEstoy.Application.DTOs.Usuarios;
using AquiEstoy.Application.Interfaces;
using AquiEstoy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AquiEstoy.Application.Services;

/// <summary>
/// Orquesta el alta de un paciente: Usuario + Caso + Conversacion en una sola
/// transaccion. Ningun Id de catalogo se hardcodea; todos se resuelven contra la BD
/// y, si falta el dato, se lanza un error que dice exactamente que sembrar.
/// </summary>
public class RegistroPacienteService : IRegistroPacienteService
{
    private readonly IAquiEstoyDbContext _context;
    private readonly ICasoService _casoService;
    private readonly IPasswordHasher _passwordHasher;

    public RegistroPacienteService(
        IAquiEstoyDbContext context,
        ICasoService casoService,
        IPasswordHasher passwordHasher)
    {
        _context = context;
        _casoService = casoService;
        _passwordHasher = passwordHasher;
    }

    public async Task<PacienteRegistradoDto> RegistrarAsync(RegistrarPacienteDto dto)
    {
        // --- Resolucion de catalogos (antes de abrir la transaccion) ---

        var rolPaciente = await _context.Roles
            .FirstOrDefaultAsync(r => r.Nombre == "Paciente");

        if (rolPaciente == null)
            throw new InvalidOperationException(
                "Falta seed: no existe el rol 'Paciente' en la tabla Roles.");

        if (await _context.Usuarios.AnyAsync(u => u.Email == dto.Email))
            throw new ArgumentException($"El correo {dto.Email} ya se encuentra registrado.");

        var nivelSeveridad = await _context.NivelesSeveridad
            .OrderBy(n => n.Id)
            .FirstOrDefaultAsync();

        if (nivelSeveridad == null)
            throw new InvalidOperationException(
                "Falta seed: la tabla NivelesSeveridad esta vacia, no hay severidad inicial que asignar al caso.");

        var estadoAbierto = await _context.EstadosCaso
            .FirstOrDefaultAsync(e => e.Nombre.ToUpper() == "ABIERTO");

        if (estadoAbierto == null)
            throw new InvalidOperationException(
                "Falta seed: no existe el EstadoCaso 'Abierto' en el catalogo EstadosCaso.");

        var profesional = await _context.Profesionales
            .Where(p => p.Disponible)
            .OrderBy(p => p.Id)
            .FirstOrDefaultAsync();

        if (profesional == null)
            throw new InvalidOperationException(
                "Falta seed: no hay ningun Profesional disponible al que asignar la conversacion.");

        // --- Transaccion: las tres entidades se crean juntas o no se crea ninguna ---

        // La transaccion manual va dentro de la estrategia de reintentos: el DbContext
        // esta configurado con EnableRetryOnFailure y SqlServerRetryingExecutionStrategy
        // rechaza cualquier transaccion iniciada por el usuario que no pase por aqui.
        // Si falla algo transitorio, ExecuteAsync reintenta el bloque entero.
        var estrategia = _context.CreateExecutionStrategy();

        return await estrategia.ExecuteAsync(async () =>
        {
            await using var transaction = await _context.BeginTransactionAsync();

            try
            {
                // Las entidades se construyen aqui dentro a proposito: en un reintento
                // el delegate se ejecuta de nuevo y debe partir de instancias limpias.
                var usuario = new Usuario
                {
                    Identificacion = dto.Identificacion,
                    Nombre = dto.Nombre,
                    Apellidos = dto.Apellidos,
                    Email = dto.Email,
                    PasswordHash = _passwordHasher.Hash(dto.Password),
                    Telefono = dto.Telefono,
                    FechaNacimiento = dto.FechaNacimiento,
                    RolId = rolPaciente.Id,
                    ProvinciaId = dto.ProvinciaId,
                    CantonId = dto.CantonId,
                    Activo = true,
                    FechaRegistro = DateTime.UtcNow
                };

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                // Se delega en CasoService en vez de duplicar sus validaciones de FK.
                // Ambos servicios comparten la misma instancia scoped de DbContext, asi que
                // el SaveChangesAsync interno de CasoService se enlista en esta transaccion.
                var caso = await _casoService.CrearCasoAsync(new CrearCasoDto
                {
                    PacienteId = usuario.Id,
                    ProfesionalId = profesional.Id,
                    NivelSeveridadId = nivelSeveridad.Id,
                    EstadoCasoId = estadoAbierto.Id,
                    Descripcion = "Caso abierto automaticamente al registrar al paciente.",
                    UsuarioActorId = usuario.Id
                });

                var conversacion = new Conversacion
                {
                    CasoId = caso.Id,
                    PacienteId = usuario.Id,
                    ProfesionalId = profesional.Id,
                    FechaInicio = DateTime.UtcNow,
                    Activa = true
                };

                _context.Conversaciones.Add(conversacion);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new PacienteRegistradoDto
                {
                    UsuarioId = usuario.Id,
                    CasoId = caso.Id,
                    ConversacionId = conversacion.Id,
                    NombreCompleto = $"{usuario.Nombre} {usuario.Apellidos}",
                    Email = usuario.Email
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }
}
