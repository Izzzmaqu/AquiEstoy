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
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Registra el DbContext para Entity Framework
            services.AddDbContext<AquiEstoyDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null)));

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

            return services;
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
