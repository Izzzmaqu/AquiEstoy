namespace AquiEstoy.Application.DTOs.Casos
{
    public class AdjuntoCasoDto
    {
        public int Id { get; set; }
        public string NombreArchivo { get; set; } = null!;
        public string RutaArchivo { get; set; } = null!;
        public string TipoArchivo { get; set; } = null!;

        public int SubidoPorId { get; set; }
        public string SubidoPorNombreCompleto { get; set; } = null!;

        public DateTime FechaSubida { get; set; }
    }
}
