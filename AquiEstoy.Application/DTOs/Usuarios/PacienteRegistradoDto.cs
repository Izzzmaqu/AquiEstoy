namespace AquiEstoy.Application.DTOs.Usuarios
{
    /// <summary>Resultado del registro transaccional: las tres entidades creadas.</summary>
    public class PacienteRegistradoDto
    {
        public int UsuarioId { get; set; }
        public int CasoId { get; set; }
        public int ConversacionId { get; set; }
        public string NombreCompleto { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
