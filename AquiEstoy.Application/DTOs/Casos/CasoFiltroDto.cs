namespace AquiEstoy.Application.DTOs.Casos
{
    public class CasoFiltroDto
    {
        public int? EstadoCasoId { get; set; }
        public int? NivelSeveridadId { get; set; }

        /// <summary>Filtra por la provincia del paciente (Caso no tiene provincia propia).</summary>
        public int? ProvinciaId { get; set; }

        public int? ProfesionalId { get; set; }

        /// <summary>Filtra los casos de un paciente concreto (dashboard del paciente).</summary>
        public int? PacienteId { get; set; }
    }
}
