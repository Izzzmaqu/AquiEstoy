using BCrypt.Net;
using AquiEstoy.Domain.Entities;
using AquiEstoy.Infrastructure.Data;
using AquiEstoy.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;

namespace AquiEstoy.Web.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly AquiEstoyDbContext _context;

        public UsuariosController(AquiEstoyDbContext context)
        {
            _context = context;
        }

        // GET: /Usuarios
        public async Task<IActionResult> Index()
        {
            // Cargar el Rol 'Paciente' para filtrar de manera dinámica y segura
            var rolPaciente = await _context.Roles.FirstOrDefaultAsync(r => r.Nombre == "Paciente");
            int rolId = rolPaciente?.Id ?? 2;

            var pacientes = await _context.Usuarios
                .Include(u => u.CasosComoPaciente)
                .Where(u => u.RolId == rolId)
                .OrderByDescending(u => u.FechaRegistro)
                .ToListAsync();

            return View(pacientes);
        }

        // POST: /Usuarios/Registrar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar(RegistrarPacienteViewModel model)
        {
            // Obtener el rol 'Paciente' dinámicamente desde la BD
            var rolPaciente = await _context.Roles.FirstOrDefaultAsync(r => r.Nombre == "Paciente");

            if (rolPaciente == null)
            {
                ModelState.AddModelError("", "El rol 'Paciente' no existe en la base de datos.");
                var listaFallback = await _context.Usuarios.ToListAsync();
                return View("Index", listaFallback);
            }

            if (!ModelState.IsValid)
            {
                var pacientesList = await _context.Usuarios.Where(u => u.RolId == rolPaciente.Id).ToListAsync();
                return View("Index", pacientesList);
            }

            // Validar que el correo no esté duplicado
            if (await _context.Usuarios.AnyAsync(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "El correo ya se encuentra registrado.");
                var pacientesList = await _context.Usuarios.Where(u => u.RolId == rolPaciente.Id).ToListAsync();
                return View("Index", pacientesList);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Crear Paciente con la clave foránea real del Rol
                var nuevoPaciente = new Usuario
                {
                    Identificacion = model.Identificacion,
                    Nombre = model.Nombre,
                    Apellidos = model.Apellidos,
                    Email = model.Email,
                    PasswordHash = string.IsNullOrEmpty(model.Password)
                        ? null
                        : BCrypt.Net.BCrypt.HashPassword(model.Password),
                    Telefono = model.Telefono,
                    FechaNacimiento = model.FechaNacimiento,
                    RolId = rolPaciente.Id, // Usamos el ID obtenido dinámicamente
                    ProvinciaId = model.ProvinciaId,
                    CantonId = model.CantonId,
                    Activo = true,
                    FechaRegistro = DateTime.UtcNow
                };

                _context.Usuarios.Add(nuevoPaciente);
                await _context.SaveChangesAsync();

                // 2. Crear Caso clínico automático
                var nuevoCaso = new Caso
                {
                    PacienteId = nuevoPaciente.Id,
                    FechaApertura = DateTime.UtcNow,
                    Descripcion = "Caso asignado tras el alta del paciente.",
                    EstadoCasoId = 1,      // Estado inicial requerido por la base de datos
                    NivelSeveridadId = 1   // Nivel de severidad inicial requerido por la base de datos
                };

                _context.Casos.Add(nuevoCaso);
                await _context.SaveChangesAsync();

                // 3. Crear Conversación asociada
                int profesionalIdActual = 2; // Reemplazar según el Id del doctor en sesión

                var nuevaConversacion = new Conversacion
                {
                    CasoId = nuevoCaso.Id,
                    PacienteId = nuevoPaciente.Id,
                    ProfesionalId = profesionalIdActual,
                    FechaInicio = DateTime.UtcNow,
                    Activa = true
                };

                _context.Conversaciones.Add(nuevaConversacion);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                TempData["Exito"] = $"Paciente {nuevoPaciente.Nombre} {nuevoPaciente.Apellidos} registrado con éxito.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                // Capturamos el mensaje interno detallado de SQL Server / EF Core
                var mensajeReal = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                ModelState.AddModelError("", "Error en BD: " + mensajeReal);

                var pacientesList = await _context.Usuarios.Where(u => u.RolId == rolPaciente.Id).ToListAsync();
                return View("Index", pacientesList);
            }
        }
    }
}