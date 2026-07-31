namespace AquiEstoy.Domain.Entities
{
    public class Profesional
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int EspecialidadId { get; set; }
        public string? NumColegiado { get; set; }
        public bool Disponible { get; set; } = true;

        public Usuario Usuario { get; set; } = null!;
        public Especialidad Especialidad { get; set; } = null!;

        public ICollection<Caso> CasosAsignados { get; set; } = new List<Caso>();
        public ICollection<Conversacion> Conversaciones { get; set; } = new List<Conversacion>();
    }
}
