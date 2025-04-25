using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjudeiMais.Data.Models
{
    public class Produto
    {
        [Key]
        public int Produto_ID { get; set; }

        public ICollection<ProdutoImagem> ProdutoImagens { get; set; }

        public int Usuario_ID { get; set; }
        public Usuario Usuario { get; set; }

        public int Categoria_ID { get; set; }
        public Categoria Categoria { get; set; }
    }
}
