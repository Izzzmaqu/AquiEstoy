using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AquiEstoy.Infrastructure.Data;

namespace AquiEstoy.Web.Controllers
{
    public class ChatController : Controller
    {
        private readonly AquiEstoyDbContext _context;

        public ChatController(AquiEstoyDbContext context)
        {
            _context = context;
        }

        // Acción para cargar la pantalla del chat filtrada por el id del caso
        public async Task<IActionResult> Index(int casoId)
        {
            // Carga los mensajes guardados de esta conversación ordenados cronológicamente
            var mensajes = await _context.Mensajes
                .Include(m => m.Remitente)
                .Where(m => m.ConversacionId == casoId)
                .OrderBy(m => m.FechaEnvio)
                .ToListAsync();

            ViewBag.CasoId = casoId;

            return View(mensajes);
        }
    }
}