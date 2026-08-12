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
        //
        // Restringido por rol a proposito: con solo [Authorize], un paciente con
        // sesion activa podia entrar aqui. Y como la cookie de autenticacion es
        // unica por navegador (Web/Program.cs), si un profesional dejaba esta
        // pantalla abierta en una pestana y despues alguien iniciaba sesion como
        // paciente en otra pestana del MISMO navegador, recargar esta pestana
        // heredaba en silencio la identidad del paciente: el usuarioId embebido
        // en la vista pasaba a ser el del paciente sin que nadie volviera a
        // iniciar sesion aqui, y el chat mostraba "Tu" en el mensaje ajeno.
        // Restringir por rol no permite dos identidades simultaneas en el mismo
        // navegador (eso es inherente a una sola cookie de sesion), pero convierte
        // ese cruce silencioso en un rechazo explicito en vez de contenido con la
        // identidad equivocada.
        [Authorize(Roles = "Profesional,Admin")]
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
