using AjudeiMais.Ecommerce.Filters;
using Microsoft.AspNetCore.Mvc;

namespace AjudeiMais.Ecommerce.Controllers
{
    public class AdminController : Controller
    {
        [RoleAuthorize("admin")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
