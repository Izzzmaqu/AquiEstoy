using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AquiEstoy.Application.DTOs.Estadisticas;
using AquiEstoy.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AquiEstoy.Application.Services
{
    public class EstadisticaService : IEstadisticaService
    {
        private readonly IAquiEstoyDbContext _context;

        public EstadisticaService(IAquiEstoyDbContext context)
        {
            _context = context;
        }

        public async Task<PanelEstadisticoDto> ObtenerPanelAsync()
        {
            var totalCasos = await _context.Casos.CountAsync();

            var casosCerrados = await _context.Casos
                .CountAsync(c =>
                    c.EstadoCaso.Nombre.ToUpper() == "CERRADO");

            var alertasPendientes = await _context.Alertas
                .CountAsync(a => !a.Atendida);

            var casosPorSeveridad = await _context.Casos
                .GroupBy(c => new
                {
                    c.NivelSeveridad.Nombre,
                    c.NivelSeveridad.Color
                })
                .Select(grupo => new DatoGraficoDto
                {
                    Etiqueta = grupo.Key.Nombre,
                    Cantidad = grupo.Count(),
                    Color = grupo.Key.Color
                })
                .OrderByDescending(dato => dato.Cantidad)
                .ToListAsync();

            var casosPorEstado = await _context.Casos
                .GroupBy(c => c.EstadoCaso.Nombre)
                .Select(grupo => new DatoGraficoDto
                {
                    Etiqueta = grupo.Key,
                    Cantidad = grupo.Count()
                })
                .OrderByDescending(dato => dato.Cantidad)
                .ToListAsync();

            var casosPorProvincia = await _context.Casos
                .GroupBy(c => c.Paciente.Provincia != null
                    ? c.Paciente.Provincia.Nombre
                    : "Sin provincia")
                .Select(grupo => new DatoGraficoDto
                {
                    Etiqueta = grupo.Key,
                    Cantidad = grupo.Count()
                })
                .OrderByDescending(dato => dato.Cantidad)
                .ToListAsync();

            var factoresRiesgoFrecuentes = await _context.CasoFactoresRiesgo
                .GroupBy(cf => cf.FactorRiesgo.Nombre)
                .Select(grupo => new DatoGraficoDto
                {
                    Etiqueta = grupo.Key,
                    Cantidad = grupo.Count()
                })
                .OrderByDescending(dato => dato.Cantidad)
                .Take(6)
                .ToListAsync();

            var fechaInicial = DateTime.UtcNow.Date.AddMonths(-5);

            fechaInicial = new DateTime(
                fechaInicial.Year,
                fechaInicial.Month,
                1,
                0,
                0,
                0,
                DateTimeKind.Utc);

            var agrupacionMensual = await _context.Casos
                .Where(c => c.FechaApertura >= fechaInicial)
                .GroupBy(c => new
                {
                    c.FechaApertura.Year,
                    c.FechaApertura.Month
                })
                .Select(grupo => new MesCantidad
                {
                    Anio = grupo.Key.Year,
                    Mes = grupo.Key.Month,
                    Cantidad = grupo.Count()
                })
                .ToListAsync();

            var casosPorMes = CrearListadoUltimosMeses(
                fechaInicial,
                agrupacionMensual);

            return new PanelEstadisticoDto
            {
                TotalCasos = totalCasos,
                CasosAbiertos = totalCasos - casosCerrados,
                CasosCerrados = casosCerrados,
                AlertasPendientes = alertasPendientes,
                CasosPorSeveridad = casosPorSeveridad,
                CasosPorEstado = casosPorEstado,
                CasosPorProvincia = casosPorProvincia,
                FactoresRiesgoFrecuentes = factoresRiesgoFrecuentes,
                CasosPorMes = casosPorMes
            };
        }

        private static List<DatoGraficoDto> CrearListadoUltimosMeses(
            DateTime fechaInicial,
            List<MesCantidad> datos)
        {
            var nombresMeses = new[]
            {
                "Enero",
                "Febrero",
                "Marzo",
                "Abril",
                "Mayo",
                "Junio",
                "Julio",
                "Agosto",
                "Septiembre",
                "Octubre",
                "Noviembre",
                "Diciembre"
            };

            var resultado = new List<DatoGraficoDto>();

            for (var i = 0; i < 6; i++)
            {
                var fecha = fechaInicial.AddMonths(i);

                var registro = datos.FirstOrDefault(d =>
                    d.Anio == fecha.Year &&
                    d.Mes == fecha.Month);

                resultado.Add(new DatoGraficoDto
                {
                    Etiqueta = $"{nombresMeses[fecha.Month - 1]} {fecha.Year}",
                    Cantidad = registro?.Cantidad ?? 0
                });
            }

            return resultado;
        }

        private class MesCantidad
        {
            public int Anio { get; set; }
            public int Mes { get; set; }
            public int Cantidad { get; set; }
        }
    }
}