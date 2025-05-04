using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjudeiMais.Data.Models.ProdutoModel
{
    public class CategoriaProduto
    {
        [Key]
        public int CategoriaProduto_ID { get; set; }
        public string Nome { get; set; }
        public string Icone { get; set; }
        public bool Habilitado { get; set; }
        public bool Excluido { get; set; }
    }
}
