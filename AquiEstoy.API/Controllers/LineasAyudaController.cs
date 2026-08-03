using AquiEstoy.Application.DTOs.LineasAyuda;
using AquiEstoy.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AquiEstoy.API.Controllers
{
    [ApiController]
    [Route("api/lineas-ayuda")]
    public class LineasAyudaController : ControllerBase
    {
        private readonly ILineaAyudaService _lineaAyudaService;

        public LineasAyudaController(ILineaAyudaService lineaAyudaService)
        {
            _lineaAyudaService = lineaAyudaService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LineaAyudaDto>>> Listar()
        {
            var lineas = await _lineaAyudaService.ListarActivasAsync();
            return Ok(lineas);
        }
    }
}
