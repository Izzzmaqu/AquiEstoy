namespace AquiEstoy.Domain.Entities
{
    public class ReporteEstadistico
    {
        public int Id { get; set; }
        public int GeneradoPorId { get; set; }
        public DateTime FechaGeneracion { get; set; } = DateTime.UtcNow;
        public string TipoReporte { get; set; } = null!;
        public string? ParametrosFiltro { get; set; }
        public string? ResultadoResumen { get; set; }

        public Usuario GeneradoPor { get; set; } = null!;
    }
}
