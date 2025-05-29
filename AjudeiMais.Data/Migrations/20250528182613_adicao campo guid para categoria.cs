using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AjudeiMais.Data.Migrations
{
    /// <inheritdoc />
    public partial class adicaocampoguidparacategoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GUID",
                table: "Categoria",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GUID",
                table: "Categoria");
        }
    }
}
