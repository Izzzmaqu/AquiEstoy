namespace AquiEstoy.Domain.Entities
{
    public class CasoFactorRiesgo
    {
        public int Id { get; set; }
        public int CasoId { get; set; }
        public int FactorRiesgoId { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
        public string? Observacion { get; set; }

        public Caso Caso { get; set; } = null!;
        public FactorRiesgo FactorRiesgo { get; set; } = null!;
    }
}
