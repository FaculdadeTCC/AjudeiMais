using Microsoft.AspNetCore.Mvc;

namespace AjudeiMais.Ecommerce.Controllers
{
    public class AnuncioController : Controller
    {
        public IActionResult Detalhe()
        {
            return View();
        } 
        
        public IActionResult Adicionar()
        {
            return View();
        }

        public IActionResult Imagens()
        {
            return View();
        }

        public IActionResult Editar()
        {
            return View();
        }
    }
}
