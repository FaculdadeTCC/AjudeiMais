using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AjudeiMais.Data.Models.PedidoModel;
using AjudeiMais.Data.Models.ProdutoModel;

namespace AjudeiMais.Data.Models.PedidoProdutoModel
{
    public class PedidoProduto
    {
		[Key]
		public int PedidoProduto_ID { get; set; }

		public int Pedido_ID { get; set; }
		public Pedido Pedido { get; set; }

		public int Produto_ID { get; set; }
		public Produto Produto { get; set; }

		public int Quantidade { get; set; }
	}
}
