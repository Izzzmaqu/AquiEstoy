using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AquiEstoy.Web.Controllers
{
    [Authorize(Roles = "Paciente")]
    public class PacienteController : Controller
    {
        public IActionResult PacienteDashboard()
        {
            return View();
        }

        // AGREGAR ESTA ACCIÓN
        public IActionResult Chat()
        {
            return View(); // Esto buscará la vista Views/Paciente/Chat.cshtml
        }
    }
}