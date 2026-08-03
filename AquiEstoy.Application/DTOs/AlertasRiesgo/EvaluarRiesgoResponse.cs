using AquiEstoy.Application.DTOs.LineasAyuda;

namespace AquiEstoy.Application.DTOs.AlertasRiesgo
{
    public class EvaluarRiesgoResponse
    {
        public int? AlertaId { get; set; }

        public string NivelRiesgo { get; set; } = string.Empty;

        public bool MostrarAlerta { get; set; }

        public string Mensaje { get; set; } = string.Empty;

        public List<LineaAyudaDto> LineasAyuda { get; set; } = [];
    }
}
