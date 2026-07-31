namespace AquiEstoy.Application.DTOs.Casos
{
    public class HistorialSeveridadDto
    {
        public int Id { get; set; }

        public int? SeveridadAnteriorId { get; set; }
        public string? SeveridadAnteriorNombre { get; set; }

        public int SeveridadNuevaId { get; set; }
        public string SeveridadNuevaNombre { get; set; } = null!;

        public int CambiadoPorId { get; set; }
        public string CambiadoPorNombreCompleto { get; set; } = null!;

        public DateTime FechaCambio { get; set; }
        public string? Motivo { get; set; }
    }
}
