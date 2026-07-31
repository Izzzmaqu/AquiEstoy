namespace AquiEstoy.Domain.Entities
{
    public class NivelSeveridad
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Color { get; set; }
        public string? Descripcion { get; set; }

        public ICollection<Caso> Casos { get; set; } = new List<Caso>();
        public ICollection<HistorialSeveridad> HistorialesComoAnterior { get; set; } = new List<HistorialSeveridad>();
        public ICollection<HistorialSeveridad> HistorialesComoNuevo { get; set; } = new List<HistorialSeveridad>();
    }
}
