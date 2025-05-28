using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using AjudeiMais.Ecommerce.Filters;
using AjudeiMais.Ecommerce.Models.CategoriaProduto;
using AjudeiMais.Ecommerce.Models.Usuario;
using AjudeiMais.Ecommerce.Tools;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;
using X.PagedList.Extensions;

namespace AjudeiMais.Ecommerce.Controllers
{
    public class CategoriaController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<CategoriaController> _logger;

        string BASE_URL = Assistant.ServerURL();

        public CategoriaController(IHttpClientFactory httpClientFactory, ILogger<CategoriaController> logger = null)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }


        [RoleAuthorize("admin")]
        public async Task<IActionResult> Index() 
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToRoute("usuario-perfil", new { guid = User.FindFirst("GUID").Value });
            }

            try
            {
                var httpClient = _httpClientFactory.CreateClient("ApiAjudeiMais");

                var response = await httpClient.GetAsync($"{BASE_URL}api/CategoriaProduto/ativos");

                var apiResponseContent = await response.Content.ReadAsStringAsync();
                ApiHelper.ApiResponse<List<CategoriaProdutoResponse>> apiResponse = null;

                try
                {
                    apiResponse = System.Text.Json.JsonSerializer.Deserialize<ApiHelper.ApiResponse<List<CategoriaProdutoResponse>>>(
                        apiResponseContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
                }
                catch (System.Text.Json.JsonException)
                {
                    return RedirectToRoute("admin-categorias-produto", new
                    {
                        alertType = "error",
                        alertMessage = "Ocorreu um erro no formato da resposta da API. Contacte o suporte."
                    });
                }

                if (response.IsSuccessStatusCode && apiResponse != null && apiResponse.Success)
                {
                    return View(apiResponse.Data);

                }
                else
                {
                    string alertType = apiResponse.Type;
                    string alertMessage = apiResponse?.Message ?? "Não foi possível processar a requisição de cadastro. Tente novamente.";

                    return RedirectToRoute("admin-categorias-produto", new { alertType = alertType, alertMessage = alertMessage });
                }
            }
            catch (HttpRequestException)
            {
                return RedirectToRoute("admin-categorias-produto", new { alertType = "error", alertMessage = "Não foi possível conectar ao servidor. Tente novamente mais tarde ou entre em contato com a nossa equipe." });
            }
            catch (Exception)
            {
                return RedirectToRoute("admin-categorias-produto", new { alertType = "error", alertMessage = "Ocorreu um erro inesperado durante o cadastro." });
            }
        }


        [RoleAuthorize("admin")]
        public IActionResult _CadastroCategoriaProduto()
        {
            return PartialView();
        }

        [RoleAuthorize("admin")]
        public IActionResult _DeletarCategoriaProduto()
        {
            return PartialView();
        }
        [RoleAuthorize("admin")]
        [HttpGet]
        public async Task<IActionResult> _AtualizarCategoriaProduto(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToRoute("usuario-perfil", new { guid = User.FindFirst("GUID").Value });
            }

            try
            {
                var httpClient = _httpClientFactory.CreateClient("ApiAjudeiMais");

                var response = await httpClient.GetAsync($"{BASE_URL}api/CategoriaProduto/{id}");

                var apiResponseContent = await response.Content.ReadAsStringAsync();
                ApiHelper.ApiResponse<CategoriaProdutoResponse> apiResponse = null;

                try
                {
                    apiResponse = System.Text.Json.JsonSerializer.Deserialize<ApiHelper.ApiResponse<CategoriaProdutoResponse>>(
                        apiResponseContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
                }
                catch (System.Text.Json.JsonException)
                {
                    return RedirectToRoute("admin-categorias-produto", new
                    {
                        alertType = "error",
                        alertMessage = "Ocorreu um erro no formato da resposta da API. Contacte o suporte."
                    });
                }

                if (response.IsSuccessStatusCode && apiResponse != null && apiResponse.Success)
                {
                    return PartialView(apiResponse.Data);

                }
                else
                {
                    string alertType = apiResponse.Type;
                    string alertMessage = apiResponse?.Message ?? "Não foi possível processar a requisição de cadastro. Tente novamente.";

                    return RedirectToRoute("admin-categorias-produto", new { alertType = alertType, alertMessage = alertMessage });
                }
            }
            catch (HttpRequestException)
            {
                return RedirectToRoute("admin-categorias-produto", new { alertType = "error", alertMessage = "Não foi possível conectar ao servidor. Tente novamente mais tarde ou entre em contato com a nossa equipe." });
            }
            catch (Exception)
            {
                return RedirectToRoute("admin-categorias-produto", new { alertType = "error", alertMessage = "Ocorreu um erro inesperado durante o cadastro." });
            }

        }

        [RoleAuthorize("admin")]
        [HttpPost]
        public async Task<IActionResult> Excluir(int CategoriaProduto_ID)
        {
            if (User.Identity.IsAuthenticated)
            {
                try
                {
                    var httpClient = _httpClientFactory.CreateClient("ApiAjudeiMais");

                    using (var formData = new MultipartFormDataContent())
                    {
                        var response = await httpClient.DeleteAsync($"{BASE_URL}api/CategoriaProduto/{CategoriaProduto_ID}");
                        var apiResponseContent = await response.Content.ReadAsStringAsync();
                        ApiHelper.ApiResponse<CategoriaProdutoModel> apiResponse = null;

                        try
                        {
                            // Prioriza System.Text.Json
                            apiResponse = System.Text.Json.JsonSerializer.Deserialize<ApiHelper.ApiResponse<CategoriaProdutoModel>>(
                                apiResponseContent,
                                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                            );
                        }
                        catch (System.Text.Json.JsonException jsonEx)
                        {
                            apiResponse = new ApiHelper.ApiResponse<CategoriaProdutoModel>
                            {
                                Success = false,
                                Type = "error",
                                Message = "Ocorreu um erro no formato da resposta da API. Contacte o suporte."
                            };

                            return RedirectToRoute("admin-categorias-produto", new { alertType = apiResponse.Type, alertMessage = apiResponse.Message });

                        }

                        if (response.IsSuccessStatusCode && apiResponse != null && apiResponse.Success)
                        {
                            return RedirectToRoute("admin-categorias-produto", new { alertType = apiResponse.Type, alertMessage = apiResponse.Message });
                        }
                        else
                        {
                            string alertType = apiResponse.Type;
                            string alertMessage = apiResponse?.Message ?? "Não foi possível processar a requisição de cadastro. Tente novamente.";

                            return RedirectToRoute("admin-categorias-produto", new { alertType = alertType, alertMessage = alertMessage });
                        }
                    }
                }
                catch (HttpRequestException ex)
                {
                    // Este catch pegará erros de rede, API fora do ar, etc.
                    return RedirectToRoute("admin-categorias-produto", new { alertType = "error", alertMessage = $"Não foi possível conectar ao servidor. Tente novamente mais tarde ou entre em contato com a nossa equipe." });
                }
                catch (Exception ex)
                {
                    // Erros inesperados no próprio Controller (ex: problema com IFormFile)
                    return RedirectToRoute("admin-categorias-produto", new { alertType = "error", alertMessage = $"Ocorreu um erro inesperado durante o cadastro." });
                }
            }
            else
            {
                return RedirectToRoute("usuario-perfil", new { guid = User.FindFirst("GUID").Value });
            }
        }

        [RoleAuthorize("admin")]
        [HttpPost]
        public async Task<IActionResult> Cadastro(CategoriaProdutoModel model)
        {
            if (User.Identity.IsAuthenticated)
            {
                try
                {
                    var httpClient = _httpClientFactory.CreateClient("ApiAjudeiMais");

                    using (var formData = new MultipartFormDataContent())
                    {
                        formData.AddObjectAsFormFields(model);

                        var response = await httpClient.PostAsync($"{BASE_URL}api/CategoriaProduto", formData);

                        var apiResponseContent = await response.Content.ReadAsStringAsync();
                        ApiHelper.ApiResponse<CategoriaProdutoModel> apiResponse = null;

                        try
                        {
                            // Prioriza System.Text.Json
                            apiResponse = System.Text.Json.JsonSerializer.Deserialize<ApiHelper.ApiResponse<CategoriaProdutoModel>>(
                                apiResponseContent,
                                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                            );
                        }
                        catch (System.Text.Json.JsonException jsonEx)
                        {
                            apiResponse = new ApiHelper.ApiResponse<CategoriaProdutoModel>
                            {
                                Success = false,
                                Type = "error",
                                Message = "Ocorreu um erro no formato da resposta da API. Contacte o suporte."
                            };

                            return RedirectToRoute("admin-categorias-produto", new { alertType = apiResponse.Type, alertMessage = apiResponse.Message });

                        }

                        if (response.IsSuccessStatusCode && apiResponse != null && apiResponse.Success)
                        {
                            return RedirectToRoute("admin-categorias-produto", new { alertType = apiResponse.Type, alertMessage = apiResponse.Message });
                        }
                        else
                        {
                            string alertType = apiResponse.Type;
                            string alertMessage = apiResponse?.Message ?? "Não foi possível processar a requisição de cadastro. Tente novamente.";

                            return RedirectToRoute("admin-categorias-produto", new { alertType = alertType, alertMessage = alertMessage });
                        }
                    }
                }
                catch (HttpRequestException ex)
                {
                    // Este catch pegará erros de rede, API fora do ar, etc.
                    return RedirectToRoute("admin-categorias-produto", new { alertType = "error", alertMessage = $"Não foi possível conectar ao servidor. Tente novamente mais tarde ou entre em contato com a nossa equipe." });
                }
                catch (Exception ex)
                {
                    // Erros inesperados no próprio Controller (ex: problema com IFormFile)
                    return RedirectToRoute("admin-categorias-produto", new { alertType = "error", alertMessage = $"Ocorreu um erro inesperado durante o cadastro." });
                }
            }
            else
            {
                return RedirectToRoute("usuario-perfil", new { guid = User.FindFirst("GUID").Value });
            }
        }

    }
}
