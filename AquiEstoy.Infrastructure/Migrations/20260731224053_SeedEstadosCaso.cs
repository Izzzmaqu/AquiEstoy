using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AquiEstoy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedEstadosCaso : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "EstadosCaso",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { 1, "Abierto" },
                    { 2, "En seguimiento" },
                    { 3, "Derivado" },
                    { 4, "Cerrado" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EstadosCaso",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "EstadosCaso",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "EstadosCaso",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "EstadosCaso",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
