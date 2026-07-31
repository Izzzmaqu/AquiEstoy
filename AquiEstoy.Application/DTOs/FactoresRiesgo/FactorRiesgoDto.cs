namespace AquiEstoy.Application.DTOs.FactoresRiesgo
{
    public class FactorRiesgoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public decimal PesoRiesgo { get; set; }

        public int CategoriaId { get; set; }
        public string CategoriaNombre { get; set; } = null!;
    }
}
