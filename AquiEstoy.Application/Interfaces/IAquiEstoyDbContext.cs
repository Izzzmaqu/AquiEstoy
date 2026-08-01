using AquiEstoy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace AquiEstoy.Application.Interfaces;

public interface IAquiEstoyDbContext
{
    DbSet<Caso> Casos { get; }
    DbSet<Mensaje> Mensajes { get; }
    DbSet<Usuario> Usuarios { get; }
    DbSet<Profesional> Profesionales { get; }
    DbSet<NivelSeveridad> NivelesSeveridad { get; }
    DbSet<EstadoCaso> EstadosCaso { get; }
    DbSet<HistorialSeveridad> HistorialesSeveridad { get; }
    DbSet<FactorRiesgo> FactoresRiesgo { get; }
    DbSet<CasoFactorRiesgo> CasoFactoresRiesgo { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}