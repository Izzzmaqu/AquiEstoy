namespace AquiEstoy.Application.DTOs.Casos
{
    public class CasoFactorRiesgoDto
    {
        public int Id { get; set; }

        public int FactorRiesgoId { get; set; }
        public string FactorRiesgoNombre { get; set; } = null!;
        public string CategoriaNombre { get; set; } = null!;

        public string? Observacion { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
