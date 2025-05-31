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
        public async Task<IActionResult> Cadastro(ProdutoModel model, IFormFile[] ProdutoImagens)
        {
            // Validação de autenticação
            var userGuidFromClaims = User.FindFirst("GUID")?.Value;
            if (string.IsNullOrEmpty(userGuidFromClaims))
            {
                return RedirectToRoute("login", new { alertType = "error", alertMessage = "Sua sessão expirou ou não foi possível identificar o usuário. Faça login novamente." });
            }

            model.Usuario_GUID = userGuidFromClaims;

            // Validação do modelo
            if (!ModelState.IsValid)
            {
                var erros = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                string mensagemErro = "Corrija os erros no formulário: " + string.Join(" ", erros);
                return RedirectToRoute("usuario-anuncio-cadastrar", new { alertType = "error", alertMessage = mensagemErro, guid = userGuidFromClaims });
            }

            if (ProdutoImagens == null || ProdutoImagens.Length == 0 || ProdutoImagens[0] == null || ProdutoImagens[0].Length == 0)
            {
                return RedirectToRoute("usuario-anuncio-cadastrar", new { alertType = "error", alertMessage = "A imagem principal do produto é obrigatória.", guid = userGuidFromClaims });
            }

            try
            {
                var httpClient = _httpClientFactory.CreateClient("ApiAjudeiMais");

                // 1. Envia o produto via JSON (POST api/Produto)
                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(model),
                    Encoding.UTF8,
                    "application/json"
                );

                var json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync($"{Tools.Assistant.ServerURL()}api/Produto", content);

                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return RedirectToRoute("usuario-anuncio-cadastrar", new { alertType = "error", alertMessage = $"Erro ao cadastrar produto: {errorMessage}", guid = userGuidFromClaims });
                }

                // Lê a resposta para extrair o produtoId retornado
                var responseContent = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<ApiHelper.ApiResponse<ProdutoResponse>>(responseContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (apiResponse == null || !apiResponse.Success)
                {
                    return RedirectToRoute("usuario-anuncio-cadastrar", new { alertType = "error", alertMessage = apiResponse?.Message ?? "Erro ao cadastrar produto.", guid = userGuidFromClaims });
                }

                var produtoId = apiResponse.Data.Produto_ID;

                // 2. Envia as imagens para api/Produto/upload-imagens vinculando ao produtoId
                if (ProdutoImagens != null && ProdutoImagens.Length > 0)
                {
                    using var formData = new MultipartFormDataContent();

                    // Adicione o Produto_ID (se necessário)
                    formData.Add(new StringContent(produtoId.ToString()), "Produto_ID");

                    // Adicione a imagem (IFormFile) com o nome "Imagem"
                    foreach (var imagem in ProdutoImagens)
                    {
                        if (imagem != null && imagem.Length > 0)
                        {
                            var imageContent = new StreamContent(imagem.OpenReadStream());
                            imageContent.Headers.ContentType = new MediaTypeHeaderValue(imagem.ContentType);

                            // OBS: O nome do campo DEVE ser "Imagem" para bater com o DTO
                            formData.Add(imageContent, "Imagem", imagem.FileName);
                        }
                    }


                    var uploadResponse = await httpClient.PostAsync($"{Tools.Assistant.ServerURL()}api/ProdutoImagem", formData);
                     responseBody = await uploadResponse.Content.ReadAsStringAsync();


                    if (!uploadResponse.IsSuccessStatusCode)
                    {
                        var uploadError = await uploadResponse.Content.ReadAsStringAsync();
                        return RedirectToRoute("usuario-anuncio-cadastrar", new { alertType = "error", alertMessage = $"Erro ao enviar imagens: {uploadError}", guid = userGuidFromClaims });
                    }
                }

                // Se tudo OK, redireciona para dashboard
                return RedirectToRoute("usuario-dashboard", new { alertType = "success", alertMessage = apiResponse.Message, guid = userGuidFromClaims });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no cadastro do produto.");
                return RedirectToRoute("usuario-anuncio-cadastrar", new { alertType = "error", alertMessage = "Erro inesperado no cadastro. Tente novamente.", guid = userGuidFromClaims });
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
