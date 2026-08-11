using AquiEstoy.Application.Interfaces;
using AquiEstoy.Application.Services;
using AquiEstoy.Infrastructure.Data;
using AquiEstoy.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AquiEstoy.Infrastructure
{
    public static class DependencyInjection
    {
        /// <summary>
        /// Configuracion de SQL Server del proyecto. Vive en un unico sitio a proposito:
        /// las pruebas deben construir su DbContext con esta misma llamada. Un contexto
        /// de prueba sin EnableRetryOnFailure no reproduce el comportamiento real y deja
        /// pasar fallos que solo aparecen con la estrategia de reintentos activa
        /// (por ejemplo, transacciones manuales fuera de un ExecutionStrategy).
        /// </summary>
        public static DbContextOptionsBuilder UseAquiEstoySqlServer(
            this DbContextOptionsBuilder options,
            string? connectionString)
        {
            return options.UseSqlServer(
                connectionString,
                sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null));
        }

        /// <summary>
        /// Sobrecarga tipada, para quien construye las opciones con
        /// DbContextOptionsBuilder&lt;TContext&gt; (las pruebas) y necesita conservar el tipo.
        /// </summary>
        public static DbContextOptionsBuilder<TContext> UseAquiEstoySqlServer<TContext>(
            this DbContextOptionsBuilder<TContext> options,
            string? connectionString)
            where TContext : DbContext
        {
            UseAquiEstoySqlServer((DbContextOptionsBuilder)options, connectionString);
            return options;
        }

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Registra el DbContext para Entity Framework
            services.AddDbContext<AquiEstoyDbContext>(options =>
                options.UseAquiEstoySqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Mapea la interfaz de la capa Application hacia la implementación concreta en Infrastructure
            services.AddScoped<IAquiEstoyDbContext>(provider => provider.GetRequiredService<AquiEstoyDbContext>());

            // Hashing de contraseñas: la abstracción vive en Application, BCrypt aquí
            services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();

            // Registra los servicios de lógica de negocio (Application)
            services.AddScoped<ICasoService, CasoService>();
            services.AddScoped<IRegistroPacienteService, RegistroPacienteService>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<ILineaAyudaService, LineaAyudaService>();
            services.AddScoped<IConversacionService, ConversacionService>();
            services.AddScoped<ICatalogoService, CatalogoService>();
            services.AddScoped<IAlertaRiesgoService, AlertaRiesgoService>();
            services.AddScoped<IEstadisticaService, EstadisticaService>();

            return services;
        }

        /// <summary>
        /// Aplica las migraciones pendientes a la base de datos.
        /// </summary>
        public static async Task MigrateDatabaseAsync(this IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AquiEstoyDbContext>();
            await context.Database.MigrateAsync();
        }

        /// <summary>
        /// Aplica el seed de catálogos. Pensado para llamarse solo en Development.
        /// Es idempotente, así que repetirlo en cada arranque no duplica datos.
        /// </summary>
        public static async Task SeedDevelopmentDataAsync(this IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<AquiEstoyDbContext>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

            await DbInitializer.SeedAsync(context, passwordHasher);
        }
    }
}
