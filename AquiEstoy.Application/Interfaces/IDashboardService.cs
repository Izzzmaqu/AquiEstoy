using AquiEstoy.Application.DTOs.Dashboard;

namespace AquiEstoy.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardResumenDto> ObtenerResumenAsync();
    }
}
