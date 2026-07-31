namespace AquiEstoy.Domain.Entities
{
    public class SesionChat
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string ConnectionId { get; set; } = null!;
        public DateTime FechaConexion { get; set; } = DateTime.UtcNow;
        public DateTime? FechaDesconexion { get; set; }

        public Usuario Usuario { get; set; } = null!;
    }
}
