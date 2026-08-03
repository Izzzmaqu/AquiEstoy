using System.ComponentModel.DataAnnotations;

namespace AquiEstoy.Application.DTOs.Usuarios
{
    public class LoginDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}
