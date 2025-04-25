using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjudeiMais.Data.Models
{
    public class ProdutoImagem
    {
        [Key]
        public int ProdutoImagem_ID { get; set; }
        public string Imagem { get; set; }

        [ForeignKey("Produto")]
        public int Produto_ID { get; set; }
        public Produto Produto { get; set; }
    }
}
