namespace AquiEstoy.Domain.Entities
{
    public class Provincia
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;

        public ICollection<Canton> Cantones { get; set; } = new List<Canton>();
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
