using AquiEstoy.Application.Interfaces;
using AquiEstoy.Domain.Entities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace AquiEstoy.API.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IAquiEstoyDbContext _context;

        public ChatHub(IAquiEstoyDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// El grupo es siempre el Id de la Conversacion, nunca el del Caso:
        /// son secuencias identity distintas y mezclarlas dejaba a paciente y
        /// profesional en grupos separados.
        /// </summary>
        public async Task JoinCaseGroup(string conversacionId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversacionId);
        }

        public async Task SendMessage(string remitenteId, string mensaje, string conversacionId)
        {
            if (!int.TryParse(conversacionId?.Trim(), out var idConversacion)
                || !int.TryParse(remitenteId?.Trim(), out var idRemitente))
            {
                throw new HubException("El identificador de conversación o de remitente no es válido.");
            }

            if (string.IsNullOrWhiteSpace(mensaje))
                throw new HubException("El mensaje no puede estar vacío.");

            // Busqueda estricta por Id de conversacion. La conversacion se crea en el
            // registro del paciente, asi que aqui nunca se inventa una.
            var conversacion = await _context.Conversaciones
                .FirstOrDefaultAsync(c => c.Id == idConversacion);

            if (conversacion == null)
                throw new HubException($"No existe la conversación {idConversacion}.");

            var remitente = await _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == idRemitente);

            if (remitente == null)
                throw new HubException($"No existe el usuario {idRemitente}.");

            var nuevoMensaje = new Mensaje
            {
                ConversacionId = conversacion.Id,
                RemitenteId = remitente.Id,
                Contenido = mensaje,
                FechaEnvio = DateTime.UtcNow
            };

            _context.Mensajes.Add(nuevoMensaje);
            await _context.SaveChangesAsync();

            var hora = nuevoMensaje.FechaEnvio.ToLocalTime().ToString("HH:mm");
            var nombreRemitente = $"{remitente.Nombre} {remitente.Apellidos}";

            // Se envia tambien el nombre para que el mensaje en vivo muestre lo mismo
            // que el historial tras recargar.
            await Clients.Group(conversacionId)
                .SendAsync("ReceiveMessage", remitenteId, nombreRemitente, mensaje, hora);
        }
    }
}
