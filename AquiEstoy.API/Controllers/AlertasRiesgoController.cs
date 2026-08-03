using AquiEstoy.Application.DTOs.AlertasRiesgo;
using AquiEstoy.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AquiEstoy.API.Controllers;

[ApiController]
[Route("api/alertas-riesgo")]
public class AlertasRiesgoController : ControllerBase
{
    private readonly IAlertaRiesgoService _service;

    public AlertasRiesgoController(IAlertaRiesgoService service)
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
