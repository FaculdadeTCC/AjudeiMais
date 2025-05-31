using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AjudeiMais.Data.Models.ProdutoModel
{
    public class ProdutoImagem
    {
        [Key]
        public int ProdutoImagem_ID { get; set; }

        public string Imagem { get; set; }
        public bool Habilitado { get; set; }
        public bool Excluido { get; set; }
        public bool IsPrincipal { get; set; }

        [ForeignKey("Produto")]
        public int Produto_ID { get; set; }
        [JsonIgnore]
        public Produto? Produto { get; set; }
    }
}
