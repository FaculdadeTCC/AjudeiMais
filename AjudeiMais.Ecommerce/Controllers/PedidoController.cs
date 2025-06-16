using AjudeiMais.Ecommerce.Filters;
using AjudeiMais.Ecommerce.Models.Instituicao;
using AjudeiMais.Ecommerce.Models.Pedido;
using AjudeiMais.Ecommerce.Tools;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using AjudeiMais.Ecommerce.Models.Produto;
using static AjudeiMais.Ecommerce.Tools.ApiHelper;
using System.Net.Http.Headers;


namespace AjudeiMais.Ecommerce.Controllers
{
    public class PedidoController : Controller
    {

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<PedidoController> _logger;
        string BASE_URL = Assistant.ServerURL();

        public PedidoController(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, ILogger<PedidoController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
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
                var (apiResponse, errorMsg) = await ApiHelper.ListAllPedidosAtivosInstiuicaoAsync(_httpClientFactory, loggedInUserGuid);

                if (apiResponse != null)
                {
                    pedidos = apiResponse;
                }
                else
                {
                    errorMessage = errorMsg ?? "Erro desconhecido ao carregar pedidos.";
                    return RedirectToRoute("usuario-perfil", new
                    {
                        alertType = "error",
                        alertMessage = errorMessage,
                        guid = loggedInUserGuid
                    });
                }
            }
            catch (Exception)
            {
                errorMessage = "Ocorreu um erro inesperado ao carregar os pedidos. Tente novamente mais tarde.";
                return RedirectToRoute("usuario-perfil", new
                {
                    alertType = "error",
                    alertMessage = errorMessage,
                    guid = loggedInUserGuid
                });
            }

            // Passar a role do usuário para a view
            string userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            ViewBag.Role = userRole;

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

        [HttpPost]
        [RoleAuthorize("instituicao", "admin", "usuario")]
        public async Task<IActionResult> CriarPedido(PedidoModel pedido)
        {
            string loggedInUserGuid = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            var unauthorizedResult = ControllerHelpers.HandleUnauthorizedAccess(this, _logger, out loggedInUserGuid);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

            pedido.Instituicao_GUID = loggedInUserGuid;
            var (apiResponse, erro) = await ApiHelper.CriarPedidoAsync(pedido, _httpClientFactory, _httpContextAccessor);

            if (!string.IsNullOrEmpty(erro))
            {
                return RedirectToRoute("produto-detalhe", new
                {
                    alertType = "error",
                    alertMessage = erro,
                    id = pedido.Produto_ID
                });
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [RoleAuthorize("instituicao", "admin", "usuario")]
        public async Task<IActionResult> AtualizarStatusInstituicao(PedidoModel pedido)
        {
            string loggedInUserGuid = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var unauthorizedResult = ControllerHelpers.HandleUnauthorizedAccess(this, _logger, out loggedInUserGuid);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

            pedido.Instituicao_GUID = loggedInUserGuid;
            //pedido.Usuario_GUID = 
            var (apiResponse, erro) = await ApiHelper.CriarPedidoAsync(pedido, _httpClientFactory, _httpContextAccessor);

            if (!string.IsNullOrEmpty(erro))
            {
                return RedirectToRoute("produto-detalhe", new
                {
                    alertType = "error",
                    alertMessage = erro,
                    id = pedido.Produto_ID
                });
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        [RoleAuthorize("instituicao", "admin")]
        public async Task<IActionResult> PedidosPorInstituicao()
        {
            string loggedInUserGuid = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var unauthorizedResult = ControllerHelpers.HandleUnauthorizedAccess(this, _logger, out loggedInUserGuid);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

            try
            {
                var (apiResponse, errorMsg) = await ApiHelper.GetPedidosPorInstituicaoGUIDAsync(loggedInUserGuid, _httpClientFactory, _httpContextAccessor);

                if (apiResponse != null && apiResponse.Success)
                {
                    return View("Index", apiResponse.Data); 
                }
                else
                {
                    return RedirectToRoute("instituicao-perfil", new
                    {
                        alertType = "error",
                        alertMessage = errorMsg ?? "Erro ao carregar pedidos da instituição.",
                        guid = loggedInUserGuid
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedidos da instituição");

                return RedirectToRoute("instituicao-perfil", new
                {
                    alertType = "error",
                    alertMessage = "Erro inesperado. Tente novamente mais tarde.",
                    guid = loggedInUserGuid
                });
            }
        }
        [HttpGet]
        [RoleAuthorize("usuario", "admin")]
        public async Task<IActionResult> PedidosPorUsuario()
        {
            string loggedInUserGuid = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var unauthorizedResult = ControllerHelpers.HandleUnauthorizedAccess(this, _logger, out loggedInUserGuid);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

            try
            {
                var (apiResponse, errorMsg) = await ApiHelper.GetPedidosPorUsuarioGUIDAsync(loggedInUserGuid, _httpClientFactory, _httpContextAccessor);

                if (apiResponse != null && apiResponse.Success)
                {
                    return View("Index", apiResponse.Data);
                }
                else
                {
                    return RedirectToRoute("instituicao-perfil", new
                    {
                        alertType = "error",
                        alertMessage = errorMsg ?? "Erro ao carregar pedidos da instituição.",
                        guid = loggedInUserGuid
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedidos da instituição");

                return RedirectToRoute("instituicao-perfil", new
                {
                    alertType = "error",
                    alertMessage = "Erro inesperado. Tente novamente mais tarde.",
                    guid = loggedInUserGuid
                });
            }
        }

    }
}
