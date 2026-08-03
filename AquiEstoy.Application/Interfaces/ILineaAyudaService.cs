using AquiEstoy.Application.DTOs.LineasAyuda;

namespace AquiEstoy.Application.Interfaces
{
    public interface ILineaAyudaService
    {
        Task<IEnumerable<LineaAyudaDto>> ListarActivasAsync();
    }
}
