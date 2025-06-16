using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AjudeiMais.Data.Migrations
{
    /// <inheritdoc />
    public partial class adicionandocamposdestatusseparadosparausuarioeinstituicaonatabelapedido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StatusInstituicao",
                table: "Pedido",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StatusUsuario",
                table: "Pedido",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusInstituicao",
                table: "Pedido");

            migrationBuilder.DropColumn(
                name: "StatusUsuario",
                table: "Pedido");
        }
    }
}
