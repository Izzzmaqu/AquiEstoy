using System.Globalization;
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

        // POST: /Casos/AgregarFactorRiesgo/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarFactorRiesgo(int id, int factorRiesgoId, string? observacion)
        {
            if (User.UsuarioId() == null) return Forbid();

            var resultado = await _api.RegistrarFactorRiesgoAsync(id, new RegistrarFactorRiesgoRequest
            {
                FactorRiesgoId = factorRiesgoId,
                Observacion = observacion
            });

            if (resultado.Success)
                TempData["Exito"] = "Factor de riesgo registrado en el caso.";
            else
                TempData["Error"] = resultado.ErrorMessage ?? "No se pudo registrar el factor de riesgo.";

            return RedirectToAction(nameof(Editar), new { id });
        }

        // POST: /Casos/EvaluarRiesgo/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EvaluarRiesgo(int id, string? puntajeRiesgo, string? mensaje)
        {
            if (User.UsuarioId() == null) return Forbid();

            // <input type="number"> envia siempre el valor en formato invariante ("0.85"),
            // pero el binding de MVC lo interpretaria con la cultura del servidor. En es-CR
            // el separador decimal es la coma, asi que "0.85" no liga y el puntaje llegaria
            // como 0, devolviendo "Bajo" para cualquier valor. Por eso se parsea a mano.
            if (!double.TryParse(puntajeRiesgo, NumberStyles.Float,
                                 CultureInfo.InvariantCulture, out var puntaje))
            {
                TempData["Error"] = "El puntaje de riesgo debe ser un número entre 0 y 1.";
                return RedirectToAction(nameof(Editar), new { id });
            }

            var resultado = await _api.EvaluarRiesgoAsync(new EvaluarRiesgoRequest
            {
                CasoId = id,
                PuntajeRiesgo = puntaje,
                Mensaje = mensaje ?? string.Empty
            });

            if (resultado.Success && resultado.Data != null)
            {
                // Se pasa por TempData porque la accion redirige: la vista lo pinta al recargar.
                TempData["RiesgoNivel"] = resultado.Data.NivelRiesgo;
                TempData["RiesgoMensaje"] = resultado.Data.Mensaje;
                TempData["RiesgoMostrarAlerta"] = resultado.Data.MostrarAlerta;
                TempData["RiesgoAlertaId"] = resultado.Data.AlertaId;
            }
            else
            {
                TempData["Error"] = resultado.ErrorMessage ?? "No se pudo evaluar el riesgo.";
            }

            return RedirectToAction(nameof(Editar), new { id });
        }

        // POST: /Casos/Cerrar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cerrar(int id, string? notas)
        {
            var usuarioId = User.UsuarioId();
            if (usuarioId == null) return Forbid();

            // PATCH /cerrar en vez del PUT generico: es el que asigna FechaCierre.
            var resultado = await _api.CerrarCasoAsync(id, new CerrarCasoRequest
            {
                UsuarioActorId = usuarioId.Value,
                Notas = notas
            });

            if (!resultado.Success)
            {
                TempData["Error"] = resultado.ErrorMessage ?? "No se pudo cerrar el caso.";
                return RedirectToAction(nameof(Editar), new { id });
            }

            TempData["Exito"] = $"Caso #{id} cerrado.";
            return RedirectToAction(nameof(Index));
        }

        private async Task CargarCatalogosAsync()
        {
            ViewBag.EstadosCaso = await _api.ListarEstadosCasoAsync();
            ViewBag.NivelesSeveridad = await _api.ListarNivelesSeveridadAsync();
            ViewBag.FactoresRiesgo = await _api.ListarFactoresRiesgoAsync();
        }
    }
}
