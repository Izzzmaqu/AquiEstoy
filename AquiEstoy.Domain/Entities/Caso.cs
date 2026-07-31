namespace AquiEstoy.Domain.Entities
{
    public class Caso
    {
        public int Id { get; set; }
        public int PacienteId { get; set; }
        public int? ProfesionalId { get; set; }
        public int NivelSeveridadId { get; set; }
        public int EstadoCasoId { get; set; }
        public DateTime FechaApertura { get; set; } = DateTime.UtcNow;
        public DateTime? FechaCierre { get; set; }
        public string? Descripcion { get; set; }
        public string? Notas { get; set; }

        public Usuario Paciente { get; set; } = null!;
        public Profesional? Profesional { get; set; }
        public NivelSeveridad NivelSeveridad { get; set; } = null!;
        public EstadoCaso EstadoCaso { get; set; } = null!;

        public ICollection<AdjuntoCaso> Adjuntos { get; set; } = new List<AdjuntoCaso>();
        public ICollection<Alerta> Alertas { get; set; } = new List<Alerta>();
        public ICollection<CasoFactorRiesgo> FactoresRiesgo { get; set; } = new List<CasoFactorRiesgo>();
        public ICollection<Conversacion> Conversaciones { get; set; } = new List<Conversacion>();
        public ICollection<HistorialSeveridad> HistorialSeveridad { get; set; } = new List<HistorialSeveridad>();
    }
}
