namespace AquiEstoy.Domain.Entities
{
    public class CategoriaFactor
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;

        public ICollection<FactorRiesgo> FactoresRiesgo { get; set; } = new List<FactorRiesgo>();
    }
}
