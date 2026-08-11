using Microsoft.Extensions.Configuration;

namespace AquiEstoy.Tests.E2E;

/// <summary>
/// Configuracion compartida por las pruebas E2E. La cadena de conexion se lee del
/// appsettings.json de la API en vez de duplicarse aqui.
/// </summary>
public static class TestConfig
{
    public const string WebBaseUrl = "https://localhost:7165";
    public const string ApiBaseUrl = "https://localhost:7139";

    /// <summary>Credenciales del usuario staff sembrado por DbInitializer.</summary>
    public const string StaffEmail = "profesional@aquiestoy.com";
    public const string StaffPassword = "Demo1234!";

    public static string ConnectionString
    {
        get
        {
            var raiz = LocalizarRaizDelRepo();
            var appsettings = Path.Combine(raiz, "AquiEstoy.API", "appsettings.json");

            if (!File.Exists(appsettings))
                throw new FileNotFoundException(
                    $"No se encontro el appsettings.json de la API en {appsettings}.");

            var config = new ConfigurationBuilder()
                .AddJsonFile(appsettings)
                .Build();

            return config.GetConnectionString("DefaultConnection")
                   ?? throw new InvalidOperationException(
                       "Falta ConnectionStrings:DefaultConnection en el appsettings.json de la API.");
        }
    }

    /// <summary>Sube desde bin/ hasta la carpeta que contiene los proyectos.</summary>
    private static string LocalizarRaizDelRepo()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);

        while (dir != null && !Directory.Exists(Path.Combine(dir.FullName, "AquiEstoy.API")))
            dir = dir.Parent;

        return dir?.FullName
               ?? throw new DirectoryNotFoundException(
                   "No se pudo localizar la raiz del repositorio desde " + AppContext.BaseDirectory);
    }
}
