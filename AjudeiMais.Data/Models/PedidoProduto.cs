using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjudeiMais.Data.Models
{
    public class PedidoProduto
    {
        [Key]
        public int PedidoProduto_ID { get; set; }

        public bool Habilitado { get; set; }
        public bool Excluido { get; set; }

        [ForeignKey("Produto")]
        public int Produto_ID { get; set; }
        public Produto Produto { get; set; }

        [ForeignKey("Pedido")]
        public int Pedido_ID { get; set; }
        public Pedido Pedido { get; set; }
    }
}
