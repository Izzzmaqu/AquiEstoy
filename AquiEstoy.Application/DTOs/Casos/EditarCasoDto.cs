namespace AquiEstoy.Application.DTOs.Casos
{
    public class EditarCasoDto
    {
        /// <summary>Actor que realiza la edición (sin auth todavía, se recibe explícito).</summary>
        public int UsuarioActorId { get; set; }

        public int? ProfesionalId { get; set; }
        public int NivelSeveridadId { get; set; }
        public int EstadoCasoId { get; set; }
        public string? Descripcion { get; set; }
        public string? Notas { get; set; }

        /// <summary>Motivo del cambio de severidad, si aplica. Si se omite se usa un texto genérico.</summary>
        public string? MotivoCambioSeveridad { get; set; }
    }
}
