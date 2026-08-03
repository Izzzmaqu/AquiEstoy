namespace AquiEstoy.Application.DTOs.Usuarios
{
    /// <summary>Fila de la pantalla Usuarios del perfil staff.</summary>
    public class PacienteListDto
    {
        public int Id { get; set; }
        public string Identificacion { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Apellidos { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Telefono { get; set; }
        public DateTime FechaRegistro { get; set; }

        /// <summary>Caso y conversacion abiertos al registrarse. Null si no se crearon.</summary>
        public int? CasoId { get; set; }
        public int? ConversacionId { get; set; }
    }
}
