namespace AquiEstoy.Application.DTOs.Casos
{
    public class CrearCasoDto
    {
        public int PacienteId { get; set; }
        public int? ProfesionalId { get; set; }
        public int NivelSeveridadId { get; set; }
        public int EstadoCasoId { get; set; }
        public string? Descripcion { get; set; }
        public string? Notas { get; set; }

        /// <summary>Actor que registra el caso. Si es null, se usa PacienteId (sin auth todavía).</summary>
        public int? UsuarioActorId { get; set; }
    }
}
