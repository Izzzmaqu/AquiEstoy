using AquiEstoy.Application.DTOs.AlertasRiesgo;
using AquiEstoy.Application.DTOs.LineasAyuda;
using AquiEstoy.Application.Interfaces;
using AquiEstoy.Domain.Entities;
using AquiEstoy.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AquiEstoy.Application.Services;

public class AlertaRiesgoService : IAlertaRiesgoService
{
    /// <summary>
    /// Nombre del TipoAlerta sembrado por DbInitializer para cada nivel calculado.
    /// Bajo no se usa para persistir (ver EvaluarAsync) pero se deja aquí para
    /// mantener el enum completo y evitar un catálogo desalineado.
    /// </summary>
    public static readonly IReadOnlyDictionary<NivelRiesgo, string> NombresTiposAlertaPorNivel =
        new Dictionary<NivelRiesgo, string>
        {
            [NivelRiesgo.Bajo] = "Riesgo Bajo",
            [NivelRiesgo.Medio] = "Riesgo Medio",
            [NivelRiesgo.Alto] = "Riesgo Alto",
            [NivelRiesgo.Critico] = "Riesgo Crítico"
        };

    private readonly IAquiEstoyDbContext _context;
    private readonly ILineaAyudaService _lineaAyudaService;

    public AlertaRiesgoService(IAquiEstoyDbContext context, ILineaAyudaService lineaAyudaService)
    {
        _context = context;
        _lineaAyudaService = lineaAyudaService;
    }

    public async Task<EvaluarRiesgoResponse> EvaluarAsync(
        EvaluarRiesgoRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!await _context.Casos.AnyAsync(c => c.Id == request.CasoId, cancellationToken))
        {
            throw new ArgumentException($"No existe un Caso con Id {request.CasoId}.");
        }

        if (string.IsNullOrWhiteSpace(request.Mensaje))
        {
            throw new ArgumentException("El mensaje es obligatorio.");
        }

        NivelRiesgo nivel = CalcularNivel(request.PuntajeRiesgo);
        string mensajeMostrado = ObtenerMensaje(nivel);

        int? alertaId = null;

        if (nivel != NivelRiesgo.Bajo)
        {
            var nombreTipoAlerta = NombresTiposAlertaPorNivel[nivel];

            var tipoAlerta = await _context.TiposAlerta
                .FirstOrDefaultAsync(t => t.Nombre == nombreTipoAlerta, cancellationToken);

            if (tipoAlerta == null)
            {
                throw new InvalidOperationException(
                    $"No existe un TipoAlerta con Nombre '{nombreTipoAlerta}'. Verifique el seed de TiposAlerta.");
            }

            var alerta = new Alerta
            {
                CasoId = request.CasoId,
                TipoAlertaId = tipoAlerta.Id,
                Descripcion = mensajeMostrado,
                Atendida = false
            };

            _context.Alertas.Add(alerta);
            await _context.SaveChangesAsync(cancellationToken);

            alertaId = alerta.Id;
        }

        return new EvaluarRiesgoResponse
        {
            AlertaId = alertaId,
            NivelRiesgo = nivel.ToString(),
            MostrarAlerta = nivel != NivelRiesgo.Bajo,
            Mensaje = mensajeMostrado,
            LineasAyuda = await ObtenerLineasAyudaAsync(nivel)
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

    private async Task<List<LineaAyudaDto>> ObtenerLineasAyudaAsync(NivelRiesgo nivel)
    {
        var lineas = (await _lineaAyudaService.ListarActivasAsync()).ToList();

        if (nivel == NivelRiesgo.Critico)
        {
            var indiceEmergencias = lineas.FindIndex(l => l.Telefono == "911");
            if (indiceEmergencias > 0)
            {
                var emergencias = lineas[indiceEmergencias];
                lineas.RemoveAt(indiceEmergencias);
                lineas.Insert(0, emergencias);
            }
        }

        return lineas;
    }
}
