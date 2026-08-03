using AquiEstoy.Application.DTOs.Catalogos;

namespace AquiEstoy.Application.Interfaces
{
    /// <summary>Catalogos de solo lectura que alimentan los desplegables de la UI.</summary>
    public interface ICatalogoService
    {
        Task<IEnumerable<EstadoCasoDto>> ListarEstadosCasoAsync();

        Task<IEnumerable<NivelSeveridadDto>> ListarNivelesSeveridadAsync();
    }
}
