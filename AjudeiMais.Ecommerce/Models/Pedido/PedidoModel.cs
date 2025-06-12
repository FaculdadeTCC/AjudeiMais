using AjudeiMais.Ecommerce.Models.Instituicao;
using AjudeiMais.Ecommerce.Models.Produto;
using AjudeiMais.Ecommerce.Models.Usuario;

namespace AjudeiMais.Ecommerce.Models.Pedido
{
    public class PedidoModel
    {
        public int Pedido_ID { get; set; }

        public string Usuario_GUID { get; set; }
        public string Instituicao_GUID { get; set; }
        public string StatusUsuario {  get; set; }
        public string StatusInstituicao {  get; set; }
        public int Produto_ID { get; set; }
    }

    public class GetPedidoModel
    {
        public int Pedido_ID { get; set; }
        public string Pedido_GUID { get; set; }
        public string Status { get; set; }

        public UsuarioResumoDTO Usuario {get;set;}
        public InstituicaoPerfilModel Instituicao { get; set; }
        public ProdutoResponse Produto { get; set; }
    }
}
