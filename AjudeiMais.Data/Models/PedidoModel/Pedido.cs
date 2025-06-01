using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AjudeiMais.Data.Models.InstituicaoModel;
using AjudeiMais.Data.Models.PedidoProdutoModel;
using AjudeiMais.Data.Models.ProdutoModel;
using AjudeiMais.Data.Models.UsuarioModel;

namespace AjudeiMais.Data.Models.PedidoModel
{
	public class Pedido
	{
		[Key]
		public int Pedido_ID { get; set; }

		public bool Habilitado { get; set; }
		public bool Excluido { get; set; }
		public string Status { get; set; }
		public string GUID { get; set; }
		public DateTime DataCriacao { get; set; }
		public DateTime DataUpdate { get; set; }

		// FK para Instituição (assumindo que você já tem uma classe Instituicao)
		public int Instituicao_ID { get; set; }
		public Instituicao Instituicao { get; set; }

		// FK para Usuario
		public int Usuario_ID { get; set; }
		public Usuario Usuario { get; set; }

		// FK para Produto
		public int Produto_ID { get; set; }
		public Produto Produto { get; set; }
	}
}
