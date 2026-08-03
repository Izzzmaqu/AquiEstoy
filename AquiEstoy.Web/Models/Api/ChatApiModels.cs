namespace AquiEstoy.Web.Models.Api
{
    public class MensajeItem
    {
        public int Id { get; set; }
        public int ConversacionId { get; set; }
        public int RemitenteId { get; set; }
        public string RemitenteNombre { get; set; } = null!;
        public string Contenido { get; set; } = null!;
        public DateTime FechaEnvio { get; set; }
    }
}
