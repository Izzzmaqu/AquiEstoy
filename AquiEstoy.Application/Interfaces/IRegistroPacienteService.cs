using AquiEstoy.Application.DTOs.Usuarios;

namespace AquiEstoy.Application.Interfaces
{
    public interface IRegistroPacienteService
    {
        /// <summary>
        /// Crea Usuario + Caso + Conversacion de forma atomica.
        /// Lanza ArgumentException por datos invalidos (p. ej. email duplicado) e
        /// InvalidOperationException si falta seed de catalogos.
        /// </summary>
        Task<PacienteRegistradoDto> RegistrarAsync(RegistrarPacienteDto dto);
    }
}
