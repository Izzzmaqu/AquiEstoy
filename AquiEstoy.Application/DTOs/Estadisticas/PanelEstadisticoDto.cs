using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AquiEstoy.Application.DTOs.Estadisticas
{
    public class PanelEstadisticoDto
    {
        public int TotalCasos { get; set; }
        public int CasosAbiertos { get; set; }
        public int CasosCerrados { get; set; }
        public int AlertasPendientes { get; set; }

        public List<DatoGraficoDto> CasosPorSeveridad { get; set; } = new();
        public List<DatoGraficoDto> CasosPorEstado { get; set; } = new();
        public List<DatoGraficoDto> CasosPorProvincia { get; set; } = new();
        public List<DatoGraficoDto> FactoresRiesgoFrecuentes { get; set; } = new();
        public List<DatoGraficoDto> CasosPorMes { get; set; } = new();
    }

    public class DatoGraficoDto
    {
        public string Etiqueta { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public string? Color { get; set; }
    }
}