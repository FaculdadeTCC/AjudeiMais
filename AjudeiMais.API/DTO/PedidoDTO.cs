using AjudeiMais.Data.Models.InstituicaoModel;
using AjudeiMais.Data.Models.UsuarioModel;

namespace AjudeiMais.API.DTO
{
	public class PedidoDTO
	{
		public int Pedido_ID { get; set; }

		public string Usuario_GUID { get; set; }
		public string Instituicao_GUID { get; set; }
		public int Produto_ID { get; set; }

	}

	public class GetPedidoDTO 
	{
		public int Pedido_ID {get; set; }
		public string Pedido_GUID { get; set; }
		public string Usuario_GUID { get;set; }
		public string Instituicao_GUID { get; set; }

		public string UsuarioContato { get; set; }
		public string InstituicaoContato { get; set; }

		public string UsuarioEmail { get; set; }
		public string InstituicaoEmail { get; set; }

		public ProdutoGetDTO Produto { get; set; }
	}


}
