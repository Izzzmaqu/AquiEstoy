using System.Security.Claims;
using AquiEstoy.Web.Models;
using AquiEstoy.Web.Models.Api;
using AquiEstoy.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace AquiEstoy.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly AquiEstoyApiClient _api;

        public AccountController(AquiEstoyApiClient api)
        {
            _api = api;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Las credenciales se verifican contra la tabla Usuarios via la API
            // (hash BCrypt). Ya no hay usuarios quemados en el codigo.
            var sesion = await _api.LoginAsync(new LoginRequest
            {
                Email = model.Email,
                Password = model.Password
            });

            if (sesion == null)
            {
                ModelState.AddModelError("", "Credenciales incorrectas. Intenta de nuevo.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, sesion.Email),
                new Claim(ClaimTypes.GivenName, $"{sesion.Nombre} {sesion.Apellidos}"),
                new Claim(ClaimTypes.Role, sesion.Rol),
                new Claim(SesionUsuario.ClaimUsuarioId, sesion.UsuarioId.ToString())
            };

            if (sesion.ProfesionalId.HasValue)
                claims.Add(new Claim(SesionUsuario.ClaimProfesionalId, sesion.ProfesionalId.Value.ToString()));

            var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");

            await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity));

            return sesion.Rol == "Paciente"
                ? RedirectToAction("PacienteDashboard", "Paciente")
                : RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult AccessDenied() => View("Login");
    }
}
