using AquiEstoy.Application.DTOs.Estadisticas;
using AquiEstoy.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AquiEstoy.API.Controllers
{
    [ApiController]
    [Route("api/estadisticas")]
    public class EstadisticasController : ControllerBase
    {
        private readonly IEstadisticaService _estadisticaService;

        public EstadisticasController(
            IEstadisticaService estadisticaService)
        {
            _estadisticaService = estadisticaService;
        }

        [HttpGet("panel")]
        public async Task<ActionResult<PanelEstadisticoDto>> ObtenerPanel()
        {
            var panel = await _estadisticaService.ObtenerPanelAsync();

            return Ok(panel);
        }
    }
}