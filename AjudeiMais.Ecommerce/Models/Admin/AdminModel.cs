using AjudeiMais.Ecommerce.Models.Instituicao;
using AjudeiMais.Ecommerce.Models.Usuario;

namespace AjudeiMais.Ecommerce.Models.Admin
{
    public class AdminModel
    {
        public UsuarioModel Usuarios { get; set; }
        public InstituicaoModel Instituicoes { get; set; }
    }
}
