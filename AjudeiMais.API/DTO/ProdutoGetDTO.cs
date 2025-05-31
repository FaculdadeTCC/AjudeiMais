using AjudeiMais.Data.Models.ProdutoModel;
using AjudeiMais.Data.Models.UsuarioModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AjudeiMais.API.DTO
{
    public class ProdutoGetDTO
    {
        public int Produto_ID { get; set; }
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
        public string UnidadeMedida { get; set; }

        public ICollection<ProdutoImagem> ProdutoImagens { get; set; } = new List<ProdutoImagem>();
        public UsuarioResumoDTO Usuario { get; set; }
        public CategoriaProdutoDTO CategoriaProduto { get; set; }

    }
}
