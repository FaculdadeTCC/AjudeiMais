using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AjudeiMais.Ecommerce.Models.Usuario;

namespace AjudeiMais.Ecommerce.Models.Produto
{
    public class ProdutoModel
    {
        [Key]
        public int? Produto_ID { get; set; }
        public string Guid { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string? Condicao { get; set; }
        public DateTime? Validade { get; set; }
        public int Quantidade { get; set; }
        public decimal? Peso { get; set; }
        public bool Disponivel { get; set; }
        public bool Habilitado { get; set; }
        public bool Excluido { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataUpdate { get; set; }

        public List<IFormFile> ProdutoImagens { get; set; }

        [ForeignKey("Usuario")]
        public int Usuario_ID { get; set; }
        public UsuarioPerfilModel? Usuario { get; set; }

        [ForeignKey("CategoriaProduto")]
        public int CategoriaProduto_ID { get; set; }
        //public CategoriaProduto? CategoriaProduto { get; set; }
    }
}
