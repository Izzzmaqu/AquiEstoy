namespace AquiEstoy.Domain.Entities
{
    public class Canton
    {
        public int Id { get; set; }
        public int ProvinciaId { get; set; }
        public string Nombre { get; set; } = null!;

        public Provincia Provincia { get; set; } = null!;
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
