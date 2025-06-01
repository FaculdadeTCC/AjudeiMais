using AjudeiMais.Data.Models.InstituicaoModel;
using AjudeiMais.Data.Models.UsuarioModel;

namespace AjudeiMais.API.DTO
{
	public class CriarPedidoDTO
	{
		public int Pedido_ID { get; set; }

		public string Usuario_GUID { get; set; }
		public string Instituicao_GUID { get; set; }
		public int Produto_ID { get; set; }

	}
}
