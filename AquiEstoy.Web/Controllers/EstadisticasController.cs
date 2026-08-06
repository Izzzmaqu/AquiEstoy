using AquiEstoy.Web.Models.Api;
using AquiEstoy.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AquiEstoy.Web.Controllers
{
    [Authorize]
    public class EstadisticasController : Controller
    {
        private readonly AquiEstoyApiClient _api;
        private readonly ILogger<EstadisticasController> _logger;

        public EstadisticasController(
            AquiEstoyApiClient api,
            ILogger<EstadisticasController> logger)
        {
            _api = api;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var panel = await _api.ObtenerPanelEstadisticoAsync();

            if (panel == null)
            {
                _logger.LogWarning(
                    "No se pudo obtener el panel estadístico desde la API.");

                ViewBag.Error = "No se pudieron cargar las estadísticas.";

                panel = new PanelEstadistico();
            }

            return View(panel);
        }
    }
}