using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AjudeiMais.Data.Models.ChatModel;
using AjudeiMais.Data.Models.InstituicaoModel;
using AjudeiMais.Data.Models.PedidoModel;
using AjudeiMais.Data.Models.PedidoProdutoModel;
using AjudeiMais.Data.Models.ProdutoModel;
using AjudeiMais.Data.Models.UsuarioModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AjudeiMais.Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
            base(options){ }

        //Usuário
        public DbSet<Usuario> Usuario { get; set; }
        //Produto
        public DbSet<Produto> Produto { get; set; }
        public DbSet<ProdutoImagem> ProdutoImagem { get; set; }
        public DbSet<CategoriaProduto> CategoriaProduto { get; set; }
        //Pedido
        public DbSet<Pedido> Pedido { get; set; }
        public DbSet<PedidoProduto> PedidoProduto { get; set; }
        //Instituição
        public DbSet<Instituicao> Instituicao { get; set; }
        public DbSet<InstituicaoImagem> InstituicaoImagem { get; set; }
        public DbSet<CategoriaInstituicao> CategoriaInstituicao { get; set; }
        public DbSet<Endereco> Endereco { get; set; }
        //Chat
        //public DbSet<MensagemChat> MensagemChat { get; set; }
        //public DbSet<Chat> Chat { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<InstituicaoCategoria>()
                .HasOne(c => c.Instituicao)
                .WithMany()
                .HasForeignKey(c => c.Instituicao_ID)
                .OnDelete(DeleteBehavior.Restrict);  // Impede a exclusão se houver registros dependentes

			modelBuilder.Entity<Pedido>()
	            .HasOne(p => p.Usuario)
	            .WithMany()
	            .HasForeignKey(p => p.Usuario_ID);

			modelBuilder.Entity<Pedido>()
				.HasOne(p => p.Instituicao)
				.WithMany()
				.HasForeignKey(p => p.Instituicao_ID);

			modelBuilder.Entity<Pedido>()
				.HasOne(p => p.Produto)
				.WithMany()
				.HasForeignKey(p => p.Produto_ID);
		}

	}
}
