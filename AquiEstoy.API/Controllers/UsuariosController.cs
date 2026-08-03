using AquiEstoy.Application.DTOs.Usuarios;
using AquiEstoy.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AquiEstoy.API.Controllers
{
    [ApiController]
    [Route("api/usuarios")]
    public class UsuariosController : ControllerBase
    {
        private readonly IRegistroPacienteService _registroPacienteService;
        private readonly IUsuarioService _usuarioService;

        public UsuariosController(
            IRegistroPacienteService registroPacienteService,
            IUsuarioService usuarioService)
        {
            _registroPacienteService = registroPacienteService;
            _usuarioService = usuarioService;
        }

        [HttpGet("pacientes")]
        public async Task<ActionResult<IEnumerable<PacienteListDto>>> ListarPacientes()
        {
            var pacientes = await _usuarioService.ListarPacientesAsync();
            return Ok(pacientes);
        }

        /// <summary>
        /// Crea Usuario + Caso + Conversacion en una sola transaccion.
        /// </summary>
        [HttpPost("registrar-paciente")]
        public async Task<ActionResult<PacienteRegistradoDto>> RegistrarPaciente(
            [FromBody] RegistrarPacienteDto dto)
        {
            try
            {
                var registrado = await _registroPacienteService.RegistrarAsync(dto);
                return Ok(registrado);
            }
            catch (ArgumentException ex)
            {
                // Datos invalidos del cliente (p. ej. email duplicado)
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                // Falta seed de catalogos: es un problema de configuracion del servidor,
                // no del cliente, y el mensaje dice exactamente que sembrar.
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<UsuarioSesionDto>> Login([FromBody] LoginDto dto)
        {
            var sesion = await _usuarioService.AutenticarAsync(dto);

            if (sesion == null)
                return Unauthorized(new { message = "Credenciales incorrectas." });

            return Ok(sesion);
        }
    }
}
