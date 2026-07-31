namespace AquiEstoy.Application.DTOs.Casos
{
    public class CasoDetalleDto : CasoListDto
    {
        public string? Notas { get; set; }

        public List<CasoFactorRiesgoDto> FactoresRiesgo { get; set; } = new();
        public List<HistorialSeveridadDto> HistorialSeveridad { get; set; } = new();
        public List<AdjuntoCasoDto> Adjuntos { get; set; } = new();
    }
}
