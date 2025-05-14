using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AjudeiMais.Ecommerce.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly HttpClient _httpClient;

        public UsuarioController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public IActionResult Perfil()
        {
            return View();
        } 
        
        public IActionResult Cadastro()
        {
            return View();
        }

        public IActionResult Anuncios()
        {
            return View();
        }

        public IActionResult AlterarDados()
        {
            return View();
        }
    }
}
