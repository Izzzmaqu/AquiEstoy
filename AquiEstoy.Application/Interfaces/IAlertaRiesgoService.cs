using AquiEstoy.Application.DTOs.AlertasRiesgo;

namespace AquiEstoy.Application.Interfaces
{
    public interface IAlertaRiesgoService
    {
        Task<EvaluarRiesgoResponse> EvaluarAsync(
            EvaluarRiesgoRequest request,
            CancellationToken cancellationToken = default);
    }
}
