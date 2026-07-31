using AquiEstoy.Application.DTOs.Casos;
using AquiEstoy.Application.DTOs.FactoresRiesgo;

namespace AquiEstoy.Application.Interfaces
{
    public interface ICasoService
    {
        Task<IEnumerable<CasoListDto>> ListarCasosAsync(CasoFiltroDto filtro);

        Task<CasoDetalleDto?> ObtenerDetalleAsync(int id);

        Task<CasoListDto> CrearCasoAsync(CrearCasoDto dto);

        Task<CasoListDto?> EditarCasoAsync(int id, EditarCasoDto dto);

        Task<CasoListDto?> CerrarCasoAsync(int id, CerrarCasoDto dto);

        Task<CasoFactorRiesgoDto?> RegistrarFactorRiesgoAsync(int casoId, RegistrarFactorRiesgoDto dto);

        Task<IEnumerable<FactorRiesgoDto>> ListarFactoresRiesgoAsync();
    }
}
