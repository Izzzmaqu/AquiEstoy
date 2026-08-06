using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AquiEstoy.Web.Models.Api;

namespace AquiEstoy.Web.Services
{
    /// <summary>Resultado de una llamada a la API: datos o mensaje de error ya legible.</summary>
    public record ApiResult<T>(bool Success, T? Data, string? ErrorMessage)
    {
        public static ApiResult<T> Ok(T data) => new(true, data, null);
        public static ApiResult<T> Fail(string mensaje) => new(false, default, mensaje);
    }

    /// <summary>
    /// Unico punto por el que la capa Web habla con AquiEstoy.API.
    /// Centraliza la deserializacion y la traduccion de errores para que los
    /// controllers no manejen HttpClient directamente.
    /// </summary>
    public class AquiEstoyApiClient
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _http;
        private readonly ILogger<AquiEstoyApiClient> _logger;

        public AquiEstoyApiClient(HttpClient http, ILogger<AquiEstoyApiClient> logger)
        {
            _http = http;
            _logger = logger;
        }

        // ---------- Usuarios ----------

        public Task<ApiResult<PacienteRegistradoResponse>> RegistrarPacienteAsync(RegistrarPacienteRequest request) =>
            PostAsync<RegistrarPacienteRequest, PacienteRegistradoResponse>("api/usuarios/registrar-paciente", request);

        public async Task<UsuarioSesionResponse?> LoginAsync(LoginRequest request)
        {
            try
            {
                var respuesta = await _http.PostAsJsonAsync("api/usuarios/login", request);

                if (respuesta.StatusCode == HttpStatusCode.Unauthorized)
                    return null;

                respuesta.EnsureSuccessStatusCode();

                return await respuesta.Content.ReadFromJsonAsync<UsuarioSesionResponse>(JsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fallo el login contra la API");
                return null;
            }
        }

        public Task<IReadOnlyList<PacienteListItem>> ListarPacientesAsync() =>
            GetListAsync<PacienteListItem>("api/usuarios/pacientes");

        // ---------- Casos ----------

        public Task<IReadOnlyList<CasoListItem>> ListarCasosAsync(int? pacienteId = null)
        {
            var url = pacienteId.HasValue
                ? $"api/casos?pacienteId={pacienteId.Value}"
                : "api/casos";

            return GetListAsync<CasoListItem>(url);
        }

        public Task<CasoDetalle?> ObtenerCasoAsync(int id) =>
            GetAsync<CasoDetalle>($"api/casos/{id}");

        public async Task<ApiResult<CasoListItem>> EditarCasoAsync(int id, EditarCasoRequest request)
        {
            try
            {
                var respuesta = await _http.PutAsJsonAsync($"api/casos/{id}", request);

                if (!respuesta.IsSuccessStatusCode)
                    return ApiResult<CasoListItem>.Fail(await LeerMensajeDeErrorAsync(respuesta));

                var datos = await respuesta.Content.ReadFromJsonAsync<CasoListItem>(JsonOptions);

                return datos == null
                    ? ApiResult<CasoListItem>.Fail("La API devolvió una respuesta vacía.")
                    : ApiResult<CasoListItem>.Ok(datos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fallo al editar el caso {CasoId}", id);
                return ApiResult<CasoListItem>.Fail("No se pudo contactar con el servicio.");
            }
        }

        // ---------- Conversaciones ----------

        public Task<IReadOnlyList<MensajeItem>> ListarMensajesAsync(int conversacionId) =>
            GetListAsync<MensajeItem>($"api/conversaciones/{conversacionId}/mensajes");

        // ---------- Catalogos ----------

        public Task<IReadOnlyList<LineaAyudaItem>> ListarLineasAyudaAsync() =>
            GetListAsync<LineaAyudaItem>("api/lineas-ayuda");

        public Task<DashboardResumen?> ObtenerResumenAsync() =>
            GetAsync<DashboardResumen>("api/dashboard/resumen");

        public Task<PanelEstadistico?> ObtenerPanelEstadisticoAsync() =>
            GetAsync<PanelEstadistico>("api/estadisticas/panel");

        public Task<IReadOnlyList<EstadoCasoItem>> ListarEstadosCasoAsync() =>
            GetListAsync<EstadoCasoItem>("api/catalogos/estados-caso");

        public Task<IReadOnlyList<NivelSeveridadItem>> ListarNivelesSeveridadAsync() =>
            GetListAsync<NivelSeveridadItem>("api/catalogos/niveles-severidad");

        // ---------- Helpers ----------

        private async Task<T?> GetAsync<T>(string url) where T : class
        {
            try
            {
                var respuesta = await _http.GetAsync(url);

                if (!respuesta.IsSuccessStatusCode)
                {
                    _logger.LogWarning("GET {Url} respondió {Status}", url, respuesta.StatusCode);
                    return null;
                }

                return await respuesta.Content.ReadFromJsonAsync<T>(JsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fallo GET {Url}", url);
                return null;
            }
        }

        private async Task<IReadOnlyList<T>> GetListAsync<T>(string url)
        {
            var datos = await GetAsync<List<T>>(url);
            return datos ?? new List<T>();
        }

        private async Task<ApiResult<TResponse>> PostAsync<TRequest, TResponse>(string url, TRequest request)
        {
            try
            {
                var respuesta = await _http.PostAsJsonAsync(url, request);

                if (!respuesta.IsSuccessStatusCode)
                    return ApiResult<TResponse>.Fail(await LeerMensajeDeErrorAsync(respuesta));

                var datos = await respuesta.Content.ReadFromJsonAsync<TResponse>(JsonOptions);

                return datos == null
                    ? ApiResult<TResponse>.Fail("La API devolvió una respuesta vacía.")
                    : ApiResult<TResponse>.Ok(datos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fallo POST {Url}", url);
                return ApiResult<TResponse>.Fail("No se pudo contactar con el servicio.");
            }
        }

        /// <summary>
        /// La API devuelve { "message": "..." } en BadRequest y 503, y
        /// ValidationProblemDetails cuando falla el ModelState de [ApiController].
        /// </summary>
        private static async Task<string> LeerMensajeDeErrorAsync(HttpResponseMessage respuesta)
        {
            var cuerpo = await respuesta.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(cuerpo))
                return $"La API respondió {(int)respuesta.StatusCode}.";

            try
            {
                using var documento = JsonDocument.Parse(cuerpo);
                var raiz = documento.RootElement;

                if (raiz.TryGetProperty("message", out var mensaje))
                    return mensaje.GetString() ?? cuerpo;

                // ValidationProblemDetails: se aplanan los errores de todos los campos.
                if (raiz.TryGetProperty("errors", out var errores))
                {
                    var detalles = errores.EnumerateObject()
                        .SelectMany(campo => campo.Value.EnumerateArray().Select(e => e.GetString()))
                        .Where(m => !string.IsNullOrWhiteSpace(m))
                        .ToList();

                    if (detalles.Count > 0)
                        return string.Join(" ", detalles);
                }

                if (raiz.TryGetProperty("title", out var titulo))
                    return titulo.GetString() ?? cuerpo;
            }
            catch (JsonException)
            {
                // Cuerpo no-JSON: se devuelve tal cual.
            }

            return cuerpo;
        }
    }
}
