namespace AquiEstoy.Web.Models.Api
{
    // Contratos propios de la capa Web para hablar con AquiEstoy.API.
    // Se declaran aqui a proposito: Web no referencia Application ni Domain,
    // asi que es un cliente HTTP puro.

    public class RegistrarPacienteRequest
    {
        public string Identificacion { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Apellidos { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Telefono { get; set; }
        public DateOnly? FechaNacimiento { get; set; }
        public int? ProvinciaId { get; set; }
        public int? CantonId { get; set; }
    }

    public class PacienteRegistradoResponse
    {
        public int UsuarioId { get; set; }
        public int CasoId { get; set; }
        public int ConversacionId { get; set; }
        public string NombreCompleto { get; set; } = null!;
        public string Email { get; set; } = null!;
    }

    public class LoginRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class UsuarioSesionResponse
    {
        public int UsuarioId { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellidos { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Rol { get; set; } = null!;
        public int? ProfesionalId { get; set; }
    }

    public class PacienteListItem
    {
        public int Id { get; set; }
        public string Identificacion { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Apellidos { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Telefono { get; set; }
        public DateTime FechaRegistro { get; set; }
        public int? CasoId { get; set; }
        public int? ConversacionId { get; set; }
    }
}
