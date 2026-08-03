using AquiEstoy.Application.DTOs;
using AquiEstoy.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace AquiEstoy.API.Controllers;

[ApiController]
[Route("api/alertas-riesgo")]
public class AlertasRiesgoController : ControllerBase
{
    private readonly AlertaRiesgoService _service;

    public AlertasRiesgoController(AlertaRiesgoService service)
    {
        _service = service;
    }

    [HttpPost("evaluar")]
    public async Task<ActionResult<EvaluarRiesgoResponse>> Evaluar(
        [FromBody] EvaluarRiesgoRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await _service.EvaluarAsync(
                request,
                cancellationToken);

            return Ok(resultado);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new
            {
                mensaje = exception.Message
            });
        }
    }
}