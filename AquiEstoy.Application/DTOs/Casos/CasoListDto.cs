namespace AquiEstoy.Application.DTOs.Casos
{
    public class CasoListDto
    {
        public int Id { get; set; }

        public int PacienteId { get; set; }
        public string PacienteNombreCompleto { get; set; } = null!;
        public int? PacienteProvinciaId { get; set; }
        public string? PacienteProvinciaNombre { get; set; }

        public int? ProfesionalId { get; set; }
        public string? ProfesionalNombreCompleto { get; set; }

        public int NivelSeveridadId { get; set; }
        public string NivelSeveridadNombre { get; set; } = null!;
        public string? NivelSeveridadColor { get; set; }

        public int EstadoCasoId { get; set; }
        public string EstadoCasoNombre { get; set; } = null!;

        public DateTime FechaApertura { get; set; }
        public DateTime? FechaCierre { get; set; }
        public string? Descripcion { get; set; }
    }
}
