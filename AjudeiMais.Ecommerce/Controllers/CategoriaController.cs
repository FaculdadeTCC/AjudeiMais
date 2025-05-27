using System.Reflection;
using System.Text.Json;
using AjudeiMais.Ecommerce.Filters;
using AjudeiMais.Ecommerce.Models.CategoriaProduto;
using AjudeiMais.Ecommerce.Models.Usuario;
using AjudeiMais.Ecommerce.Tools;
using Microsoft.AspNetCore.Mvc;

namespace AjudeiMais.Ecommerce.Controllers
{
    public class CategoriaController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ProdutoController> _logger;

        string BASE_URL = Assistant.ServerURL();

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

        //[RoleAuthorize("admin")]
        public IActionResult Cadastro()
        {
            return View();
        }

        //[RoleAuthorize("admin")]
        [HttpPost]
        public async Task<IActionResult> Cadastro(CategoriaProdutoModel model, IFormFile Icone)
        {
            if (!User.Identity.IsAuthenticated)
            {
                try
                {
                    var httpClient = _httpClientFactory.CreateClient("ApiAjudeiMais");

                    using (var formData = new MultipartFormDataContent())
                    {
                        formData.AddObjectAsFormFields(model);
                        formData.AddFileContent(Icone, "Icone");

                        var response = await httpClient.PostAsync($"{Tools.Assistant.ServerURL()}api/Categoria", formData);

                        // Sempre tentaremos deserializar para ApiResponse<Usuario>
                        var apiResponseContent = await response.Content.ReadAsStringAsync();
                        ApiHelper.ApiResponse<CategoriaProdutoResponse> apiResponse = null;

                        try
                        {
                            // Prioriza System.Text.Json
                            apiResponse = System.Text.Json.JsonSerializer.Deserialize<ApiHelper.ApiResponse<CategoriaProdutoResponse>>(
                                apiResponseContent,
                                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                            );
                        }
                        catch (System.Text.Json.JsonException jsonEx)
                        {
                            apiResponse = new ApiHelper.ApiResponse<CategoriaProdutoResponse>
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
            else
            {
                return RedirectToRoute("usuario-perfil", new { guid = User.FindFirst("GUID").Value });
            }
        }

    }
}
