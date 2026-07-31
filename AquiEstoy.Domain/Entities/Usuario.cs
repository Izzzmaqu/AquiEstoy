namespace AquiEstoy.Domain.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellidos { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string? Telefono { get; set; }
        public DateOnly? FechaNacimiento { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
        public bool Activo { get; set; } = true;
        public int RolId { get; set; }
        public int? ProvinciaId { get; set; }
        public int? CantonId { get; set; }

        public Rol Rol { get; set; } = null!;
        public Provincia? Provincia { get; set; }
        public Canton? Canton { get; set; }

        // Navegaciones inversas (un Usuario puede aparecer en distintos roles funcionales)
        public Profesional? Profesional { get; set; }
        public ICollection<Caso> CasosComoPaciente { get; set; } = new List<Caso>();
        public ICollection<AdjuntoCaso> AdjuntosSubidos { get; set; } = new List<AdjuntoCaso>();
        public ICollection<HistorialSeveridad> CambiosSeveridadRealizados { get; set; } = new List<HistorialSeveridad>();
        public ICollection<Conversacion> ConversacionesComoPaciente { get; set; } = new List<Conversacion>();
        public ICollection<Mensaje> MensajesEnviados { get; set; } = new List<Mensaje>();
        public ICollection<ReporteEstadistico> ReportesGenerados { get; set; } = new List<ReporteEstadistico>();
        public ICollection<SesionChat> SesionesChat { get; set; } = new List<SesionChat>();
        public ICollection<Alerta> AlertasAtendidas { get; set; } = new List<Alerta>();
        public ICollection<Auditoria> RegistrosAuditoria { get; set; } = new List<Auditoria>();
    }
}
