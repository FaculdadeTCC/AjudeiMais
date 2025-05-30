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

        [RoleAuthorize("usuario")]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detalhe()
        {
            return View();
        }

        [RoleAuthorize("admin", "usuario")]
        [HttpPost]
        public async Task<IActionResult> Cadastro(ProdutoModel model, IFormFile[] Imagens)
        {
            // --- 1. Validação de Autenticação e Autorização do Usuário Logado ---
            string loggedInUserGuid = string.Empty;
            var unauthorizedResult = ControllerHelpers.HandleUnauthorizedAccess(this, _logger, out loggedInUserGuid);

            if (unauthorizedResult != null)
            {
                return unauthorizedResult; // Redireciona para login ou home
            }

            var userGuidFromClaims = User.FindFirst("GUID")?.Value;

            if (string.IsNullOrEmpty(userGuidFromClaims))
            {
                _logger.LogWarning("GUID do usuário não encontrado nos claims durante o cadastro de produto.");
                // Redireciona para a página de login se o GUID não for encontrado
                return RedirectToRoute("login", new { alertType = "error", alertMessage = "Sua sessão expirou ou não foi possível identificar o usuário. Por favor, faça login novamente." });
            }
            if (model.Usuario == null)
            {
                model.Usuario = new UsuarioPerfilModel();
            }
            model.Usuario.GUID = userGuidFromClaims;

            // --- 2. Validação do Modelo (Campos Obrigatórios da View) ---
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                var errorMessage = "Por favor, corrija os erros no formulário: " + string.Join(" ", errors);

                return RedirectToRoute("produto-cadastro", new { alertType = "error", alertMessage = errorMessage, guid = userGuidFromClaims });
            }

            if (Imagens == null || Imagens.Length == 0 || Imagens[0] == null || Imagens[0].Length == 0)
            {
                return RedirectToRoute("produto-cadastro", new { alertType = "error", alertMessage = "A imagem principal do anúncio é obrigatória.", guid = userGuidFromClaims });
            }

            // --- 3. Envio dos Dados para a API ---
            try
            {
                var httpClient = _httpClientFactory.CreateClient("ApiAjudeiMais");

                using (var formData = new MultipartFormDataContent())
                {
                    formData.AddObjectAsFormFields(model);

                    for (int i = 0; i < Imagens.Length; i++)
                    {
                        var imagem = Imagens[i];
                        if (imagem != null && imagem.Length > 0)
                        {
                            var fileContent = new StreamContent(imagem.OpenReadStream());
                            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(imagem.ContentType);

                            // O nome "Imagens" deve corresponder ao parâmetro na sua API.
                            // A API deve inferir a imagem principal pela ordem (primeira imagem = IsAtivo).
                            formData.Add(fileContent, "Imagens", imagem.FileName);
                        }
                    }

                    var response = await httpClient.PostAsync($"{BASE_URL}/api/produto", formData);

                    var apiResponseContent = await response.Content.ReadAsStringAsync();
                    ApiHelper.ApiResponse<ProdutoModel> apiResponse = null;

                    // Tenta desserializar a resposta da API
                    try
                    {
                        apiResponse = System.Text.Json.JsonSerializer.Deserialize<ApiHelper.ApiResponse<ProdutoModel>>(
                            apiResponseContent,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                        );
                    }
                    catch (JsonException jsonEx)
                    {
                        _logger.LogError(jsonEx, "Erro ao desserializar resposta da API de cadastro de produto: {ApiResponseContent}", apiResponseContent);
                        return RedirectToRoute("produto-cadastro", new { alertType = "error", alertMessage = "Ocorreu um erro no formato da resposta da API. Contacte o suporte.", guid = userGuidFromClaims });
                    }

                    // --- 4. Verificação da Resposta da API ---
                    if (response.IsSuccessStatusCode && apiResponse != null && apiResponse.Success)
                    {
                        // Sucesso: Redireciona com a mensagem de sucesso da API
                        return RedirectToRoute("produto-lista-usuario", new { alertType = apiResponse.Type, alertMessage = apiResponse.Message, guid = userGuidFromClaims });
                    }
                    else
                    {
                        // API retornou erro ou não sucesso
                        string alertType = apiResponse?.Type ?? "error";
                        string alertMessage = apiResponse?.Message ?? "Não foi possível processar o cadastro do anúncio. Tente novamente.";

                        _logger.LogError("API retornou erro ao cadastrar produto. Status: {StatusCode}, Message: {Message}", response.StatusCode, alertMessage);
                        return RedirectToRoute("produto-cadastro", new { alertType = alertType, alertMessage = alertMessage, guid = userGuidFromClaims });
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                // Erros de rede
                return RedirectToRoute("produto-cadastro", new { alertType = "error", alertMessage = $"Não foi possível conectar ao servidor. Tente novamente mais tarde ou entre em contato com o suporte.", guid = userGuidFromClaims });
            }
            catch (Exception ex)
            {
                // Qualquer outro erro inesperado
                _logger.LogError(ex, "Ocorreu um erro inesperado durante o processamento do cadastro do anúncio.");
                return RedirectToRoute("produto-cadastro", new { alertType = "error", alertMessage = $"Ocorreu um erro inesperado durante o cadastro. Por favor, tente novamente.", guid = userGuidFromClaims });
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

            // Você pode usar o 'guid' recebido da rota para carregar dados, se este for um cenário de edição.
            // Para um novo cadastro, 'guid' pode ser ignorado ou representar o ID do usuário (como em 'produto-cadastro/{guid}').
            // Se o 'guid' for o ID do usuário logado, você pode fazer uma verificação.

            // Inicialize o modelo para a view
            var model = new ProdutoModel();
            // Carregue as categorias para o dropdown
            var response = await ApiHelper.ListAllCategoriasProdutoAtivosAsync(_httpClientFactory);

            model.Categorias = response.Data;

            // Se o modelo precisar do GUID do usuário logado para exibição inicial, atribua-o aqui.
            if (model.Usuario == null)
            {
                model.Usuario = new UsuarioPerfilModel();
            }
            model.Usuario.GUID = loggedInUserGuid; // Atribua o GUID obtido de HandleUnauthorizedAccess

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
