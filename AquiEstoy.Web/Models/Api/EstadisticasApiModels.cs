namespace AquiEstoy.Web.Models.Api
{
    public class PanelEstadistico
    {
        public int TotalCasos { get; set; }
        public int CasosAbiertos { get; set; }
        public int CasosCerrados { get; set; }
        public int AlertasPendientes { get; set; }

        public List<DatoGrafico> CasosPorSeveridad { get; set; } = new();
        public List<DatoGrafico> CasosPorEstado { get; set; } = new();
        public List<DatoGrafico> CasosPorProvincia { get; set; } = new();
        public List<DatoGrafico> FactoresRiesgoFrecuentes { get; set; } = new();
        public List<DatoGrafico> CasosPorMes { get; set; } = new();
    }

    public class DatoGrafico
    {
        public string Etiqueta { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public string? Color { get; set; }
    }
}