using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AjudeiMais.Data.Models.UsuarioModel;

namespace AjudeiMais.Data.Models.ProdutoModel
{
    public class Produto
    {
        [Key]
        public int Produto_ID { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string? Condicao { get; set; }
        public string? Validade { get; set; }
        public int Quantidade { get; set; }
        public decimal? Peso { get; set; }
        public bool Disponivel { get; set; }
        public bool Habilitado { get; set; }
        public bool Excluido { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataUpdate { get; set; }

        public ICollection<ProdutoImagem> ProdutoImagens { get; set; }

        [ForeignKey("Usuario")]
        public int Usuario_ID { get; set; }
        public Usuario Usuario { get; set; }

        [ForeignKey("CategoriaProduto")]
        public int CategoriaProduto_ID { get; set; }
        public CategoriaProduto CategoriaProduto { get; set; }
    }
}
