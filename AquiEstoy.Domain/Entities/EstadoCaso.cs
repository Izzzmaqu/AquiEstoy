namespace AquiEstoy.Domain.Entities
{
    public class EstadoCaso
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;

        public ICollection<Caso> Casos { get; set; } = new List<Caso>();
    }
}
