using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AjudeiMais.Data.Migrations
{
	/// <inheritdoc />
	public partial class AtualizandoatabelapedidocomProdutoIDeGUID : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "Pedido",
				columns: table => new
				{
					Pedido_ID = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					Habilitado = table.Column<bool>(type: "bit", nullable: false),
					Excluido = table.Column<bool>(type: "bit", nullable: false),
					Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
					GUID = table.Column<string>(type: "nvarchar(max)", nullable: false),
					DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
					DataUpdate = table.Column<DateTime>(type: "datetime2", nullable: false),
					UsuarioContato = table.Column<string>(type: "nvarchar(max)", nullable: false),
					InstituicaoContato = table.Column<string>(type: "nvarchar(max)", nullable: false),
					UsuarioEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
					InstituicaoEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
					Instituicao_ID = table.Column<int>(type: "int", nullable: false),
					Usuario_ID = table.Column<int>(type: "int", nullable: false),
					Produto_ID = table.Column<int>(type: "int", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Pedido", x => x.Pedido_ID);

					table.ForeignKey(
						name: "FK_Pedido_Instituicao_Instituicao_ID",
						column: x => x.Instituicao_ID,
						principalTable: "Instituicao",
						principalColumn: "Instituicao_ID",
						onDelete: ReferentialAction.Cascade);

					table.ForeignKey(
						name: "FK_Pedido_Produto_Produto_ID",
						column: x => x.Produto_ID,
						principalTable: "Produto",
						principalColumn: "Produto_ID",
						onDelete: ReferentialAction.Cascade);

					// ALTERADO AQUI: evitar ciclo de cascata
					table.ForeignKey(
						name: "FK_Pedido_Usuario_Usuario_ID",
						column: x => x.Usuario_ID,
						principalTable: "Usuario",
						principalColumn: "Usuario_ID",
						onDelete: ReferentialAction.NoAction);
				});
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "Pedido");
		}
	}
}
