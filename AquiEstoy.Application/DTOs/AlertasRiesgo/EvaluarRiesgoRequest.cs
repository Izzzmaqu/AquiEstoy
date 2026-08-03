namespace AquiEstoy.Application.DTOs.AlertasRiesgo
{
    public class EvaluarRiesgoRequest
    {
        public int CasoId { get; set; }

        public string Mensaje { get; set; } = string.Empty;

        // Este valor puede ser enviado por la IA.
        public double PuntajeRiesgo { get; set; }
    }
}
