using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AjudeiMais.Data.Migrations
{
    /// <inheritdoc />
    public partial class CriaçãodatabelaAvaliacao2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Avaliacao",
                columns: table => new
                {
                    Avaliacao_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nota = table.Column<int>(type: "int", nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataAvaliacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Usuario_ID = table.Column<int>(type: "int", nullable: false),
                    Instituicao_ID = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Avaliacao", x => x.Avaliacao_ID);
                    table.ForeignKey(
                        name: "FK_Avaliacao_Instituicao_Instituicao_ID",
                        column: x => x.Instituicao_ID,
                        principalTable: "Instituicao",
                        principalColumn: "Instituicao_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Avaliacao_Usuario_Usuario_ID",
                        column: x => x.Usuario_ID,
                        principalTable: "Usuario",
                        principalColumn: "Usuario_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Avaliacao_Instituicao_ID",
                table: "Avaliacao",
                column: "Instituicao_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Avaliacao_Usuario_ID",
                table: "Avaliacao",
                column: "Usuario_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Avaliacao");
        }
    }
}
