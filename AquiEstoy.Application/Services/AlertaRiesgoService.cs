using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AquiEstoy.Application.DTOs;
using AquiEstoy.Application.Interfaces;
using AquiEstoy.Domain.Entities;
using AquiEstoy.Domain.Enums;

namespace AquiEstoy.Application.Services;

public class AlertaRiesgoService
{
    private readonly IAlertaRiesgoRepository _repository;

    public AlertaRiesgoService(IAlertaRiesgoRepository repository)
    {
        _repository = repository;
    }

    public async Task<EvaluarRiesgoResponse> EvaluarAsync(
        EvaluarRiesgoRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.UsuarioId == Guid.Empty)
        {
            throw new ArgumentException("El usuario es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(request.Mensaje))
        {
            throw new ArgumentException("El mensaje es obligatorio.");
        }

        NivelRiesgo nivel = CalcularNivel(request.PuntajeRiesgo);

        var alerta = new AlertaRiesgo
        {
            UsuarioId = request.UsuarioId,
            Nivel = nivel,
            Motivo = "Nivel de riesgo identificado durante el análisis.",
            MensajeMostrado = ObtenerMensaje(nivel)
        };

        await _repository.GuardarAsync(alerta, cancellationToken);

        return new EvaluarRiesgoResponse
        {
            AlertaId = alerta.Id,
            NivelRiesgo = nivel.ToString(),
            MostrarAlerta = nivel != NivelRiesgo.Bajo,
            Mensaje = alerta.MensajeMostrado,
            LineasAyuda = ObtenerLineasAyuda(nivel)
        };
    }

    private static NivelRiesgo CalcularNivel(double puntaje)
    {
        puntaje = Math.Clamp(puntaje, 0, 1);

        return puntaje switch
        {
            < 0.30 => NivelRiesgo.Bajo,
            < 0.60 => NivelRiesgo.Medio,
            < 0.80 => NivelRiesgo.Alto,
            _ => NivelRiesgo.Critico
        };
    }

    private static string ObtenerMensaje(NivelRiesgo nivel)
    {
        return nivel switch
        {
            NivelRiesgo.Bajo =>
                "Puedes continuar utilizando las herramientas de bienestar de la aplicación.",

            NivelRiesgo.Medio =>
                "Parece que estás atravesando un momento difícil. Considera hablar con una persona de confianza o un profesional.",

            NivelRiesgo.Alto =>
                "Es importante buscar apoyo profesional lo antes posible. Puedes utilizar los recursos de ayuda disponibles.",

            NivelRiesgo.Critico =>
                "Busca apoyo inmediato. Comunícate con emergencias o solicita ayuda a una persona adulta o de confianza que esté cerca.",

            _ => "Hay recursos de apoyo disponibles."
        };
    }

    private static List<LineaAyudaDto> ObtenerLineasAyuda(
        NivelRiesgo nivel)
    {
        var lineas = new List<LineaAyudaDto>
        {
            new()
            {
                Nombre = "Línea Aquí Estoy",
                Telefono = "800-2737869",
                Horario = "Lunes a viernes de 2:00 p. m. a 10:00 p. m. y sábados de 9:00 a. m. a 4:00 p. m.",
                EsEmergencia = false
            }
        };

        if (nivel == NivelRiesgo.Critico)
        {
            lineas.Insert(0, new LineaAyudaDto
            {
                Nombre = "Sistema de Emergencias",
                Telefono = "911",
                Horario = "Disponible las 24 horas",
                EsEmergencia = true
            });
        }

        return lineas;
    }
}