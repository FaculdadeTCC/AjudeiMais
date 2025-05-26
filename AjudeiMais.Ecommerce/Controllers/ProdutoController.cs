using AjudeiMais.Ecommerce.Filters;
using Microsoft.AspNetCore.Mvc;

namespace AjudeiMais.Ecommerce.Controllers
{
    public class ProdutoController : Controller
    {
        [RoleAuthorize("admin")]

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detalhe()
        {
            return View();
        }

        [RoleAuthorize("admin", "usuario")]
        public IActionResult Cadastro()
        {
            return View();
        }

        [RoleAuthorize("admin", "usuario")]

        public IActionResult Imagens()
        {
            return View();
        }

        [RoleAuthorize("admin", "usuario")]

        public IActionResult Editar()
        {
            return View();
        }
    }
}
