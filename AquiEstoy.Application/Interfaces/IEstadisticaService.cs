using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AquiEstoy.Application.DTOs.Estadisticas;

namespace AquiEstoy.Application.Interfaces
{
    public interface IEstadisticaService
    {
        Task<PanelEstadisticoDto> ObtenerPanelAsync();
    }
}