using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using AquiEstoy.Infrastructure.Data;
using AquiEstoy.Domain.Entities;

namespace AquiEstoy.Web.Hubs
{
    public class ChatHub : Hub
    {
        private readonly AquiEstoyDbContext _context;

        public ChatHub(AquiEstoyDbContext context)
        {
            _context = context;
        }

        public async Task JoinCaseGroup(string casoId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, casoId);
        }

        public async Task SendMessage(string remitenteId, string mensaje, string casoId)
        {
            // Limpiamos el ID por si la vista envía prefijos como "CS-1042"
            string idLimpio = casoId.Replace("CS-", "").Replace("cs-", "").Trim();

            if (int.TryParse(idLimpio, out int targetId) && int.TryParse(remitenteId, out int userId))
            {
                // 1. Buscar la conversación activa vinculada al Id o CasoId
                var conversacion = await _context.Conversaciones
                    .FirstOrDefaultAsync(c => c.Id == targetId || c.CasoId == targetId);

                // 2. Si no existe, creamos la entidad Conversacion básica
                if (conversacion == null)
                {
                    conversacion = new Conversacion
                    {
                        CasoId = targetId,
                        PacienteId = userId,     // ID del paciente en sesión
                        ProfesionalId = 1,       // ID por defecto del profesional asignado
                        FechaInicio = DateTime.UtcNow,
                        Activa = true
                    };

                    _context.Conversaciones.Add(conversacion);
                    await _context.SaveChangesAsync();
                }

                // 3. Crear y guardar el mensaje usando la entidad Conversacion
                var nuevoMensaje = new Mensaje
                {
                    ConversacionId = conversacion.Id,
                    RemitenteId = userId,
                    Contenido = mensaje,
                    FechaEnvio = DateTime.UtcNow
                };

                _context.Mensajes.Add(nuevoMensaje);
                await _context.SaveChangesAsync();

                // 4. Transmitir en tiempo real mediante SignalR
                var hora = nuevoMensaje.FechaEnvio.ToLocalTime().ToString("HH:mm");
                await Clients.Group(casoId).SendAsync("ReceiveMessage", remitenteId, mensaje, hora);
            }
        }
    }
}