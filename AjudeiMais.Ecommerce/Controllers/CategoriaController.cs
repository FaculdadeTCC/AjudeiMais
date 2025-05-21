using AjudeiMais.Ecommerce.Filters;
using Microsoft.AspNetCore.Mvc;

namespace AjudeiMais.Ecommerce.Controllers
{
    public class CategoriaController : Controller
    {
        [RoleAuthorize("admin")]
        public IActionResult Index()
        {
            return View();
        }

        [RoleAuthorize("admin")]
        public IActionResult AlterarDados()
        {
            return View();
        }

        [RoleAuthorize("admin")]
        public IActionResult Adicionar()
        {
            return View();
        }

    }
}
