using AjudeiMais.Ecommerce.Filters;
using Microsoft.AspNetCore.Mvc;

namespace AjudeiMais.Ecommerce.Controllers
{
    public class InstituicaoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [RoleAuthorize("admin", "instituicao")]
        public IActionResult Perfil()
        {
            return View();
        }

        //[RoleAuthorize("admin", "instituicao")]
        public IActionResult Cadastro()
        {
            return View();
        }

        [RoleAuthorize("admin", "instituicao")]
        public IActionResult AlterarDados()
        {
            return View();
        }
    }
}
