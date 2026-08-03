using AquiEstoy.Application.DTOs.Conversaciones;
using AquiEstoy.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AquiEstoy.API.Controllers
{
    [ApiController]
    [Route("api/conversaciones")]
    public class ConversacionesController : ControllerBase
    {
        private readonly IConversacionService _conversacionService;

        public ConversacionesController(IConversacionService conversacionService)
        {
            _conversacionService = conversacionService;
        }

        [HttpGet("{id:int}/mensajes")]
        public async Task<ActionResult<IEnumerable<MensajeDto>>> Mensajes(int id)
        {
            var mensajes = await _conversacionService.ListarMensajesAsync(id);

            if (mensajes == null) return NotFound();

            return Ok(mensajes);
        }
    }
}
