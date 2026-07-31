using AquiEstoy.Application.DTOs.Casos;
using AquiEstoy.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AquiEstoy.API.Controllers
{
    [ApiController]
    [Route("api/casos")]
    public class CasosController : ControllerBase
    {
        private readonly ICasoService _casoService;

        public CasosController(ICasoService casoService)
        {
            _casoService = casoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CasoListDto>>> Listar([FromQuery] CasoFiltroDto filtro)
        {
            var casos = await _casoService.ListarCasosAsync(filtro);
            return Ok(casos);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CasoDetalleDto>> ObtenerDetalle(int id)
        {
            var caso = await _casoService.ObtenerDetalleAsync(id);
            if (caso == null) return NotFound();
            return Ok(caso);
        }

        [HttpPost]
        public async Task<ActionResult<CasoListDto>> Crear([FromBody] CrearCasoDto dto)
        {
            try
            {
                var creado = await _casoService.CrearCasoAsync(dto);
                return CreatedAtAction(nameof(ObtenerDetalle), new { id = creado.Id }, creado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<CasoListDto>> Editar(int id, [FromBody] EditarCasoDto dto)
        {
            try
            {
                var actualizado = await _casoService.EditarCasoAsync(id, dto);
                if (actualizado == null) return NotFound();
                return Ok(actualizado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("{id:int}/cerrar")]
        public async Task<ActionResult<CasoListDto>> Cerrar(int id, [FromBody] CerrarCasoDto dto)
        {
            try
            {
                var cerrado = await _casoService.CerrarCasoAsync(id, dto);
                if (cerrado == null) return NotFound();
                return Ok(cerrado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id:int}/factores-riesgo")]
        public async Task<ActionResult<CasoFactorRiesgoDto>> RegistrarFactorRiesgo(int id, [FromBody] RegistrarFactorRiesgoDto dto)
        {
            try
            {
                var registrado = await _casoService.RegistrarFactorRiesgoAsync(id, dto);
                if (registrado == null) return NotFound();
                return CreatedAtAction(nameof(ObtenerDetalle), new { id }, registrado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
