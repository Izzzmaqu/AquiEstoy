using AquiEstoy.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AquiEstoy.Web.Controllers
{
    [Authorize(Roles = "Paciente")]
    public class PacienteController : Controller
    {
        private readonly AquiEstoyApiClient _api;

        public PacienteController(AquiEstoyApiClient api)
        {
            _api = api;
        }

        public async Task<IActionResult> PacienteDashboard()
        {
            var usuarioId = User.UsuarioId();
            if (usuarioId == null) return Forbid();

            // Los datos propios salen del caso del paciente en sesion (Include(Paciente)).
            var casos = await _api.ListarCasosAsync(pacienteId: usuarioId.Value);

            ViewBag.NombreCompleto = User.NombreCompleto();

            return View(casos);
        }

        // Chat del paciente. La conversacion se resuelve desde su propio caso:
        // no se acepta un id arbitrario por query string.
        public async Task<IActionResult> Chat()
        {
            var usuarioId = User.UsuarioId();
            if (usuarioId == null) return Forbid();

            var casos = await _api.ListarCasosAsync(pacienteId: usuarioId.Value);

            // Se toma el caso mas reciente que tenga conversacion asociada.
            var caso = casos
                .Where(c => c.ConversacionId.HasValue)
                .OrderByDescending(c => c.FechaApertura)
                .FirstOrDefault();

            if (caso == null)
            {
                ViewBag.SinConversacion = true;
                ViewBag.ConversacionId = 0;
                ViewBag.UsuarioId = usuarioId.Value;
                return View(Array.Empty<Models.Api.MensajeItem>());
            }

            // Siempre el Id de la conversacion, nunca el del caso.
            var conversacionId = caso.ConversacionId!.Value;
            var mensajes = await _api.ListarMensajesAsync(conversacionId);

            ViewBag.ConversacionId = conversacionId;
            ViewBag.UsuarioId = usuarioId.Value;

            return View(mensajes);
        }

        public async Task<IActionResult> LineasAyuda()
        {
            var lineas = await _api.ListarLineasAyudaAsync();
            return View(lineas);
        }
    }
}
