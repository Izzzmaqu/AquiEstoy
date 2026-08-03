using AquiEstoy.Application.DTOs.Conversaciones;
using AquiEstoy.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AquiEstoy.Application.Services;

public class ConversacionService : IConversacionService
{
    private readonly IAquiEstoyDbContext _context;

    public ConversacionService(IAquiEstoyDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MensajeDto>?> ListarMensajesAsync(int conversacionId)
    {
        // Solo se acepta el Id de la conversacion. Aceptar tambien el CasoId era una
        // fuga de datos: Casos.Id y Conversaciones.Id son secuencias identity distintas
        // que se solapan, asi que un mismo numero podia resolver a la conversacion de
        // otro paciente.
        if (!await _context.Conversaciones.AnyAsync(c => c.Id == conversacionId))
            return null;

        return await _context.Mensajes
            .AsNoTracking()
            .Where(m => m.ConversacionId == conversacionId)
            .OrderBy(m => m.FechaEnvio)
            .Select(m => new MensajeDto
            {
                Id = m.Id,
                ConversacionId = m.ConversacionId,
                RemitenteId = m.RemitenteId,
                RemitenteNombre = m.Remitente.Nombre + " " + m.Remitente.Apellidos,
                Contenido = m.Contenido,
                FechaEnvio = m.FechaEnvio
            })
            .ToListAsync();
    }
}
