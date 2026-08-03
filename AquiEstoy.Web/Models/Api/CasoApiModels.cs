namespace AquiEstoy.Web.Models.Api
{
    public class CasoListItem
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

        /// <summary>
        /// Unico identificador valido para abrir el chat. No usar Id (el del caso):
        /// Casos.Id y Conversaciones.Id se solapan y apuntarian a otro paciente.
        /// </summary>
        public int? ConversacionId { get; set; }
    }

    public class CasoDetalle : CasoListItem
    {
        public string? Notas { get; set; }
        public List<HistorialSeveridadItem> HistorialSeveridad { get; set; } = new();
    }

    public class HistorialSeveridadItem
    {
        public int Id { get; set; }
        public string? SeveridadAnteriorNombre { get; set; }
        public string SeveridadNuevaNombre { get; set; } = null!;
        public string CambiadoPorNombreCompleto { get; set; } = null!;
        public DateTime FechaCambio { get; set; }
        public string? Motivo { get; set; }
    }

    /// <summary>
    /// Cuerpo de PUT /api/casos/{id}. UsuarioActorId sale del claim de la cookie,
    /// nunca de un valor fijo.
    /// </summary>
    public class EditarCasoRequest
    {
        public int UsuarioActorId { get; set; }
        public int? ProfesionalId { get; set; }
        public int NivelSeveridadId { get; set; }
        public int EstadoCasoId { get; set; }
        public string? Descripcion { get; set; }
        public string? Notas { get; set; }
        public string? MotivoCambioSeveridad { get; set; }
    }
}
