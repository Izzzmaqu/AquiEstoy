namespace AquiEstoy.Domain.Entities
{
    public class HistorialSeveridad
    {
        public int Id { get; set; }
        public int CasoId { get; set; }
        public int? SeveridadAnterId { get; set; }
        public int SeveridadNuevaId { get; set; }
        public int CambiadoPorId { get; set; }
        public DateTime FechaCambio { get; set; } = DateTime.UtcNow;
        public string? Motivo { get; set; }

        public Caso Caso { get; set; } = null!;
        public NivelSeveridad? SeveridadAnterior { get; set; }
        public NivelSeveridad SeveridadNueva { get; set; } = null!;
        public Usuario CambiadoPor { get; set; } = null!;
    }
}
