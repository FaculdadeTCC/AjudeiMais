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
        public string? StatusUsuario { get; set; }
        public string? StatusInstituicao { get; set; }
        public string GUID { get; set; }
		public DateTime DataCriacao { get; set; }
		public DateTime	 DataUpdate { get; set; }
		public string UsuarioContato { get; set; }
		public string InstituicaoContato { get; set; }
		public string UsuarioEmail { get; set; }
		public string InstituicaoEmail { get; set; }

		public int Instituicao_ID { get; set; }
		public Instituicao Instituicao { get; set; }

        public int Usuario_ID { get; set; }
		public Usuario Usuario { get; set; }

		public int Produto_ID { get; set; }
		public Produto Produto { get; set; }



	}
}
