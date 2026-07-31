namespace AquiEstoy.Domain.Entities
{
    public class TipoAlerta
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;

        public ICollection<Alerta> Alertas { get; set; } = new List<Alerta>();
    }
}
