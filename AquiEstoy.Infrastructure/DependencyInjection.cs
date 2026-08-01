using AquiEstoy.Application.Interfaces;
using AquiEstoy.Application.Services;
using AquiEstoy.Infrastructure.Data;
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
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Mapea la interfaz de la capa Application hacia la implementación concreta en Infrastructure
            services.AddScoped<IAquiEstoyDbContext>(provider => provider.GetRequiredService<AquiEstoyDbContext>());

            // Registra el servicio de lógica de negocio (Application)
            services.AddScoped<ICasoService, CasoService>();

            return services;
        }
    }
}