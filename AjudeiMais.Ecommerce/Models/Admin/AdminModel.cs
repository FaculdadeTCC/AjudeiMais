using AjudeiMais.Ecommerce.Models.Instituicao;
using AjudeiMais.Ecommerce.Models.Usuario;

namespace AjudeiMais.Ecommerce.Models.Admin
{
    public class AdminModel
    {
        public IEnumerable<UsuarioPerfilModel> Usuarios { get; set; }
        public IEnumerable<InstituicaoModel> Instituicoes { get; set; }
    }
}
