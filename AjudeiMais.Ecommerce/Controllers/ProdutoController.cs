using System.Net.Http;
using System;
using AjudeiMais.Ecommerce.Filters;
using AjudeiMais.Ecommerce.Tools;
using Microsoft.AspNetCore.Mvc;

namespace AjudeiMais.Ecommerce.Controllers
{
    public class ProdutoController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ProdutoController> _logger;

        public ProdutoController(IHttpClientFactory httpClientFactory, ILogger<ProdutoController> logger = null)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger; // Atribui o logger, se injetado
        }

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
        public IActionResult Cadastro(string guid)
        {

            string loggedInUserGuid;

            // Primeiro, valida se o usuário está autenticado e se o GUID dele é válido
            var unauthorizedResult = ControllerHelpers.HandleUnauthorizedAccess(this, _logger, out loggedInUserGuid);

            if (unauthorizedResult != null)
            {
                return unauthorizedResult; // Redireciona para login ou home
            }

            // Em seguida, valida se o usuário tem permissão para acessar o perfil solicitado (GUID da URL)
            var profileAccessResult = ControllerHelpers.ValidateUserProfileAccess(this, guid, loggedInUserGuid);

            if (profileAccessResult != null)
            {
                return profileAccessResult; // Redireciona com mensagem de erro de permissão
            }




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
