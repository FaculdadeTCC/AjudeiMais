using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AjudeiMais.Data.Migrations
{
    /// <inheritdoc />
    public partial class SecondaryMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categoria",
                columns: table => new
                {
                    Categoria_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Habilitado = table.Column<bool>(type: "bit", nullable: false),
                    Excluido = table.Column<bool>(type: "bit", nullable: false),
                    Icone = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categoria", x => x.Categoria_ID);
                });

            migrationBuilder.CreateTable(
                name: "CategoriaProduto",
                columns: table => new
                {
                    CategoriaProduto_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Icone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Habilitado = table.Column<bool>(type: "bit", nullable: false),
                    Excluido = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriaProduto", x => x.CategoriaProduto_ID);
                });

            migrationBuilder.CreateTable(
                name: "Instituicao",
                columns: table => new
                {
                    Instituicao_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Imagem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Senha = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Guid = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Avaliacao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Habilitado = table.Column<bool>(type: "bit", nullable: false),
                    Excluido = table.Column<bool>(type: "bit", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataUpdate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instituicao", x => x.Instituicao_ID);
                });

            migrationBuilder.CreateTable(
                name: "InstituicaoImagem",
                columns: table => new
                {
                    InsituicaoImagem_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Imagem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Habilitado = table.Column<bool>(type: "bit", nullable: false),
                    Excluido = table.Column<bool>(type: "bit", nullable: false),
                    Instituicao_ID = table.Column<int>(type: "int", nullable: false),
                    Instituicao = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstituicaoImagem", x => x.InsituicaoImagem_ID);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    Usuario_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeCompleto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Documento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Senha = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GUID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CEP = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rua = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Numero = table.Column<int>(type: "int", nullable: false),
                    Complemento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bairro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cidade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Habilitado = table.Column<bool>(type: "bit", nullable: false),
                    Excluido = table.Column<bool>(type: "bit", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataUpdate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.Usuario_ID);
                });

            migrationBuilder.CreateTable(
                name: "Endereco",
                columns: table => new
                {
                    Endereco_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CEP = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rua = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Numero = table.Column<int>(type: "int", nullable: false),
                    Complemento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bairro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cidade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Habilitado = table.Column<bool>(type: "bit", nullable: false),
                    Excluido = table.Column<bool>(type: "bit", nullable: false),
                    Instituicao_ID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Endereco", x => x.Endereco_ID);
                    table.ForeignKey(
                        name: "FK_Endereco_Instituicao_Instituicao_ID",
                        column: x => x.Instituicao_ID,
                        principalTable: "Instituicao",
                        principalColumn: "Instituicao_ID");
                });

            migrationBuilder.CreateTable(
                name: "InstituicaoCategoria",
                columns: table => new
                {
                    InstituicaoCategoria_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Instituicao_ID = table.Column<int>(type: "int", nullable: false),
                    Categoria_ID = table.Column<int>(type: "int", nullable: false),
                    Categoria_ID1 = table.Column<int>(type: "int", nullable: false),
                    Instituicao_ID1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstituicaoCategoria", x => x.InstituicaoCategoria_ID);
                    table.ForeignKey(
                        name: "FK_InstituicaoCategoria_Categoria_Categoria_ID1",
                        column: x => x.Categoria_ID1,
                        principalTable: "Categoria",
                        principalColumn: "Categoria_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InstituicaoCategoria_Instituicao_Instituicao_ID",
                        column: x => x.Instituicao_ID,
                        principalTable: "Instituicao",
                        principalColumn: "Instituicao_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InstituicaoCategoria_Instituicao_Instituicao_ID1",
                        column: x => x.Instituicao_ID1,
                        principalTable: "Instituicao",
                        principalColumn: "Instituicao_ID");
                });

            migrationBuilder.CreateTable(
                name: "Pedido",
                columns: table => new
                {
                    Pedido_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroPedido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Habilitado = table.Column<bool>(type: "bit", nullable: false),
                    Excluido = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataUpdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Instituicao_ID = table.Column<int>(type: "int", nullable: false)
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
                });

            migrationBuilder.CreateTable(
                name: "Chat",
                columns: table => new
                {
                    Chat_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataAbertura = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Habilitado = table.Column<bool>(type: "bit", nullable: false),
                    Excluido = table.Column<bool>(type: "bit", nullable: false),
                    GUID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Usuario_ID = table.Column<int>(type: "int", nullable: false),
                    Instituicao_ID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chat", x => x.Chat_ID);
                    table.ForeignKey(
                        name: "FK_Chat_Instituicao_Instituicao_ID",
                        column: x => x.Instituicao_ID,
                        principalTable: "Instituicao",
                        principalColumn: "Instituicao_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Chat_Usuario_Usuario_ID",
                        column: x => x.Usuario_ID,
                        principalTable: "Usuario",
                        principalColumn: "Usuario_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Produto",
                columns: table => new
                {
                    Produto_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Condicao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Validade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    Peso = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Disponivel = table.Column<bool>(type: "bit", nullable: false),
                    Habilitado = table.Column<bool>(type: "bit", nullable: false),
                    Excluido = table.Column<bool>(type: "bit", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataUpdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Usuario_ID = table.Column<int>(type: "int", nullable: false),
                    CategoriaProduto_ID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produto", x => x.Produto_ID);
                    table.ForeignKey(
                        name: "FK_Produto_CategoriaProduto_CategoriaProduto_ID",
                        column: x => x.CategoriaProduto_ID,
                        principalTable: "CategoriaProduto",
                        principalColumn: "CategoriaProduto_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Produto_Usuario_Usuario_ID",
                        column: x => x.Usuario_ID,
                        principalTable: "Usuario",
                        principalColumn: "Usuario_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MensagemChat",
                columns: table => new
                {
                    MensagemChat_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Mensagem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Habilitado = table.Column<bool>(type: "bit", nullable: false),
                    Excluido = table.Column<bool>(type: "bit", nullable: false),
                    GUID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoRemetente = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remetente_ID = table.Column<int>(type: "int", nullable: false),
                    Chat_ID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MensagemChat", x => x.MensagemChat_ID);
                    table.ForeignKey(
                        name: "FK_MensagemChat_Chat_Chat_ID",
                        column: x => x.Chat_ID,
                        principalTable: "Chat",
                        principalColumn: "Chat_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PedidoProduto",
                columns: table => new
                {
                    PedidoProduto_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Habilitado = table.Column<bool>(type: "bit", nullable: false),
                    Excluido = table.Column<bool>(type: "bit", nullable: false),
                    Produto_ID = table.Column<int>(type: "int", nullable: false),
                    Pedido_ID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidoProduto", x => x.PedidoProduto_ID);
                    table.ForeignKey(
                        name: "FK_PedidoProduto_Pedido_Pedido_ID",
                        column: x => x.Pedido_ID,
                        principalTable: "Pedido",
                        principalColumn: "Pedido_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PedidoProduto_Produto_Produto_ID",
                        column: x => x.Produto_ID,
                        principalTable: "Produto",
                        principalColumn: "Produto_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProdutoImagem",
                columns: table => new
                {
                    ProdutoImagem_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Imagem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Habilitado = table.Column<bool>(type: "bit", nullable: false),
                    Excluido = table.Column<bool>(type: "bit", nullable: false),
                    Produto_ID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProdutoImagem", x => x.ProdutoImagem_ID);
                    table.ForeignKey(
                        name: "FK_ProdutoImagem_Produto_Produto_ID",
                        column: x => x.Produto_ID,
                        principalTable: "Produto",
                        principalColumn: "Produto_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chat_Instituicao_ID",
                table: "Chat",
                column: "Instituicao_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Chat_Usuario_ID",
                table: "Chat",
                column: "Usuario_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Endereco_Instituicao_ID",
                table: "Endereco",
                column: "Instituicao_ID");

            migrationBuilder.CreateIndex(
                name: "IX_InstituicaoCategoria_Categoria_ID1",
                table: "InstituicaoCategoria",
                column: "Categoria_ID1");

            migrationBuilder.CreateIndex(
                name: "IX_InstituicaoCategoria_Instituicao_ID",
                table: "InstituicaoCategoria",
                column: "Instituicao_ID");

            migrationBuilder.CreateIndex(
                name: "IX_InstituicaoCategoria_Instituicao_ID1",
                table: "InstituicaoCategoria",
                column: "Instituicao_ID1");

            migrationBuilder.CreateIndex(
                name: "IX_MensagemChat_Chat_ID",
                table: "MensagemChat",
                column: "Chat_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_Instituicao_ID",
                table: "Pedido",
                column: "Instituicao_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoProduto_Pedido_ID",
                table: "PedidoProduto",
                column: "Pedido_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoProduto_Produto_ID",
                table: "PedidoProduto",
                column: "Produto_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Produto_CategoriaProduto_ID",
                table: "Produto",
                column: "CategoriaProduto_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Produto_Usuario_ID",
                table: "Produto",
                column: "Usuario_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ProdutoImagem_Produto_ID",
                table: "ProdutoImagem",
                column: "Produto_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Endereco");

            migrationBuilder.DropTable(
                name: "InstituicaoCategoria");

            migrationBuilder.DropTable(
                name: "InstituicaoImagem");

            migrationBuilder.DropTable(
                name: "MensagemChat");

            migrationBuilder.DropTable(
                name: "PedidoProduto");

            migrationBuilder.DropTable(
                name: "ProdutoImagem");

            migrationBuilder.DropTable(
                name: "Categoria");

            migrationBuilder.DropTable(
                name: "Chat");

            migrationBuilder.DropTable(
                name: "Pedido");

            migrationBuilder.DropTable(
                name: "Produto");

            migrationBuilder.DropTable(
                name: "Instituicao");

            migrationBuilder.DropTable(
                name: "CategoriaProduto");

            migrationBuilder.DropTable(
                name: "Usuario");
        }
    }
}
