namespace AquiEstoy.Application.DTOs.Dashboard
{
    public class DashboardResumenDto
    {
        public int TotalPacientes { get; set; }
        public int TotalCasos { get; set; }
        public int CasosAbiertos { get; set; }
        public int CasosCerrados { get; set; }

        public List<SeveridadConteoDto> CasosPorSeveridad { get; set; } = new();
    }

    public class SeveridadConteoDto
    {
        public int NivelSeveridadId { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Color { get; set; }
        public int Cantidad { get; set; }
    }
}
