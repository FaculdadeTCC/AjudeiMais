using System.Net.Http;
using System;
using AjudeiMais.Ecommerce.Filters;
using AjudeiMais.Ecommerce.Tools;
using Microsoft.AspNetCore.Mvc;
using AjudeiMais.Ecommerce.Models.Produto;
using System.Threading.Tasks;
using AjudeiMais.Ecommerce.Models.Usuario;
using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;

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

        [RoleAuthorize("usuario", "admin")]
        public async Task<IActionResult> Index(string guid)
        {
            string loggedInUserGuid;
            var unauthorizedResult = ControllerHelpers.HandleUnauthorizedAccess(this, _logger, out loggedInUserGuid);

            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

            var profileAccessResult = ControllerHelpers.ValidateUserProfileAccess(this, guid, loggedInUserGuid);

            if (profileAccessResult != null)
            {
                return profileAccessResult;
            }

            var (produtos, errorMessage) = await ApiHelper.ListAllAnunciosAtivosByGuidAsync(_httpClientFactory, guid);

            if (produtos != null)
            {
                ProdutoIndex model = new ProdutoIndex
                {
                    Anuncios = produtos
                };

                return View(model);
            }
            else
            {
                return RedirectToRoute("usuario-perfil", new { alertType = "error", alertMessage = errorMessage, guid = loggedInUserGuid });
            }
        }

        public IActionResult Detalhe()
        {
            return View();
        }

        [RoleAuthorize("admin", "usuario")]
        [HttpPost]
        public async Task<IActionResult> Cadastro(ProdutoModel model, IFormFile[] ProdutoImagens)
        {
            // 1. Validação de sessão/autenticação
            var userGuidFromClaims = User.FindFirst("GUID")?.Value;
            if (string.IsNullOrEmpty(userGuidFromClaims))
            {
                return RedirectToRoute("login", new
                {
                    alertType = "error",
                    alertMessage = "Sua sessão expirou ou não foi possível identificar o usuário. Faça login novamente."
                });
            }

            model.Usuario_GUID = userGuidFromClaims;

            // 2. Validação do modelo enviado pelo formulário
            if (!ModelState.IsValid)
            {
                var erros = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                string mensagemErro = "Corrija os erros no formulário: " + string.Join(" ", erros);

                return RedirectToRoute("usuario-anuncio-cadastrar", new
                {
                    alertType = "error",
                    alertMessage = mensagemErro,
                    guid = userGuidFromClaims
                });
            }

            // 3. Validação obrigatória da imagem principal
            if (ProdutoImagens == null || ProdutoImagens.Length == 0 || ProdutoImagens[0] == null || ProdutoImagens[0].Length == 0)
            {
                return RedirectToRoute("usuario-anuncio-cadastrar", new
                {
                    alertType = "error",
                    alertMessage = "A imagem principal do produto é obrigatória.",
                    guid = userGuidFromClaims
                });
            }

            try
            {
                var httpClient = _httpClientFactory.CreateClient("ApiAjudeiMais");

                // 4. Envia os dados do produto para a API
                var json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync($"{Tools.Assistant.ServerURL()}api/Produto", content);

                var responseBody = await response.Content.ReadAsStringAsync();

                ApiHelper.ApiResponse<ProdutoResponse> apiResponse;
                try
                {
                    apiResponse = JsonSerializer.Deserialize<ApiHelper.ApiResponse<ProdutoResponse>>(responseBody,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao desserializar resposta da API no cadastro do produto.");
                    return RedirectToRoute("usuario-anuncio-cadastrar", new
                    {
                        alertType = "error",
                        alertMessage = "Erro inesperado no cadastro. Tente novamente.",
                        guid = userGuidFromClaims
                    });
                }

                if (apiResponse == null || !apiResponse.Success)
                {
                    return RedirectToRoute("usuario-anuncio-cadastrar", new
                    {
                        alertType = "error",
                        alertMessage = apiResponse?.Message ?? "Erro ao cadastrar produto.",
                        guid = userGuidFromClaims
                    });
                }

                var produtoId = apiResponse.Data.Produto_ID;

                // 5. Envia cada imagem para a API
                foreach (var imagem in ProdutoImagens)
                {
                    if (imagem != null && imagem.Length > 0)
                    {
                        using var formData = new MultipartFormDataContent();

                        formData.Add(new StringContent(produtoId.ToString()), "Produto_ID");

                        var imageContent = new StreamContent(imagem.OpenReadStream());
                        imageContent.Headers.ContentType = new MediaTypeHeaderValue(imagem.ContentType);

                        formData.Add(imageContent, "Imagem", imagem.FileName);

                        var uploadResponse = await httpClient.PostAsync($"{Tools.Assistant.ServerURL()}api/ProdutoImagem", formData);
                        if (!uploadResponse.IsSuccessStatusCode)
                        {
                            var uploadResponseBody = await uploadResponse.Content.ReadAsStringAsync();
                            return RedirectToRoute("usuario-anuncio-cadastrar", new
                            {
                                alertType = "error",
                                alertMessage = $"Erro ao enviar imagem: {uploadResponseBody}",
                                guid = userGuidFromClaims
                            });
                        }
                    }
                }

                // 6. Sucesso no cadastro
                return RedirectToRoute("usuario-dashboard", new
                {
                    alertType = "success",
                    alertMessage = apiResponse.Message,
                    guid = userGuidFromClaims
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado no cadastro do produto.");
                return RedirectToRoute("usuario-anuncio-cadastrar", new
                {
                    alertType = "error",
                    alertMessage = "Erro inesperado no cadastro. Tente novamente.",
                    guid = userGuidFromClaims
                });
            }
        }

        // Seu método GET para exibir a view de Gerenciar Anúncio (permanece o mesmo, ou adicione o parâmetro 'guid' se a rota espera)
        [RoleAuthorize("admin", "usuario")]
        [HttpGet]
        public async Task<IActionResult> Cadastro([FromRoute] Guid guid) // Adicione [FromRoute] se a rota espera 'guid' aqui
        {
            // Lógica para obter o GUID do usuário logado, se necessário
            string loggedInUserGuid;
            var unauthorizedResult = ControllerHelpers.HandleUnauthorizedAccess(this, _logger, out loggedInUserGuid);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

            // Inicialize o modelo para a view
            var model = new ProdutoModel();
            // Carregue as categorias para o dropdown
            var response = await ApiHelper.ListAllCategoriasProdutoAtivosAsync(_httpClientFactory);

            model.Categorias = response.Data;


            model.Usuario_GUID = loggedInUserGuid; // Atribua o GUID obtido de HandleUnauthorizedAccess

            return View(model);
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
