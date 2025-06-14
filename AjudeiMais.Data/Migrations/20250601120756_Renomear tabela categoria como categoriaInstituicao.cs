using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AjudeiMais.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenomeartabelacategoriacomocategoriaInstituicao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_InstituicaoCategoria_Categoria_Categoria_ID1",
            //    table: "InstituicaoCategoria");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_Pedido_Instituicao_Instituicao_ID",
            //    table: "Pedido");

            //migrationBuilder.DropTable(
            //    name: "Categoria");

            //migrationBuilder.DropIndex(
            //    name: "IX_Pedido_Instituicao_ID",
            //    table: "Pedido");

            //migrationBuilder.RenameColumn(
            //    name: "NumeroPedido",
            //    table: "Pedido",
            //    newName: "GUID");

            //migrationBuilder.AlterColumn<int>(
            //    name: "Usuario_ID",
            //    table: "Pedido",
            //    type: "int",
            //    nullable: false,
            //    oldClrType: typeof(Guid),
            //    oldType: "uniqueidentifier");

            //migrationBuilder.AddColumn<int>(
            //    name: "Instituicao_ID1",
            //    table: "Pedido",
            //    type: "int",
            //    nullable: false,
            //    defaultValue: 0);

            //migrationBuilder.AddColumn<int>(
            //    name: "Produto_ID",
            //    table: "Pedido",
            //    type: "int",
            //    nullable: false,
            //    defaultValue: 0);

            //migrationBuilder.AddColumn<int>(
            //    name: "Produto_ID1",
            //    table: "Pedido",
            //    type: "int",
            //    nullable: false,
            //    defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CategoriaInstituicao",
                columns: table => new
                {
                    Categoria_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Habilitado = table.Column<bool>(type: "bit", nullable: false),
                    Excluido = table.Column<bool>(type: "bit", nullable: false),
                    Icone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GUID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataUpdate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriaInstituicao", x => x.Categoria_ID);
                });

            //migrationBuilder.CreateIndex(
            //    name: "IX_Pedido_Instituicao_ID1",
            //    table: "Pedido",
            //    column: "Instituicao_ID1");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Pedido_Produto_ID1",
            //    table: "Pedido",
            //    column: "Produto_ID1");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_InstituicaoCategoria_CategoriaInstituicao_Categoria_ID1",
            //    table: "InstituicaoCategoria",
            //    column: "Categoria_ID1",
            //    principalTable: "CategoriaInstituicao",
            //    principalColumn: "Categoria_ID");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Pedido_Instituicao_Instituicao_ID1",
            //    table: "Pedido",
            //    column: "Instituicao_ID1",
            //    principalTable: "Instituicao",
            //    principalColumn: "Instituicao_ID",
            //    onDelete: ReferentialAction.Cascade);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Pedido_Produto_Produto_ID1",
            //    table: "Pedido",
            //    column: "Produto_ID1",
            //    principalTable: "Produto",
            //    principalColumn: "Produto_ID",
            //    onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InstituicaoCategoria_CategoriaInstituicao_Categoria_ID1",
                table: "InstituicaoCategoria");

            migrationBuilder.DropForeignKey(
                name: "FK_Pedido_Instituicao_Instituicao_ID1",
                table: "Pedido");

            migrationBuilder.DropForeignKey(
                name: "FK_Pedido_Produto_Produto_ID1",
                table: "Pedido");

            migrationBuilder.DropTable(
                name: "CategoriaInstituicao");

            migrationBuilder.DropIndex(
                name: "IX_Pedido_Instituicao_ID1",
                table: "Pedido");

            migrationBuilder.DropIndex(
                name: "IX_Pedido_Produto_ID1",
                table: "Pedido");

            migrationBuilder.DropColumn(
                name: "Instituicao_ID1",
                table: "Pedido");

            migrationBuilder.DropColumn(
                name: "Produto_ID",
                table: "Pedido");

            migrationBuilder.DropColumn(
                name: "Produto_ID1",
                table: "Pedido");

            migrationBuilder.RenameColumn(
                name: "GUID",
                table: "Pedido",
                newName: "NumeroPedido");

            migrationBuilder.AlterColumn<Guid>(
                name: "Usuario_ID",
                table: "Pedido",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "Categoria",
                columns: table => new
                {
                    Categoria_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataUpdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Excluido = table.Column<bool>(type: "bit", nullable: false),
                    GUID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Habilitado = table.Column<bool>(type: "bit", nullable: false),
                    Icone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categoria", x => x.Categoria_ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_Instituicao_ID",
                table: "Pedido",
                column: "Instituicao_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_InstituicaoCategoria_Categoria_Categoria_ID1",
                table: "InstituicaoCategoria",
                column: "Categoria_ID1",
                principalTable: "Categoria",
                principalColumn: "Categoria_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Pedido_Instituicao_Instituicao_ID",
                table: "Pedido",
                column: "Instituicao_ID",
                principalTable: "Instituicao",
                principalColumn: "Instituicao_ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
