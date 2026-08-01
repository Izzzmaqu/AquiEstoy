using System.ComponentModel.DataAnnotations;

namespace AquiEstoy.Web.Models
{
    public class RegistrarPacienteViewModel
    {
        [Required(ErrorMessage = "La identificación es requerida.")]
        [Display(Name = "Cédula / Identificación")]
        public string Identificacion { get; set; } = null!;

        [Required(ErrorMessage = "El nombre es requerido.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "Los apellidos son requeridos.")]
        [Display(Name = "Apellidos")]
        public string Apellidos { get; set; } = null!;

        [Required(ErrorMessage = "El correo electrónico es requerido.")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido.")]
        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Debe asignar una contraseña inicial.")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña Temporal")]
        public string Password { get; set; } = null!;

        [Phone]
        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        [Display(Name = "Fecha de Nacimiento")]
        [DataType(DataType.Date)]
        public DateOnly? FechaNacimiento { get; set; }

        [Display(Name = "Provincia")]
        public int? ProvinciaId { get; set; }

        [Display(Name = "Cantón")]
        public int? CantonId { get; set; }
    }
}