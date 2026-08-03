using AquiEstoy.Application.DTOs.Dashboard;
using AquiEstoy.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AquiEstoy.API.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("resumen")]
        public async Task<ActionResult<DashboardResumenDto>> Resumen()
        {
            var resumen = await _dashboardService.ObtenerResumenAsync();
            return Ok(resumen);
        }
    }
}
