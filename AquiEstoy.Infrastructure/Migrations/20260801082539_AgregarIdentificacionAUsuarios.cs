using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AquiEstoy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarIdentificacionAUsuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Identificacion",
                table: "Usuarios",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Identificacion",
                table: "Usuarios");
        }
    }
}
