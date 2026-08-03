namespace AquiEstoy.Application.DTOs.Catalogos
{
    public class EstadoCasoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
    }

    public class NivelSeveridadDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Color { get; set; }
    }
}
