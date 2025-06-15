using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AjudeiMais.Data.Migrations
{
    /// <inheritdoc />
    public partial class col_latitude_longitude_instituicao_REMOVE : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Instituicao");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Instituicao");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Latitude",
                table: "Instituicao",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Longitude",
                table: "Instituicao",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
