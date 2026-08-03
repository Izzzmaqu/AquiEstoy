namespace AquiEstoy.Application.DTOs.Usuarios
{
    /// <summary>
    /// Identidad devuelta tras un login correcto. La capa Web la guarda en claims
    /// de la cookie para que ningun UsuarioId quede hardcodeado en vistas o controllers.
    /// </summary>
    public class UsuarioSesionDto
    {
        public int UsuarioId { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellidos { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Rol { get; set; } = null!;

        /// <summary>Id del registro Profesional si el usuario lo es; null para pacientes.</summary>
        public int? ProfesionalId { get; set; }
    }
}
