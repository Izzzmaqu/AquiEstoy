using AquiEstoy.Web.Models;
using AquiEstoy.Web.Models.Api;
using AquiEstoy.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AquiEstoy.Web.Controllers
{
    [Authorize]
    public class UsuariosController : Controller
    {
        private readonly AquiEstoyApiClient _api;

        public UsuariosController(AquiEstoyApiClient api)
        {
            _api = api;
        }

        // GET: /Usuarios
        public async Task<IActionResult> Index()
        {
            var pacientes = await _api.ListarPacientesAsync();
            return View(pacientes);
        }

        // POST: /Usuarios/Registrar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar(RegistrarPacienteViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Index", await _api.ListarPacientesAsync());

            // La API crea Usuario + Caso + Conversacion en una sola transaccion.
            var resultado = await _api.RegistrarPacienteAsync(new RegistrarPacienteRequest
            {
                Identificacion = model.Identificacion,
                Nombre = model.Nombre,
                Apellidos = model.Apellidos,
                Email = model.Email,
                Password = model.Password,
                Telefono = model.Telefono,
                FechaNacimiento = model.FechaNacimiento,
                ProvinciaId = model.ProvinciaId,
                CantonId = model.CantonId
            });

            if (!resultado.Success)
            {
                ModelState.AddModelError("", resultado.ErrorMessage ?? "No se pudo registrar al paciente.");
                return View("Index", await _api.ListarPacientesAsync());
            }

            TempData["Exito"] = $"Paciente {resultado.Data!.NombreCompleto} registrado con éxito.";
            return RedirectToAction(nameof(Index));
        }
    }
}
