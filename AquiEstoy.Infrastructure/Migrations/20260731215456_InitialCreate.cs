using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AquiEstoy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoriasFactor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriasFactor", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Especialidades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Especialidades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EstadosCaso",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosCaso", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LineasAyuda",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Activa = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineasAyuda", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NivelesSeveridad",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Color = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NivelesSeveridad", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Provincias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provincias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposAlerta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposAlerta", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FactoresRiesgo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoriaId = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PesoRiesgo = table.Column<decimal>(type: "decimal(5,2)", nullable: false, defaultValue: 1.00m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FactoresRiesgo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FactoresRiesgo_CategoriasFactor_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "CategoriasFactor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cantones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProvinciaId = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cantones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cantones_Provincias_ProvinciaId",
                        column: x => x.ProvinciaId,
                        principalTable: "Provincias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Apellidos = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FechaNacimiento = table.Column<DateOnly>(type: "date", nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    RolId = table.Column<int>(type: "int", nullable: false),
                    ProvinciaId = table.Column<int>(type: "int", nullable: true),
                    CantonId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuarios_Cantones_CantonId",
                        column: x => x.CantonId,
                        principalTable: "Cantones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Usuarios_Provincias_ProvinciaId",
                        column: x => x.ProvinciaId,
                        principalTable: "Provincias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Usuarios_Roles_RolId",
                        column: x => x.RolId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Auditoria",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Entidad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntidadId = table.Column<int>(type: "int", nullable: false),
                    Accion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FechaAccion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    DetalleJson = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auditoria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Auditoria_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Profesionales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    EspecialidadId = table.Column<int>(type: "int", nullable: false),
                    NumColegiado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Disponible = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profesionales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Profesionales_Especialidades_EspecialidadId",
                        column: x => x.EspecialidadId,
                        principalTable: "Especialidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Profesionales_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReportesEstadisticos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GeneradoPorId = table.Column<int>(type: "int", nullable: false),
                    FechaGeneracion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    TipoReporte = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ParametrosFiltro = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ResultadoResumen = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportesEstadisticos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportesEstadisticos_Usuarios_GeneradoPorId",
                        column: x => x.GeneradoPorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SesionesChat",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    ConnectionId = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    FechaConexion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    FechaDesconexion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SesionesChat", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SesionesChat_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Casos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PacienteId = table.Column<int>(type: "int", nullable: false),
                    ProfesionalId = table.Column<int>(type: "int", nullable: true),
                    NivelSeveridadId = table.Column<int>(type: "int", nullable: false),
                    EstadoCasoId = table.Column<int>(type: "int", nullable: false),
                    FechaApertura = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    FechaCierre = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notas = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Casos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Casos_EstadosCaso_EstadoCasoId",
                        column: x => x.EstadoCasoId,
                        principalTable: "EstadosCaso",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Casos_NivelesSeveridad_NivelSeveridadId",
                        column: x => x.NivelSeveridadId,
                        principalTable: "NivelesSeveridad",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Casos_Profesionales_ProfesionalId",
                        column: x => x.ProfesionalId,
                        principalTable: "Profesionales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Casos_Usuarios_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AdjuntosCaso",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CasoId = table.Column<int>(type: "int", nullable: false),
                    SubidoPorId = table.Column<int>(type: "int", nullable: false),
                    NombreArchivo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    RutaArchivo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TipoArchivo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FechaSubida = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdjuntosCaso", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdjuntosCaso_Casos_CasoId",
                        column: x => x.CasoId,
                        principalTable: "Casos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AdjuntosCaso_Usuarios_SubidoPorId",
                        column: x => x.SubidoPorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Alertas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CasoId = table.Column<int>(type: "int", nullable: false),
                    TipoAlertaId = table.Column<int>(type: "int", nullable: false),
                    FechaGeneracion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Atendida = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    FechaAtencion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AtendidaPorId = table.Column<int>(type: "int", nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alertas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alertas_Casos_CasoId",
                        column: x => x.CasoId,
                        principalTable: "Casos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Alertas_TiposAlerta_TipoAlertaId",
                        column: x => x.TipoAlertaId,
                        principalTable: "TiposAlerta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Alertas_Usuarios_AtendidaPorId",
                        column: x => x.AtendidaPorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CasoFactoresRiesgo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CasoId = table.Column<int>(type: "int", nullable: false),
                    FactorRiesgoId = table.Column<int>(type: "int", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Observacion = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CasoFactoresRiesgo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CasoFactoresRiesgo_Casos_CasoId",
                        column: x => x.CasoId,
                        principalTable: "Casos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CasoFactoresRiesgo_FactoresRiesgo_FactorRiesgoId",
                        column: x => x.FactorRiesgoId,
                        principalTable: "FactoresRiesgo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Conversaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CasoId = table.Column<int>(type: "int", nullable: false),
                    PacienteId = table.Column<int>(type: "int", nullable: false),
                    ProfesionalId = table.Column<int>(type: "int", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Activa = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Conversaciones_Casos_CasoId",
                        column: x => x.CasoId,
                        principalTable: "Casos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Conversaciones_Profesionales_ProfesionalId",
                        column: x => x.ProfesionalId,
                        principalTable: "Profesionales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Conversaciones_Usuarios_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HistorialSeveridad",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CasoId = table.Column<int>(type: "int", nullable: false),
                    SeveridadAnterId = table.Column<int>(type: "int", nullable: true),
                    SeveridadNuevaId = table.Column<int>(type: "int", nullable: false),
                    CambiadoPorId = table.Column<int>(type: "int", nullable: false),
                    FechaCambio = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Motivo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialSeveridad", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialSeveridad_Casos_CasoId",
                        column: x => x.CasoId,
                        principalTable: "Casos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistorialSeveridad_NivelesSeveridad_SeveridadAnterId",
                        column: x => x.SeveridadAnterId,
                        principalTable: "NivelesSeveridad",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistorialSeveridad_NivelesSeveridad_SeveridadNuevaId",
                        column: x => x.SeveridadNuevaId,
                        principalTable: "NivelesSeveridad",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistorialSeveridad_Usuarios_CambiadoPorId",
                        column: x => x.CambiadoPorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Mensajes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConversacionId = table.Column<int>(type: "int", nullable: false),
                    RemitenteId = table.Column<int>(type: "int", nullable: false),
                    Contenido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaEnvio = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Leido = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    FechaLectura = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mensajes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mensajes_Conversaciones_ConversacionId",
                        column: x => x.ConversacionId,
                        principalTable: "Conversaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Mensajes_Usuarios_RemitenteId",
                        column: x => x.RemitenteId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdjuntosCaso_CasoId",
                table: "AdjuntosCaso",
                column: "CasoId");

            migrationBuilder.CreateIndex(
                name: "IX_AdjuntosCaso_SubidoPorId",
                table: "AdjuntosCaso",
                column: "SubidoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_Alertas_AtendidaPorId",
                table: "Alertas",
                column: "AtendidaPorId");

            migrationBuilder.CreateIndex(
                name: "IX_Alertas_CasoId",
                table: "Alertas",
                column: "CasoId");

            migrationBuilder.CreateIndex(
                name: "IX_Alertas_TipoAlertaId",
                table: "Alertas",
                column: "TipoAlertaId");

            migrationBuilder.CreateIndex(
                name: "IX_Auditoria_UsuarioId",
                table: "Auditoria",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Cantones_ProvinciaId",
                table: "Cantones",
                column: "ProvinciaId");

            migrationBuilder.CreateIndex(
                name: "IX_CasoFactoresRiesgo_CasoId",
                table: "CasoFactoresRiesgo",
                column: "CasoId");

            migrationBuilder.CreateIndex(
                name: "IX_CasoFactoresRiesgo_FactorRiesgoId",
                table: "CasoFactoresRiesgo",
                column: "FactorRiesgoId");

            migrationBuilder.CreateIndex(
                name: "IX_Casos_EstadoCasoId",
                table: "Casos",
                column: "EstadoCasoId");

            migrationBuilder.CreateIndex(
                name: "IX_Casos_NivelSeveridadId",
                table: "Casos",
                column: "NivelSeveridadId");

            migrationBuilder.CreateIndex(
                name: "IX_Casos_PacienteId",
                table: "Casos",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Casos_ProfesionalId",
                table: "Casos",
                column: "ProfesionalId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversaciones_CasoId",
                table: "Conversaciones",
                column: "CasoId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversaciones_PacienteId",
                table: "Conversaciones",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversaciones_ProfesionalId",
                table: "Conversaciones",
                column: "ProfesionalId");

            migrationBuilder.CreateIndex(
                name: "IX_FactoresRiesgo_CategoriaId",
                table: "FactoresRiesgo",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialSeveridad_CambiadoPorId",
                table: "HistorialSeveridad",
                column: "CambiadoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialSeveridad_CasoId",
                table: "HistorialSeveridad",
                column: "CasoId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialSeveridad_SeveridadAnterId",
                table: "HistorialSeveridad",
                column: "SeveridadAnterId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialSeveridad_SeveridadNuevaId",
                table: "HistorialSeveridad",
                column: "SeveridadNuevaId");

            migrationBuilder.CreateIndex(
                name: "IX_Mensajes_ConversacionId",
                table: "Mensajes",
                column: "ConversacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Mensajes_RemitenteId",
                table: "Mensajes",
                column: "RemitenteId");

            migrationBuilder.CreateIndex(
                name: "IX_Profesionales_EspecialidadId",
                table: "Profesionales",
                column: "EspecialidadId");

            migrationBuilder.CreateIndex(
                name: "IX_Profesionales_UsuarioId",
                table: "Profesionales",
                column: "UsuarioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReportesEstadisticos_GeneradoPorId",
                table: "ReportesEstadisticos",
                column: "GeneradoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_SesionesChat_UsuarioId",
                table: "SesionesChat",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_CantonId",
                table: "Usuarios",
                column: "CantonId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_ProvinciaId",
                table: "Usuarios",
                column: "ProvinciaId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_RolId",
                table: "Usuarios",
                column: "RolId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdjuntosCaso");

            migrationBuilder.DropTable(
                name: "Alertas");

            migrationBuilder.DropTable(
                name: "Auditoria");

            migrationBuilder.DropTable(
                name: "CasoFactoresRiesgo");

            migrationBuilder.DropTable(
                name: "HistorialSeveridad");

            migrationBuilder.DropTable(
                name: "LineasAyuda");

            migrationBuilder.DropTable(
                name: "Mensajes");

            migrationBuilder.DropTable(
                name: "ReportesEstadisticos");

            migrationBuilder.DropTable(
                name: "SesionesChat");

            migrationBuilder.DropTable(
                name: "TiposAlerta");

            migrationBuilder.DropTable(
                name: "FactoresRiesgo");

            migrationBuilder.DropTable(
                name: "Conversaciones");

            migrationBuilder.DropTable(
                name: "CategoriasFactor");

            migrationBuilder.DropTable(
                name: "Casos");

            migrationBuilder.DropTable(
                name: "EstadosCaso");

            migrationBuilder.DropTable(
                name: "NivelesSeveridad");

            migrationBuilder.DropTable(
                name: "Profesionales");

            migrationBuilder.DropTable(
                name: "Especialidades");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Cantones");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Provincias");
        }
    }
}
