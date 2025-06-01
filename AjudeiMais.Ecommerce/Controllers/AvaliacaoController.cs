using AjudeiMais.Ecommerce.Filters;
using AjudeiMais.Ecommerce.Models.Avaliacao;
using Microsoft.AspNetCore.Mvc;

namespace AjudeiMais.Ecommerce.Controllers
{
    public class AvaliacaoController : Controller
    {
        public IActionResult Avaliar()
        {
            return View();
        }

        [HttpPost]
        [RoleAuthorize("admin", "instituicao")]
        public IActionResult AvaliarUsuario(AvaliacaoModel model)
        {
            return View();
        }
        
        [HttpPost]
        [RoleAuthorize("admin", "instituicao")]
        public IActionResult AvaliarInstituicao(AvaliacaoModel model)
        {
            return View();
        }

        [RoleAuthorize("admin")]
        public IActionResult Update()
        {
            return View();
        }
        
        public IActionResult Excluir()
        {
            return View();
        }
    }
}
