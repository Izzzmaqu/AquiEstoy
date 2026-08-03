using System.ComponentModel.DataAnnotations;

namespace AquiEstoy.Application.DTOs.Usuarios
{
    /// <summary>
    /// Datos de entrada para POST /api/usuarios/registrar-paciente.
    /// Refleja exactamente los campos de RegistrarPacienteViewModel en la capa Web.
    /// </summary>
    public class RegistrarPacienteDto
    {
        [Required, MaxLength(30)]
        public string Identificacion { get; set; } = null!;

        [Required, MaxLength(100)]
        public string Nombre { get; set; } = null!;

        [Required, MaxLength(100)]
        public string Apellidos { get; set; } = null!;

        [Required, EmailAddress, MaxLength(150)]
        public string Email { get; set; } = null!;

        [Required, MinLength(6)]
        public string Password { get; set; } = null!;

        [MaxLength(20)]
        public string? Telefono { get; set; }

        public DateOnly? FechaNacimiento { get; set; }

        public int? ProvinciaId { get; set; }

        public int? CantonId { get; set; }
    }
}
