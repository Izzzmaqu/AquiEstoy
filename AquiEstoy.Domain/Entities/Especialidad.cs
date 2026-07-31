namespace AquiEstoy.Domain.Entities
{
    public class Especialidad
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;

        public ICollection<Profesional> Profesionales { get; set; } = new List<Profesional>();
    }
}
