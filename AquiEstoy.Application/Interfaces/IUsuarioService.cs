using AquiEstoy.Application.DTOs.Usuarios;

namespace AquiEstoy.Application.Interfaces
{
    public interface IUsuarioService
    {
        /// <summary>Devuelve la identidad del usuario, o null si las credenciales no son validas.</summary>
        Task<UsuarioSesionDto?> AutenticarAsync(LoginDto dto);

        Task<IEnumerable<PacienteListDto>> ListarPacientesAsync();
    }
}
