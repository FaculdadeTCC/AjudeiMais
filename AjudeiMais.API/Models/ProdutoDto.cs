using AjudeiMais.Data.Models.ProdutoModel;
using AjudeiMais.Data.Models.UsuarioModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace AjudeiMais.API.Models
{
    public class ProdutoDto
    {
        public int Produto_ID { get; set; }
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
        public DateTime? DataUpdate { get; set; }
        public List<IFormFile> Imagens { get; set; } = new List<IFormFile>();
        public int Usuario_ID { get; set; }
        public int CategoriaProduto_ID { get; set; }
    }
}
