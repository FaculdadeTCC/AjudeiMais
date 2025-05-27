using System.Net.Http;
using System;
using AjudeiMais.Ecommerce.Filters;
using AjudeiMais.Ecommerce.Tools;
using Microsoft.AspNetCore.Mvc;
using AjudeiMais.Ecommerce.Models.Produto;
using System.Threading.Tasks;
using AjudeiMais.Ecommerce.Models.Usuario;
using System.Text.Json;

namespace AjudeiMais.Ecommerce.Controllers
{
    public class ProdutoController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ProdutoController> _logger;

        string BASE_URL = Assistant.ServerURL();
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
        
        public IActionResult Cadastro()
        {
            return View();
        }

        [RoleAuthorize("admin", "usuario")]
        [HttpPost]
        public async Task<IActionResult> Cadastro(ProdutoModel model, List<IFormFile> ProdutoImagens, string guid)
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

            try
            {
                var httpClient = _httpClientFactory.CreateClient("ApiAjudeiMais");

                using (var formData = new MultipartFormDataContent())
                {
                    formData.AddObjectAsFormFields(model);
                    foreach (var imagem in ProdutoImagens)
                    {
                        formData.AddFileContent(imagem, "Imagem");
                    }

                    var response = await httpClient.PostAsync($"{BASE_URL}/api/produto", formData);

                    var apiResponseContent = await response.Content.ReadAsStringAsync();
                    ApiHelper.ApiResponse<ProdutoModel> apiResponse = null;

                    try
                    {
                        // Prioriza System.Text.Json
                        apiResponse = System.Text.Json.JsonSerializer.Deserialize<ApiHelper.ApiResponse<ProdutoModel>>(
                            apiResponseContent,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                        );
                    }
                    catch (System.Text.Json.JsonException jsonEx)
                    {
                        apiResponse = new ApiHelper.ApiResponse<ProdutoModel>
                        {
                            Success = false,
                            Type = "error",
                            Message = "Ocorreu um erro no formato da resposta da API. Contacte o suporte."
                        };

                        return RedirectToRoute("usuario-cadastrar", new { alertType = apiResponse.Type, alertMessage = apiResponse.Message });

                    }

                    if (response.IsSuccessStatusCode && apiResponse != null && apiResponse.Success)
                    {
                        return RedirectToRoute("login", new { alertType = apiResponse.Type, alertMessage = apiResponse.Message });
                    }
                    else
                    {
                        string alertType = "error";
                        string alertMessage = apiResponse?.Message ?? "Não foi possível processar a requisição de cadastro. Tente novamente.";

                        return RedirectToRoute("usuario-cadastrar", new { alertType = alertType, alertMessage = alertMessage });
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                // Este catch pegará erros de rede, API fora do ar, etc.
                return RedirectToRoute("usuario-cadastrar", new { alertType = "error", alertMessage = $"Não foi possível conectar ao servidor. Tente novamente mais tarde ou entre em contato com a nossa equipe." });
            }
            catch (Exception ex)
            {
                // Erros inesperados no próprio Controller (ex: problema com IFormFile)
                return RedirectToRoute("usuario-cadastrar", new { alertType = "error", alertMessage = $"Ocorreu um erro inesperado durante o cadastro." });
            }
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
