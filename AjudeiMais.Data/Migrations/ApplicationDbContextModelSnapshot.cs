﻿// <auto-generated />
using System;
using AjudeiMais.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AjudeiMais.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("AjudeiMais.Data.Models.InstituicaoModel.CategoriaInstituicao", b =>
                {
                    b.Property<int>("Categoria_ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Categoria_ID"));

                    b.Property<DateTime>("DataCriacao")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DataUpdate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Excluido")
                        .HasColumnType("bit");

                    b.Property<string>("GUID")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Habilitado")
                        .HasColumnType("bit");

                    b.Property<string>("Icone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Categoria_ID");

                    b.ToTable("CategoriaInstituicao");
                });

            modelBuilder.Entity("AjudeiMais.Data.Models.InstituicaoModel.Endereco", b =>
                {
                    b.Property<int>("Endereco_ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Endereco_ID"));

                    b.Property<string>("Bairro")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CEP")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Cidade")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Complemento")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Estado")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Excluido")
                        .HasColumnType("bit");

                    b.Property<bool>("Habilitado")
                        .HasColumnType("bit");

                    b.Property<int>("Instituicao_ID")
                        .HasColumnType("int");

                    b.Property<int>("Numero")
                        .HasColumnType("int");

                    b.Property<string>("Rua")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Endereco_ID");

                    b.HasIndex("Instituicao_ID");

                    b.ToTable("Endereco");
                });

            modelBuilder.Entity("AjudeiMais.Data.Models.InstituicaoModel.Instituicao", b =>
                {
                    b.Property<int>("Instituicao_ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Instituicao_ID"));

                    b.Property<string>("Avaliacao")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DataCriacao")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DataUpdate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Descricao")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Documento")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Excluido")
                        .HasColumnType("bit");

                    b.Property<string>("FotoPerfil")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GUID")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Habilitado")
                        .HasColumnType("bit");

                    b.Property<string>("Latitude")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Longitude")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Senha")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Telefone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Instituicao_ID");

                    b.ToTable("Instituicao");
                });

            modelBuilder.Entity("AjudeiMais.Data.Models.InstituicaoModel.InstituicaoCategoria", b =>
                {
                    b.Property<int>("InstituicaoCategoria_ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("InstituicaoCategoria_ID"));

                    b.Property<int>("Categoria_ID")
                        .HasColumnType("int");

                    b.Property<int?>("Categoria_ID1")
                        .HasColumnType("int");

                    b.Property<int>("Instituicao_ID")
                        .HasColumnType("int");

                    b.Property<int?>("Instituicao_ID1")
                        .HasColumnType("int");

                    b.HasKey("InstituicaoCategoria_ID");

                    b.HasIndex("Categoria_ID1");

                    b.HasIndex("Instituicao_ID");

                    b.HasIndex("Instituicao_ID1");

                    b.ToTable("InstituicaoCategoria");
                });

            modelBuilder.Entity("AjudeiMais.Data.Models.InstituicaoModel.InstituicaoImagem", b =>
                {
                    b.Property<int>("InsituicaoImagem_ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("InsituicaoImagem_ID"));

                    b.Property<string>("CaminhoImagem")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Excluido")
                        .HasColumnType("bit");

                    b.Property<bool>("Habilitado")
                        .HasColumnType("bit");

                    b.Property<int>("Instituicao_ID")
                        .HasColumnType("int");

                    b.HasKey("InsituicaoImagem_ID");

                    b.HasIndex("Instituicao_ID");

                    b.ToTable("InstituicaoImagem");
                });

            modelBuilder.Entity("AjudeiMais.Data.Models.PedidoModel.Pedido", b =>
                {
                    b.Property<int>("Pedido_ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Pedido_ID"));

                    b.Property<DateTime>("DataCriacao")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DataUpdate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Excluido")
                        .HasColumnType("bit");

                    b.Property<string>("GUID")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Habilitado")
                        .HasColumnType("bit");

                    b.Property<string>("InstituicaoContato")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("InstituicaoEmail")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Instituicao_ID")
                        .HasColumnType("int");

                    b.Property<int>("Produto_ID")
                        .HasColumnType("int");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StatusInstituicao")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StatusUsuario")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UsuarioContato")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UsuarioEmail")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Usuario_ID")
                        .HasColumnType("int");

                    b.HasKey("Pedido_ID");

                    b.HasIndex("Instituicao_ID");

                    b.HasIndex("Produto_ID");

                    b.HasIndex("Usuario_ID");

                    b.ToTable("Pedido");
                });

            modelBuilder.Entity("AjudeiMais.Data.Models.PedidoProdutoModel.PedidoProduto", b =>
                {
                    b.Property<int>("PedidoProduto_ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PedidoProduto_ID"));

                    b.Property<int>("Pedido_ID")
                        .HasColumnType("int");

                    b.Property<int>("Pedido_ID1")
                        .HasColumnType("int");

                    b.Property<int>("Produto_ID")
                        .HasColumnType("int");

                    b.Property<int>("Produto_ID1")
                        .HasColumnType("int");

                    b.Property<int>("Quantidade")
                        .HasColumnType("int");

                    b.HasKey("PedidoProduto_ID");

                    b.HasIndex("Pedido_ID1");

                    b.HasIndex("Produto_ID1");

                    b.ToTable("PedidoProduto");
                });

            modelBuilder.Entity("AjudeiMais.Data.Models.ProdutoModel.CategoriaProduto", b =>
                {
                    b.Property<int>("CategoriaProduto_ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CategoriaProduto_ID"));

                    b.Property<DateTime>("DataCadastro")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DataUpdate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Excluido")
                        .HasColumnType("bit");

                    b.Property<bool>("Habilitado")
                        .HasColumnType("bit");

                    b.Property<string>("Icone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CategoriaProduto_ID");

                    b.ToTable("CategoriaProduto");
                });

            modelBuilder.Entity("AjudeiMais.Data.Models.ProdutoModel.Produto", b =>
                {
                    b.Property<int>("Produto_ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Produto_ID"));

                    b.Property<int>("CategoriaProduto_ID")
                        .HasColumnType("int");

                    b.Property<string>("Condicao")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DataCriacao")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DataUpdate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Descricao")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Disponivel")
                        .HasColumnType("bit");

                    b.Property<bool>("Excluido")
                        .HasColumnType("bit");

                    b.Property<string>("Guid")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Habilitado")
                        .HasColumnType("bit");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal?>("Peso")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("Quantidade")
                        .HasColumnType("int");

                    b.Property<string>("UnidadeMedida")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Usuario_ID")
                        .HasColumnType("int");

                    b.Property<DateTime?>("Validade")
                        .HasColumnType("datetime2");

                    b.HasKey("Produto_ID");

                    b.HasIndex("CategoriaProduto_ID");

                    b.HasIndex("Usuario_ID");

                    b.ToTable("Produto");
                });

            modelBuilder.Entity("AjudeiMais.Data.Models.ProdutoModel.ProdutoImagem", b =>
                {
                    b.Property<int>("ProdutoImagem_ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProdutoImagem_ID"));

                    b.Property<bool>("Excluido")
                        .HasColumnType("bit");

                    b.Property<bool>("Habilitado")
                        .HasColumnType("bit");

                    b.Property<string>("Imagem")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsPrincipal")
                        .HasColumnType("bit");

                    b.Property<int>("Produto_ID")
                        .HasColumnType("int");

                    b.HasKey("ProdutoImagem_ID");

                    b.HasIndex("Produto_ID");

                    b.ToTable("ProdutoImagem");
                });

            modelBuilder.Entity("AjudeiMais.Data.Models.UsuarioModel.Usuario", b =>
                {
                    b.Property<int>("Usuario_ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Usuario_ID"));

                    b.Property<string>("Bairro")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CEP")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Cidade")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Complemento")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DataCriacao")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DataUpdate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Documento")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Estado")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Excluido")
                        .HasColumnType("bit");

                    b.Property<string>("FotoDePerfil")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GUID")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Habilitado")
                        .HasColumnType("bit");

                    b.Property<string>("Latitude")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Longitude")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NomeCompleto")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Numero")
                        .HasColumnType("int");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Rua")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Senha")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Telefone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TelefoneFixo")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Usuario_ID");

                    b.ToTable("Usuario");
                });

            modelBuilder.Entity("AjudeiMais.Data.Models.InstituicaoModel.Endereco", b =>
                {
                    b.HasOne("AjudeiMais.Data.Models.InstituicaoModel.Instituicao", "Instituicao")
                        .WithMany("Enderecos")
                        .HasForeignKey("Instituicao_ID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Instituicao");
                });

            modelBuilder.Entity("AjudeiMais.Data.Models.InstituicaoModel.InstituicaoCategoria", b =>
                {
                    b.HasOne("AjudeiMais.Data.Models.InstituicaoModel.CategoriaInstituicao", "Categoria")
                        .WithMany()
                        .HasForeignKey("Categoria_ID1");

                    b.HasOne("AjudeiMais.Data.Models.InstituicaoModel.Instituicao", "Instituicao")
                        .WithMany()
                        .HasForeignKey("Instituicao_ID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("AjudeiMais.Data.Models.InstituicaoModel.Instituicao", null)
                        .WithMany("InstituicaoCategorias")
                        .HasForeignKey("Instituicao_ID1");

                    b.Navigation("Categoria");

                    b.Navigation("Instituicao");
                });

            modelBuilder.Entity("AjudeiMais.Data.Models.InstituicaoModel.InstituicaoImagem", b =>
                {
                    b.HasOne("AjudeiMais.Data.Models.InstituicaoModel.Instituicao", "Instituicao")
                        .WithMany("instituicaoImagems")
                        .HasForeignKey("Instituicao_ID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Instituicao");
                });

            modelBuilder.Entity("AjudeiMais.Data.Models.PedidoModel.Pedido", b =>
                {
                    b.HasOne("AjudeiMais.Data.Models.InstituicaoModel.Instituicao", "Instituicao")
                        .WithMany()
                        .HasForeignKey("Instituicao_ID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AjudeiMais.Data.Models.ProdutoModel.Produto", "Produto")
                        .WithMany()
                        .HasForeignKey("Produto_ID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AjudeiMais.Data.Models.UsuarioModel.Usuario", "Usuario")
                        .WithMany()
                        .HasForeignKey("Usuario_ID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Instituicao");

                    b.Navigation("Produto");

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("AjudeiMais.Data.Models.PedidoProdutoModel.PedidoProduto", b =>
                {
                    b.HasOne("AjudeiMais.Data.Models.PedidoModel.Pedido", "Pedido")
                        .WithMany()
                        .HasForeignKey("Pedido_ID1")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AjudeiMais.Data.Models.ProdutoModel.Produto", "Produto")
                        .WithMany()
                        .HasForeignKey("Produto_ID1")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Pedido");

                    b.Navigation("Produto");
                });

            modelBuilder.Entity("AjudeiMais.Data.Models.ProdutoModel.Produto", b =>
                {
                    b.HasOne("AjudeiMais.Data.Models.ProdutoModel.CategoriaProduto", "CategoriaProduto")
                        .WithMany()
                        .HasForeignKey("CategoriaProduto_ID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AjudeiMais.Data.Models.UsuarioModel.Usuario", "Usuario")
                        .WithMany("Produtos")
                        .HasForeignKey("Usuario_ID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CategoriaProduto");

                    b.Navigation("Usuario");
                });

            modelBuilder.Entity("AjudeiMais.Data.Models.ProdutoModel.ProdutoImagem", b =>
                {
                    b.HasOne("AjudeiMais.Data.Models.ProdutoModel.Produto", "Produto")
                        .WithMany("ProdutoImagens")
                        .HasForeignKey("Produto_ID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Produto");
                });

            modelBuilder.Entity("AjudeiMais.Data.Models.InstituicaoModel.Instituicao", b =>
                {
                    b.Navigation("Enderecos");

                    b.Navigation("InstituicaoCategorias");

                    b.Navigation("instituicaoImagems");
                });

            modelBuilder.Entity("AjudeiMais.Data.Models.ProdutoModel.Produto", b =>
                {
                    b.Navigation("ProdutoImagens");
                });

            modelBuilder.Entity("AjudeiMais.Data.Models.UsuarioModel.Usuario", b =>
                {
                    b.Navigation("Produtos");
                });
#pragma warning restore 612, 618
        }
    }
}
