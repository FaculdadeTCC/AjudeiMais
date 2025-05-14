using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AjudeiMais.Data.Migrations
{
    /// <inheritdoc />
    public partial class AtualizaçãoTabelaInstituição : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Endereco_Instituicao_Instituicao_ID",
                table: "Endereco");

            migrationBuilder.DropColumn(
                name: "Instituicao",
                table: "InstituicaoImagem");

            migrationBuilder.DropColumn(
                name: "Imagem",
                table: "Instituicao");

            migrationBuilder.AlterColumn<int>(
                name: "Instituicao_ID",
                table: "Endereco",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InstituicaoImagem_Instituicao_ID",
                table: "InstituicaoImagem",
                column: "Instituicao_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Endereco_Instituicao_Instituicao_ID",
                table: "Endereco",
                column: "Instituicao_ID",
                principalTable: "Instituicao",
                principalColumn: "Instituicao_ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InstituicaoImagem_Instituicao_Instituicao_ID",
                table: "InstituicaoImagem",
                column: "Instituicao_ID",
                principalTable: "Instituicao",
                principalColumn: "Instituicao_ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Endereco_Instituicao_Instituicao_ID",
                table: "Endereco");

            migrationBuilder.DropForeignKey(
                name: "FK_InstituicaoImagem_Instituicao_Instituicao_ID",
                table: "InstituicaoImagem");

            migrationBuilder.DropIndex(
                name: "IX_InstituicaoImagem_Instituicao_ID",
                table: "InstituicaoImagem");

            migrationBuilder.AddColumn<bool>(
                name: "Instituicao",
                table: "InstituicaoImagem",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Imagem",
                table: "Instituicao",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "Instituicao_ID",
                table: "Endereco",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Endereco_Instituicao_Instituicao_ID",
                table: "Endereco",
                column: "Instituicao_ID",
                principalTable: "Instituicao",
                principalColumn: "Instituicao_ID");
        }
    }
}
