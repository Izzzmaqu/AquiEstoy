using AquiEstoy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Reflection.Emit;

namespace AquiEstoy.Infrastructure.Data
{
    public class AquiEstoyDbContext : DbContext
    {
        public AquiEstoyDbContext(DbContextOptions<AquiEstoyDbContext> options)
            : base(options)
        {
        }

        // Catalogos
        public DbSet<EstadoCaso> EstadosCaso => Set<EstadoCaso>();
        public DbSet<Provincia> Provincias => Set<Provincia>();
        public DbSet<Canton> Cantones => Set<Canton>();
        public DbSet<Rol> Roles => Set<Rol>();
        public DbSet<Especialidad> Especialidades => Set<Especialidad>();
        public DbSet<NivelSeveridad> NivelesSeveridad => Set<NivelSeveridad>();
        public DbSet<TipoAlerta> TiposAlerta => Set<TipoAlerta>();
        public DbSet<CategoriaFactor> CategoriasFactor => Set<CategoriaFactor>();
        public DbSet<LineaAyuda> LineasAyuda => Set<LineaAyuda>();

        // Entidades principales
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Profesional> Profesionales => Set<Profesional>();
        public DbSet<Caso> Casos => Set<Caso>();
        public DbSet<AdjuntoCaso> AdjuntosCaso => Set<AdjuntoCaso>();
        public DbSet<Alerta> Alertas => Set<Alerta>();
        public DbSet<Auditoria> Auditorias => Set<Auditoria>();
        public DbSet<FactorRiesgo> FactoresRiesgo => Set<FactorRiesgo>();
        public DbSet<CasoFactorRiesgo> CasoFactoresRiesgo => Set<CasoFactorRiesgo>();
        public DbSet<Conversacion> Conversaciones => Set<Conversacion>();
        public DbSet<Mensaje> Mensajes => Set<Mensaje>();
        public DbSet<HistorialSeveridad> HistorialesSeveridad => Set<HistorialSeveridad>();
        public DbSet<ReporteEstadistico> ReportesEstadisticos => Set<ReporteEstadistico>();
        public DbSet<SesionChat> SesionesChat => Set<SesionChat>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ============================================================
            // CATALOGOS
            // ============================================================

            modelBuilder.Entity<EstadoCaso>(e =>
            {
                e.ToTable("EstadosCaso");
                e.HasKey(x => x.Id);
                e.Property(x => x.Nombre).IsRequired().HasMaxLength(30);

                e.HasData(
                    new EstadoCaso { Id = 1, Nombre = "Abierto" },
                    new EstadoCaso { Id = 2, Nombre = "En seguimiento" },
                    new EstadoCaso { Id = 3, Nombre = "Derivado" },
                    new EstadoCaso { Id = 4, Nombre = "Cerrado" }
                );
            });

            modelBuilder.Entity<Provincia>(e =>
            {
                e.ToTable("Provincias");
                e.HasKey(x => x.Id);
                e.Property(x => x.Nombre).IsRequired().HasMaxLength(50);
            });

