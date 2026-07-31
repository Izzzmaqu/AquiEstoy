namespace AquiEstoy.Domain.Entities
{
    public class AdjuntoCaso
    {
        public int Id { get; set; }
        public int CasoId { get; set; }
        public int SubidoPorId { get; set; }
        public string NombreArchivo { get; set; } = null!;
        public string RutaArchivo { get; set; } = null!;
        public string TipoArchivo { get; set; } = null!;
        public DateTime FechaSubida { get; set; } = DateTime.UtcNow;

        public Caso Caso { get; set; } = null!;
        public Usuario SubidoPor { get; set; } = null!;
    }
}
