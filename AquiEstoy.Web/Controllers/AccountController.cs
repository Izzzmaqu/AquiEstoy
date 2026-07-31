using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using AquiEstoy.Web.Models;

namespace AquiEstoy.Web.Controllers
{
    public class AccountController : Controller
    {
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

            // TODO: Aquí conectarás con tu Base de Datos / Servicio
            // Por ahora, lógica simulada para pruebas rápidas del MVP:

            string? role = null;

            if (model.Email == "admin@aquiestoy.com" && model.Password == "admin123")
            {
                role = "Admin";
            }
            else if (model.Email == "paciente@aquiestoy.com" && model.Password == "user123")
            {
                role = "Paciente";
            }

            if (role != null)
            {
                // Crear claims (datos de identidad)
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, model.Email),
                    new Claim(ClaimTypes.Role, role)
                };

                var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");

                // Iniciar sesión (grabar cookie)
                await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity));

                // REDIRECCIÓN SEGÚN EL ROL
                if (role == "Admin")
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("PacienteDashboard", "Paciente");
                }
            }

            ModelState.AddModelError("", "Credenciales incorrectas. Intenta de nuevo.");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Login");
        }
    }
}