namespace AquiEstoy.Application.DTOs.LineasAyuda
{
    public class LineaAyudaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Telefono { get; set; } = null!;
        public string? Descripcion { get; set; }
    }
}
