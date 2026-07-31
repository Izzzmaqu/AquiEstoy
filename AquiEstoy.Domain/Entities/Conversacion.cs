namespace AquiEstoy.Domain.Entities
{
    public class Conversacion
    {
        public int Id { get; set; }
        public int CasoId { get; set; }
        public int PacienteId { get; set; }
        public int ProfesionalId { get; set; }
        public DateTime FechaInicio { get; set; } = DateTime.UtcNow;
        public bool Activa { get; set; } = true;

        public Caso Caso { get; set; } = null!;
        public Usuario Paciente { get; set; } = null!;
        public Profesional Profesional { get; set; } = null!;

        public ICollection<Mensaje> Mensajes { get; set; } = new List<Mensaje>();
    }
}
