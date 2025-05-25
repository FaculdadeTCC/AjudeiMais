using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AjudeiMais.Data.Migrations
{
    /// <inheritdoc />
    public partial class Correçãotabeladeenderecoeinstituicao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Imagem",
                table: "InstituicaoImagem",
                newName: "CaminhoImagem");

            migrationBuilder.RenameColumn(
                name: "documento",
                table: "Instituicao",
                newName: "Documento");

            migrationBuilder.RenameColumn(
                name: "Guid",
                table: "Instituicao",
                newName: "GUID");

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Endereco",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Endereco");

            migrationBuilder.RenameColumn(
                name: "CaminhoImagem",
                table: "InstituicaoImagem",
                newName: "Imagem");

            migrationBuilder.RenameColumn(
                name: "GUID",
                table: "Instituicao",
                newName: "Guid");

            migrationBuilder.RenameColumn(
                name: "Documento",
                table: "Instituicao",
                newName: "documento");
        }
    }
}
