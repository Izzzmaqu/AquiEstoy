namespace AquiEstoy.Domain.Entities
{
    public class Alerta
    {
        public int Id { get; set; }
        public int CasoId { get; set; }
        public int TipoAlertaId { get; set; }
        public DateTime FechaGeneracion { get; set; } = DateTime.UtcNow;
        public bool Atendida { get; set; } = false;
        public DateTime? FechaAtencion { get; set; }
        public int? AtendidaPorId { get; set; }
        public string? Descripcion { get; set; }

        public Caso Caso { get; set; } = null!;
        public TipoAlerta TipoAlerta { get; set; } = null!;
        public Usuario? AtendidaPor { get; set; }
    }
}
