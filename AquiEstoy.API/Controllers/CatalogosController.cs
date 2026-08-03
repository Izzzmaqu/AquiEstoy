using AquiEstoy.Application.DTOs.Catalogos;
using AquiEstoy.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AquiEstoy.API.Controllers
{
    [ApiController]
    [Route("api/catalogos")]
    public class CatalogosController : ControllerBase
    {
        private readonly ICatalogoService _catalogoService;

        public CatalogosController(ICatalogoService catalogoService)
        {
            _catalogoService = catalogoService;
        }

        [HttpGet("estados-caso")]
        public async Task<ActionResult<IEnumerable<EstadoCasoDto>>> EstadosCaso()
        {
            return Ok(await _catalogoService.ListarEstadosCasoAsync());
        }

        [HttpGet("niveles-severidad")]
        public async Task<ActionResult<IEnumerable<NivelSeveridadDto>>> NivelesSeveridad()
        {
            return Ok(await _catalogoService.ListarNivelesSeveridadAsync());
        }
    }
}
