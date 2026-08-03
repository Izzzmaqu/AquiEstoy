using AquiEstoy.Application.DTOs.Conversaciones;

namespace AquiEstoy.Application.Interfaces
{
    public interface IConversacionService
    {
        /// <summary>
        /// Mensajes de la conversacion, ordenados cronologicamente.
        /// El parametro es el Id de la Conversacion, nunca el del Caso.
        /// Devuelve null si la conversacion no existe.
        /// </summary>
        Task<IEnumerable<MensajeDto>?> ListarMensajesAsync(int conversacionId);
    }
}
