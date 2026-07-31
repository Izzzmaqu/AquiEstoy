namespace AquiEstoy.Domain.Entities
{
    public class Mensaje
    {
        public int Id { get; set; }
        public int ConversacionId { get; set; }
        public int RemitenteId { get; set; }
        public string Contenido { get; set; } = null!;
        public DateTime FechaEnvio { get; set; } = DateTime.UtcNow;
        public bool Leido { get; set; } = false;
        public DateTime? FechaLectura { get; set; }

        public Conversacion Conversacion { get; set; } = null!;
        public Usuario Remitente { get; set; } = null!;
    }
}
