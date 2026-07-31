namespace AquiEstoy.Domain.Entities
{
    // Log de auditoria desacoplado a proposito: EntidadId no lleva FK
    // porque debe sobrevivir aunque el registro auditado se elimine.
    // El detalle completo del cambio se guarda en DetalleJson.
    public class Auditoria
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Entidad { get; set; } = null!;
        public int EntidadId { get; set; }
        public string Accion { get; set; } = null!;
        public DateTime FechaAccion { get; set; } = DateTime.UtcNow;
        public string? DetalleJson { get; set; }

        public Usuario Usuario { get; set; } = null!;
    }
}
