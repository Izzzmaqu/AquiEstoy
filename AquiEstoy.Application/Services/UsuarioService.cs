using AquiEstoy.Application.DTOs.Usuarios;
using AquiEstoy.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AquiEstoy.Application.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IAquiEstoyDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public UsuarioService(IAquiEstoyDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<UsuarioSesionDto?> AutenticarAsync(LoginDto dto)
    {
        var usuario = await _context.Usuarios
            .AsNoTracking()
            .Include(u => u.Rol)
            .Include(u => u.Profesional)
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (usuario == null || !usuario.Activo)
            return null;

        if (!_passwordHasher.Verify(dto.Password, usuario.PasswordHash))
            return null;

        return new UsuarioSesionDto
        {
            UsuarioId = usuario.Id,
            Nombre = usuario.Nombre,
            Apellidos = usuario.Apellidos,
            Email = usuario.Email,
            Rol = usuario.Rol.Nombre,
            ProfesionalId = usuario.Profesional?.Id
        };
    }

    public async Task<IEnumerable<PacienteListDto>> ListarPacientesAsync()
    {
        var rolPaciente = await _context.Roles
            .FirstOrDefaultAsync(r => r.Nombre == "Paciente");

        if (rolPaciente == null)
            return Enumerable.Empty<PacienteListDto>();

        return await _context.Usuarios
            .AsNoTracking()
            .Where(u => u.RolId == rolPaciente.Id)
            .OrderByDescending(u => u.FechaRegistro)
            .Select(u => new PacienteListDto
            {
                Id = u.Id,
                Identificacion = u.Identificacion,
                Nombre = u.Nombre,
                Apellidos = u.Apellidos,
                Email = u.Email,
                Telefono = u.Telefono,
                FechaRegistro = u.FechaRegistro,
                CasoId = u.CasosComoPaciente
                    .OrderByDescending(c => c.FechaApertura)
                    .Select(c => (int?)c.Id)
                    .FirstOrDefault(),
                ConversacionId = u.ConversacionesComoPaciente
                    .OrderByDescending(c => c.FechaInicio)
                    .Select(c => (int?)c.Id)
                    .FirstOrDefault()
            })
            .ToListAsync();
    }
}
