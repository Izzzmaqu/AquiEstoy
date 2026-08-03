using AquiEstoy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
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
    DbSet<Conversacion> Conversaciones { get; }
    DbSet<Rol> Roles { get; }
    DbSet<LineaAyuda> LineasAyuda { get; }
    DbSet<Alerta> Alertas { get; }
    DbSet<TipoAlerta> TiposAlerta { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Abre una transaccion explicita. Necesario para el registro de pacientes,
    /// que debe crear Usuario + Caso + Conversacion de forma atomica.
    /// </summary>
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}
