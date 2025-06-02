using AjudeiMais.Ecommerce.Filters;
using AjudeiMais.Ecommerce.Models.Instituicao;
using AjudeiMais.Ecommerce.Models.Pedido;
using AjudeiMais.Ecommerce.Tools;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace AjudeiMais.Ecommerce.Controllers
{
    public class PedidoController : Controller
    {

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<PedidoController> _logger;

        string BASE_URL = Assistant.ServerURL();

        public PedidoController(IHttpClientFactory httpClientFactory, ILogger<PedidoController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }


        [RoleAuthorize("usuario", "instituicao", "admin")]
        public async Task<IActionResult> Index()
        {
            string loggedInUserGuid = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;


            var unauthorizedResult = ControllerHelpers.HandleUnauthorizedAccess(this, _logger, out loggedInUserGuid);

            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

            string errorMessage = null;
            List<GetPedidoModel> pedidos = new List<GetPedidoModel>();

            try
            {
                var (apiResponse, errorMsg) = await ApiHelper.ListAllPedidosAtivosAsync(_httpClientFactory);

                if (apiResponse != null)
                {
                    pedidos = apiResponse;
                }
                else
                {
                    errorMessage = errorMsg ?? "Erro desconhecido ao carregar pedidos.";
                    return RedirectToRoute("usuario-perfil", new { alertType = "error", alertMessage = errorMessage, guid = loggedInUserGuid });
                }
            }
            catch (Exception)
            {
                errorMessage = "Ocorreu um erro inesperado ao carregar os pedidos. Tente novamente mais tarde.";
                return RedirectToRoute("usuario-perfil", new { alertType = "error", alertMessage = errorMessage, guid = loggedInUserGuid });
            }

            return View(pedidos);
        }

        [HttpPost]
        public async Task<IActionResult> Index(PedidoModel model)
        {
            string loggedInUserGuid = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;


            var unauthorizedResult = ControllerHelpers.HandleUnauthorizedAccess(this, _logger, out loggedInUserGuid);

            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

            string errorMessage = null;
            List<GetPedidoModel> pedidos = new List<GetPedidoModel>();

            try
            {
                var (apiResponse, errorMsg) = await ApiHelper.ListAllPedidosAtivosAsync(_httpClientFactory);

                if (apiResponse != null)
                {
                    pedidos = apiResponse;
                }
                else
                {
                    errorMessage = errorMsg ?? "Erro desconhecido ao carregar pedidos.";
                    return RedirectToRoute("usuario-perfil", new { alertType = "error", alertMessage = errorMessage, guid = loggedInUserGuid });
                }
            }
            catch (Exception)
            {
                errorMessage = "Ocorreu um erro inesperado ao carregar os pedidos. Tente novamente mais tarde.";
                return RedirectToRoute("usuario-perfil", new { alertType = "error", alertMessage = errorMessage, guid = loggedInUserGuid });
            }

            return View(pedidos);
        }
        public IActionResult Detalhe()
        {
            return PartialView();
        }
    }
}
