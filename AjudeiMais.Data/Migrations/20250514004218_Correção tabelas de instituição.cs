using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AjudeiMais.Data.Migrations
{
    /// <inheritdoc />
    public partial class Correçãotabelasdeinstituição : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InstituicaoCategoria_Categoria_Categoria_ID1",
                table: "InstituicaoCategoria");

            migrationBuilder.AlterColumn<int>(
                name: "Categoria_ID1",
                table: "InstituicaoCategoria",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_InstituicaoCategoria_Categoria_Categoria_ID1",
                table: "InstituicaoCategoria",
                column: "Categoria_ID1",
                principalTable: "Categoria",
                principalColumn: "Categoria_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InstituicaoCategoria_Categoria_Categoria_ID1",
                table: "InstituicaoCategoria");

            migrationBuilder.AlterColumn<int>(
                name: "Categoria_ID1",
                table: "InstituicaoCategoria",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InstituicaoCategoria_Categoria_Categoria_ID1",
                table: "InstituicaoCategoria",
                column: "Categoria_ID1",
                principalTable: "Categoria",
                principalColumn: "Categoria_ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
