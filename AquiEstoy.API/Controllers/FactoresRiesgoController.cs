using AquiEstoy.Application.DTOs.FactoresRiesgo;
using AquiEstoy.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AquiEstoy.API.Controllers
{
    [ApiController]
    [Route("api/factores-riesgo")]
    public class FactoresRiesgoController : ControllerBase
    {
        private readonly ICasoService _casoService;

        public FactoresRiesgoController(ICasoService casoService)
        {
            _casoService = casoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FactorRiesgoDto>>> Listar()
        {
            var factores = await _casoService.ListarFactoresRiesgoAsync();
            return Ok(factores);
        }
    }
}
