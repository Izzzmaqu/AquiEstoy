namespace AquiEstoy.Domain.Entities
{
    public class FactorRiesgo
    {
        public int Id { get; set; }
        public int CategoriaId { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public decimal PesoRiesgo { get; set; } = 1.00m;

        public CategoriaFactor Categoria { get; set; } = null!;
        public ICollection<CasoFactorRiesgo> Casos { get; set; } = new List<CasoFactorRiesgo>();
    }
}
