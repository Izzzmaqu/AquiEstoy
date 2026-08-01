using AquiEstoy.Infrastructure.Data;
using AquiEstoy.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AquiEstoy.Web.Controllers
{
    [Authorize(Roles = "Paciente")]
    public class PacienteController : Controller
    {
        private readonly AquiEstoyDbContext _context;

        public PacienteController(AquiEstoyDbContext context)
        {
            _context = context;
        }

        public IActionResult PacienteDashboard()
        {
            return View();
        }

        // Carga la vista del Paciente con el historial basado en la entidad Conversacion
        public async Task<IActionResult> Chat(int conversacionId = 1)
        {
            var conversacion = await _context.Conversaciones
                .Include(c => c.Mensajes)
                    .ThenInclude(m => m.Remitente)
                .FirstOrDefaultAsync(c => c.Id == conversacionId || c.CasoId == conversacionId);

            var mensajes = conversacion?.Mensajes
                .OrderBy(m => m.FechaEnvio)
                .ToList() ?? new List<Mensaje>();

            ViewBag.ConversacionId = conversacionId;

            return View(mensajes);
        }
    }
}