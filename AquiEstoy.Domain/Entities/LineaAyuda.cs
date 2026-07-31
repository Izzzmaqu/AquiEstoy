namespace AquiEstoy.Domain.Entities
{
    public class LineaAyuda
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Telefono { get; set; } = null!;
        public string? Descripcion { get; set; }
        public bool Activa { get; set; } = true;
    }
}
