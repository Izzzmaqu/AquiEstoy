using AquiEstoy.Web.Models.Api;
using AquiEstoy.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AquiEstoy.Web.Controllers
{
    [Authorize]
    public class CasosController : Controller
    {
        private readonly AquiEstoyApiClient _api;

        public CasosController(AquiEstoyApiClient api)
        {
            _api = api;
        }

        // GET: /Casos
        public async Task<IActionResult> Index()
        {
            var casos = await _api.ListarCasosAsync();
            return View(casos);
        }

        // GET: /Casos/Editar/5
        public async Task<IActionResult> Editar(int id)
        {
            var caso = await _api.ObtenerCasoAsync(id);
            if (caso == null) return NotFound();

            await CargarCatalogosAsync();

            return View(caso);
        }

        // POST: /Casos/Editar/5  -> nota y/o cambio de estado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, int estadoCasoId, int nivelSeveridadId, string? notas)
        {
            var usuarioId = User.UsuarioId();
            if (usuarioId == null) return Forbid();

            var caso = await _api.ObtenerCasoAsync(id);
            if (caso == null) return NotFound();

            var resultado = await _api.EditarCasoAsync(id, new EditarCasoRequest
            {
                // El actor sale del claim de la cookie, nunca de un valor fijo.
                UsuarioActorId = usuarioId.Value,
                ProfesionalId = caso.ProfesionalId,
                NivelSeveridadId = nivelSeveridadId,
                EstadoCasoId = estadoCasoId,
                Descripcion = caso.Descripcion,
                Notas = notas
            });

            if (!resultado.Success)
            {
                ModelState.AddModelError("", resultado.ErrorMessage ?? "No se pudo actualizar el caso.");

                // Se recarga el detalle para no perder el contexto del formulario.
                var recargado = await _api.ObtenerCasoAsync(id);
                await CargarCatalogosAsync();
                return View(recargado ?? caso);
            }

            TempData["Exito"] = $"Caso #{id} actualizado.";
            return RedirectToAction(nameof(Index));
        }

        private async Task CargarCatalogosAsync()
        {
            ViewBag.EstadosCaso = await _api.ListarEstadosCasoAsync();
            ViewBag.NivelesSeveridad = await _api.ListarNivelesSeveridadAsync();
        }
    }
}
