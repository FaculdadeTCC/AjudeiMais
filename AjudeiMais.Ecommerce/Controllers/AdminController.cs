using Microsoft.AspNetCore.Mvc;

namespace AjudeiMais.Ecommerce.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
