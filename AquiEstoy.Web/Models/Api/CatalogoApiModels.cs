namespace AquiEstoy.Web.Models.Api
{
    public class LineaAyudaItem
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Telefono { get; set; } = null!;
        public string? Descripcion { get; set; }
    }

    public class DashboardResumen
    {
        public int TotalPacientes { get; set; }
        public int TotalCasos { get; set; }
        public int CasosAbiertos { get; set; }
        public int CasosCerrados { get; set; }
        public List<SeveridadConteo> CasosPorSeveridad { get; set; } = new();
    }

    public class SeveridadConteo
    {
        public int NivelSeveridadId { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Color { get; set; }
        public int Cantidad { get; set; }
    }

    public class EstadoCasoItem
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
    }

    public class NivelSeveridadItem
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Color { get; set; }
    }
}
