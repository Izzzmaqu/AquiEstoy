using System.Diagnostics;
using AquiEstoy.Web.Models;
using AquiEstoy.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AquiEstoy.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AquiEstoyApiClient _api;

        public HomeController(ILogger<HomeController> logger, AquiEstoyApiClient api)
        {
            _logger = logger;
            _api = api;
        }

        // Dashboard del perfil staff: conteos + notificacion de mensajes nuevos.
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var resumen = await _api.ObtenerResumenAsync();

            if (resumen == null)
            {
                ViewBag.ErrorResumen = "No se pudo obtener el resumen desde la API.";
                resumen = new Models.Api.DashboardResumen();
            }

            return View(resumen);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Chat del lado profesional. El identificador puede ser el del caso o el de
        // la conversacion; la API resuelve ambos.
        [Authorize]
        public async Task<IActionResult> ChatSeguro(int conversacionId)
        {
            var usuarioId = User.UsuarioId();
            if (usuarioId == null) return Forbid();

            var mensajes = await _api.ListarMensajesAsync(conversacionId);

            ViewBag.ConversacionId = conversacionId;
            ViewBag.UsuarioId = usuarioId.Value;

            return View(mensajes);
        }
    }
}
