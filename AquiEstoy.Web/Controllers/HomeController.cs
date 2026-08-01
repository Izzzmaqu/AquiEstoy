using AquiEstoy.Infrastructure.Data;
using AquiEstoy.Domain.Entities;
using AquiEstoy.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace AquiEstoy.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AquiEstoyDbContext _context;

        public HomeController(ILogger<HomeController> logger, AquiEstoyDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Carga la vista del Profesional con el historial basado en la entidad Conversacion
        public async Task<IActionResult> ChatSeguro(int conversacionId = 1)
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