            modelBuilder.Entity<Canton>(e =>
            {
                e.ToTable("Cantones");
                e.HasKey(x => x.Id);
                e.Property(x => x.Nombre).IsRequired().HasMaxLength(80);

                e.HasOne(x => x.Provincia)
                    .WithMany(p => p.Cantones)
                    .HasForeignKey(x => x.ProvinciaId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Rol>(e =>
            {
                e.ToTable("Roles");
                e.HasKey(x => x.Id);
                e.Property(x => x.Nombre).IsRequired().HasMaxLength(50);
                e.Property(x => x.Descripcion).HasMaxLength(200);
            });

            modelBuilder.Entity<Especialidad>(e =>
            {
                e.ToTable("Especialidades");
                e.HasKey(x => x.Id);
                e.Property(x => x.Nombre).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<NivelSeveridad>(e =>
            {
                e.ToTable("NivelesSeveridad");
                e.HasKey(x => x.Id);
                e.Property(x => x.Nombre).IsRequired().HasMaxLength(50);
                e.Property(x => x.Color).HasMaxLength(10);
                e.Property(x => x.Descripcion).HasMaxLength(300);
            });

            modelBuilder.Entity<TipoAlerta>(e =>
            {
                e.ToTable("TiposAlerta");
                e.HasKey(x => x.Id);
                e.Property(x => x.Nombre).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<CategoriaFactor>(e =>
            {
                e.ToTable("CategoriasFactor");
                e.HasKey(x => x.Id);
                e.Property(x => x.Nombre).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<LineaAyuda>(e =>
            {
                e.ToTable("LineasAyuda");
                e.HasKey(x => x.Id);
                e.Property(x => x.Nombre).IsRequired().HasMaxLength(150);
                e.Property(x => x.Telefono).IsRequired().HasMaxLength(30);
                e.Property(x => x.Descripcion).HasMaxLength(500);
                e.Property(x => x.Activa).IsRequired().HasDefaultValue(true);
            });

            // ============================================================
            // USUARIOS Y PROFESIONALES
            // ============================================================

            modelBuilder.Entity<Usuario>(e =>
            {
                e.ToTable("Usuarios");
                e.HasKey(x => x.Id);
                e.Property(x => x.Nombre).IsRequired().HasMaxLength(100);
                e.Property(x => x.Apellidos).IsRequired().HasMaxLength(100);
                e.Property(x => x.Email).IsRequired().HasMaxLength(150);
                e.Property(x => x.PasswordHash).IsRequired().HasMaxLength(256);
                e.Property(x => x.Telefono).HasMaxLength(20);
                e.Property(x => x.FechaRegistro).IsRequired().HasDefaultValueSql("GETUTCDATE()");
                e.Property(x => x.Activo).IsRequired().HasDefaultValue(true);

                e.HasIndex(x => x.Email).IsUnique();

                e.HasOne(x => x.Rol)
                    .WithMany(r => r.Usuarios)
                    .HasForeignKey(x => x.RolId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Provincia)
                    .WithMany(p => p.Usuarios)
                    .HasForeignKey(x => x.ProvinciaId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Canton)
                    .WithMany(c => c.Usuarios)
                    .HasForeignKey(x => x.CantonId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Profesional>(e =>
            {
                e.ToTable("Profesionales");
                e.HasKey(x => x.Id);
                e.Property(x => x.NumColegiado).HasMaxLength(50);
                e.Property(x => x.Disponible).IsRequired().HasDefaultValue(true);

                e.HasIndex(x => x.UsuarioId).IsUnique();

                e.HasOne(x => x.Usuario)
                    .WithOne(u => u.Profesional)
                    .HasForeignKey<Profesional>(x => x.UsuarioId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Especialidad)
                    .WithMany(esp => esp.Profesionales)
                    .HasForeignKey(x => x.EspecialidadId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ============================================================
            // CASOS Y RELACIONADOS
            // ============================================================

            modelBuilder.Entity<Caso>(e =>
            {
                e.ToTable("Casos");
                e.HasKey(x => x.Id);
                e.Property(x => x.FechaApertura).IsRequired().HasDefaultValueSql("GETUTCDATE()");
                e.Property(x => x.Descripcion).HasColumnType("nvarchar(max)");
                e.Property(x => x.Notas).HasColumnType("nvarchar(max)");

                e.HasOne(x => x.Paciente)
                    .WithMany(u => u.CasosComoPaciente)
                    .HasForeignKey(x => x.PacienteId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Profesional)
                    .WithMany(p => p.CasosAsignados)
                    .HasForeignKey(x => x.ProfesionalId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.NivelSeveridad)
                    .WithMany(n => n.Casos)
                    .HasForeignKey(x => x.NivelSeveridadId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.EstadoCaso)
                    .WithMany(es => es.Casos)
                    .HasForeignKey(x => x.EstadoCasoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<AdjuntoCaso>(e =>
            {
                e.ToTable("AdjuntosCaso");
                e.HasKey(x => x.Id);
                e.Property(x => x.NombreArchivo).IsRequired().HasMaxLength(255);
                e.Property(x => x.RutaArchivo).IsRequired().HasMaxLength(500);
                e.Property(x => x.TipoArchivo).IsRequired().HasMaxLength(50);
                e.Property(x => x.FechaSubida).IsRequired().HasDefaultValueSql("GETUTCDATE()");

                e.HasOne(x => x.Caso)
                    .WithMany(c => c.Adjuntos)
                    .HasForeignKey(x => x.CasoId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.SubidoPor)
                    .WithMany(u => u.AdjuntosSubidos)
                    .HasForeignKey(x => x.SubidoPorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Alerta>(e =>
            {
                e.ToTable("Alertas");
                e.HasKey(x => x.Id);
                e.Property(x => x.FechaGeneracion).IsRequired().HasDefaultValueSql("GETUTCDATE()");
                e.Property(x => x.Atendida).IsRequired().HasDefaultValue(false);
                e.Property(x => x.Descripcion).HasColumnType("nvarchar(max)");

                e.HasOne(x => x.Caso)
                    .WithMany(c => c.Alertas)
                    .HasForeignKey(x => x.CasoId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.TipoAlerta)
                    .WithMany(t => t.Alertas)
                    .HasForeignKey(x => x.TipoAlertaId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.AtendidaPor)
                    .WithMany(u => u.AlertasAtendidas)
                    .HasForeignKey(x => x.AtendidaPorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Auditoria>(e =>
            {
                e.ToTable("Auditoria");
                e.HasKey(x => x.Id);
                e.Property(x => x.Entidad).IsRequired().HasMaxLength(100);
                e.Property(x => x.Accion).IsRequired().HasMaxLength(50);
                e.Property(x => x.FechaAccion).IsRequired().HasDefaultValueSql("GETUTCDATE()");
                e.Property(x => x.DetalleJson).HasColumnType("nvarchar(max)");
                // EntidadId es intencionalmente polimorfico: sin FK.
                // El log de auditoria debe sobrevivir aunque se borre la entidad auditada.

                e.HasOne(x => x.Usuario)
                    .WithMany(u => u.RegistrosAuditoria)
                    .HasForeignKey(x => x.UsuarioId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<CategoriaFactor>();

            modelBuilder.Entity<FactorRiesgo>(e =>
            {
                e.ToTable("FactoresRiesgo");
                e.HasKey(x => x.Id);
                e.Property(x => x.Nombre).IsRequired().HasMaxLength(150);
                e.Property(x => x.Descripcion).HasMaxLength(500);
                e.Property(x => x.PesoRiesgo).IsRequired().HasColumnType("decimal(5,2)").HasDefaultValue(1.00m);

                e.HasOne(x => x.Categoria)
                    .WithMany(c => c.FactoresRiesgo)
                    .HasForeignKey(x => x.CategoriaId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<CasoFactorRiesgo>(e =>
            {
                e.ToTable("CasoFactoresRiesgo");
                e.HasKey(x => x.Id);
                e.Property(x => x.FechaRegistro).IsRequired().HasDefaultValueSql("GETUTCDATE()");
                e.Property(x => x.Observacion).HasColumnType("nvarchar(max)");

                e.HasOne(x => x.Caso)
                    .WithMany(c => c.FactoresRiesgo)
                    .HasForeignKey(x => x.CasoId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.FactorRiesgo)
                    .WithMany(f => f.Casos)
                    .HasForeignKey(x => x.FactorRiesgoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ============================================================
            // CHAT SEGURO
            // ============================================================

            modelBuilder.Entity<Conversacion>(e =>
            {
                e.ToTable("Conversaciones");
                e.HasKey(x => x.Id);
                e.Property(x => x.FechaInicio).IsRequired().HasDefaultValueSql("GETUTCDATE()");
                e.Property(x => x.Activa).IsRequired().HasDefaultValue(true);

                e.HasOne(x => x.Caso)
                    .WithMany(c => c.Conversaciones)
                    .HasForeignKey(x => x.CasoId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Paciente)
                    .WithMany(u => u.ConversacionesComoPaciente)
                    .HasForeignKey(x => x.PacienteId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Profesional)
                    .WithMany(p => p.Conversaciones)
                    .HasForeignKey(x => x.ProfesionalId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Mensaje>(e =>
            {
                e.ToTable("Mensajes");
                e.HasKey(x => x.Id);
                e.Property(x => x.Contenido).IsRequired().HasColumnType("nvarchar(max)");
                e.Property(x => x.FechaEnvio).IsRequired().HasDefaultValueSql("GETUTCDATE()");
                e.Property(x => x.Leido).IsRequired().HasDefaultValue(false);

                e.HasOne(x => x.Conversacion)
                    .WithMany(c => c.Mensajes)
                    .HasForeignKey(x => x.ConversacionId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Remitente)
                    .WithMany(u => u.MensajesEnviados)
                    .HasForeignKey(x => x.RemitenteId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<SesionChat>(e =>
            {
                e.ToTable("SesionesChat");
                e.HasKey(x => x.Id);
                e.Property(x => x.ConnectionId).IsRequired().HasMaxLength(150);
                e.Property(x => x.FechaConexion).IsRequired().HasDefaultValueSql("GETUTCDATE()");

                e.HasOne(x => x.Usuario)
                    .WithMany(u => u.SesionesChat)
                    .HasForeignKey(x => x.UsuarioId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ============================================================
            // HISTORIAL Y REPORTES
            // ============================================================

            modelBuilder.Entity<HistorialSeveridad>(e =>
            {
                e.ToTable("HistorialSeveridad");
                e.HasKey(x => x.Id);
                e.Property(x => x.FechaCambio).IsRequired().HasDefaultValueSql("GETUTCDATE()");
                e.Property(x => x.Motivo).HasColumnType("nvarchar(max)");

                e.HasOne(x => x.Caso)
                    .WithMany(c => c.HistorialSeveridad)
                    .HasForeignKey(x => x.CasoId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.SeveridadAnterior)
                    .WithMany(n => n.HistorialesComoAnterior)
                    .HasForeignKey(x => x.SeveridadAnterId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.SeveridadNueva)
                    .WithMany(n => n.HistorialesComoNuevo)
                    .HasForeignKey(x => x.SeveridadNuevaId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.CambiadoPor)
                    .WithMany(u => u.CambiosSeveridadRealizados)
                    .HasForeignKey(x => x.CambiadoPorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ReporteEstadistico>(e =>
            {
                e.ToTable("ReportesEstadisticos");
                e.HasKey(x => x.Id);
                e.Property(x => x.FechaGeneracion).IsRequired().HasDefaultValueSql("GETUTCDATE()");
                e.Property(x => x.TipoReporte).IsRequired().HasMaxLength(100);
                e.Property(x => x.ParametrosFiltro).HasMaxLength(500);
                e.Property(x => x.ResultadoResumen).HasColumnType("nvarchar(max)");

                e.HasOne(x => x.GeneradoPor)
                    .WithMany(u => u.ReportesGenerados)
                    .HasForeignKey(x => x.GeneradoPorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}