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
        public List<CasoFactorRiesgoItem> FactoresRiesgo { get; set; } = new();
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

    /// <summary>
    /// Cuerpo de PATCH /api/casos/{id}/cerrar. Es el unico endpoint que asigna
    /// FechaCierre; el PUT generico cambia el estado pero la deja nula.
    /// </summary>
    public class CerrarCasoRequest
    {
        public int UsuarioActorId { get; set; }
        public string? Notas { get; set; }
    }

    /// <summary>Item del catalogo que devuelve GET /api/factores-riesgo.</summary>
    public class FactorRiesgoItem
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public decimal PesoRiesgo { get; set; }
        public int CategoriaId { get; set; }
        public string CategoriaNombre { get; set; } = null!;
    }

    /// <summary>Factor ya registrado en un caso, tal como viene en el detalle.</summary>
    public class CasoFactorRiesgoItem
    {
        public int Id { get; set; }
        public int FactorRiesgoId { get; set; }
        public string FactorRiesgoNombre { get; set; } = null!;
        public string CategoriaNombre { get; set; } = null!;
        public string? Observacion { get; set; }
        public DateTime FechaRegistro { get; set; }
    }

    /// <summary>Cuerpo de POST /api/casos/{id}/factores-riesgo.</summary>
    public class RegistrarFactorRiesgoRequest
    {
        public int FactorRiesgoId { get; set; }
        public string? Observacion { get; set; }
    }

    /// <summary>Cuerpo de POST /api/alertas-riesgo/evaluar.</summary>
    public class EvaluarRiesgoRequest
    {
        public int CasoId { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public double PuntajeRiesgo { get; set; }
    }

    public class EvaluarRiesgoResponse
    {
        public int? AlertaId { get; set; }
        public string NivelRiesgo { get; set; } = string.Empty;
        public bool MostrarAlerta { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public List<LineaAyudaItem> LineasAyuda { get; set; } = new();
    }
}
