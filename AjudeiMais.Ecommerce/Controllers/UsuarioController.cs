using System; // Para Exception
using System.Net;
using System.Net.Http;
using System.Text.Json; // Usar System.Text.Json consistentemente
using System.Threading.Tasks;
using AjudeiMais.Ecommerce.Filters; // Para RoleAuthorize
using AjudeiMais.Ecommerce.Models;
using AjudeiMais.Ecommerce.Tools; // Para Assistant e ApiHelper, e o novo ControllerHelpers
using Microsoft.AspNetCore.Http; // Para IFormFile
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; // Para logging (opcional, mas boa prática)

namespace AjudeiMais.Ecommerce.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<UsuarioController> _logger; // Injeção de logger (opcional)

        public UsuarioController(IHttpClientFactory httpClientFactory, ILogger<UsuarioController> logger = null)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger; // Atribui o logger, se injetado
        }

        // --- Ações do Controlador ---
        [RoleAuthorize("admin")]
        public IActionResult Index()
        {
            return View();
        }

        [RoleAuthorize("usuario", "admin")]
        [HttpGet]
        public async Task<IActionResult> Perfil(string guid)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToRoute("home");
            }

            // Validação de segurança para garantir que o usuário só veja seu próprio perfil
            var loggedInUserGuid = Assistant.GetUserGuidFromClaims(User, "GUID");
            if (string.IsNullOrEmpty(loggedInUserGuid) || (!User.IsInRole("admin") && !string.Equals(guid, loggedInUserGuid, StringComparison.OrdinalIgnoreCase)))
            {
                // Se não for admin e o GUID não corresponder, redireciona para o próprio perfil ou home
                return RedirectToRoute("usuario-perfil", new { alertType = "error", alertMessage = "Você não tem permissão para acessar este perfil.", guid = loggedInUserGuid });
            }

            var (usuario, errorMessage) = await ApiHelper.GetUsuarioByGuidAsync(_httpClientFactory, guid); // Use o GUID da URL aqui

            if (usuario != null)
            {
                return View(usuario);
            }
            else
            {
                _logger?.LogError("Erro ao obter dados do perfil do usuário {Guid}: {ErrorMessage}", guid, errorMessage);
                // Redireciona para o perfil do usuário logado em caso de erro ao obter os dados
                return RedirectToRoute("usuario-perfil", new { alertType = "error", alertMessage = errorMessage, guid = loggedInUserGuid });
            }
        }

        [HttpGet]
        public IActionResult Cadastro()
        {
            // Se o usuário já estiver autenticado, não deve acessar a página de cadastro novamente.
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToRoute("home");
            }
            return View();
        }

        [RoleAuthorize("usuario", "admin")]
        [HttpGet]
        public async Task<IActionResult> AlterarDados(string guid)
        {
            // Unificado o tratamento de não autenticado/GUID inválido
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToRoute("home"); // Redireciona para home se não autenticado
            }

            string loggedInUserGuid;

            var unauthorizedResult = ControllerHelpers.HandleUnauthorizedAccess(this, _logger, out loggedInUserGuid);

            if (unauthorizedResult != null)
            {
                return unauthorizedResult; // Retorna login ou erro se GUID inválido
            }

            // Valida se o usuário logado está tentando acessar seu próprio perfil ou se é um admin
            if (!User.IsInRole("admin") && !string.Equals(guid, loggedInUserGuid, StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToRoute("usuario-perfil", new { alertType = "error", alertMessage = "Você não tem permissão para acessar este perfil.", guid = loggedInUserGuid });
            }

            // Busca os dados do usuário usando o GUID da URL (pode ser o próprio ou de outro se for admin)
            var (usuario, errorMessage) = await ApiHelper.GetUsuarioByGuidAsync(_httpClientFactory, guid);

            if (usuario != null)
            {
                return View("AlterarDados", usuario); // Retorna a view AlterarDados
            }
            else
            {
                _logger?.LogError("Erro ao obter dados para AlterarDados para o GUID {Guid}: {ErrorMessage}", guid, errorMessage);
                // Redireciona para o perfil do usuário logado se os dados do perfil solicitado não puderem ser carregados
                return RedirectToRoute("usuario-perfil", new { alertType = "error", alertMessage = errorMessage, guid = loggedInUserGuid });
            }
        }

        /// <summary>
        /// Método para realizar o cadastro de um usuário enviando os dados e a foto de perfil para a API.
        /// </summary>
        /// <param name="model">Objeto com os dados do usuário a serem cadastrados.</param>
        /// <param name="FotoDePerfil">Arquivo da foto de perfil enviada pelo usuário.</param>
        /// <returns>Redireciona para a página de login em caso de sucesso, ou para a página de cadastro com mensagem de erro em caso de falha.</returns>
        [HttpPost]
        public async Task<IActionResult> Cadastro(UsuarioModel model, IFormFile FotoDePerfil)
        {
            if (!User.Identity.IsAuthenticated)
            {
                try
                {
                    var httpClient = _httpClientFactory.CreateClient("ApiAjudeiMais");

                    using (var formData = new MultipartFormDataContent())
                    {
                        // Usa o método de extensão para adicionar automaticamente todas as propriedades simples do model
                        formData.AddObjectAsFormFields(model);

                        // Adiciona manualmente campos extras que não estão no model (exemplo: Role)
                        formData.Add(new StringContent("usuario"), "Role");

                        // Usa o método de extensão para adicionar o arquivo de forma limpa
                        formData.AddFileContent(FotoDePerfil, "FotoDePerfil");

                        // Envia a requisição POST para a API com o formulário multipart/form-data
                        var response = await httpClient.PostAsync($"{Tools.Assistant.ServerURL()}api/Usuario", formData);

                        if (response.IsSuccessStatusCode)
                        {
                            // Cadastro realizado com sucesso, redireciona para login com mensagem de sucesso
                            return RedirectToRoute("login", new { alertType = "success", alertMessage = "Cadastro realizado com sucesso. Faça o Login." });
                        }
                        else
                        {
                            // Tenta ler e interpretar os detalhes do problema retornados pela API
                            var responseContent = await response.Content.ReadAsStringAsync();
                            ProblemDetails problemDetails = null;

                            try
                            {
                                problemDetails = System.Text.Json.JsonSerializer.Deserialize<ProblemDetails>(responseContent,
                                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                            }
                            catch (System.Text.Json.JsonException)
                            {
                                // Ignora se não for ProblemDetails, usa a mensagem crua
                            }


                            if (problemDetails?.Title != null && problemDetails?.Detail != null)
                            {
                                return RedirectToRoute("usuario-cadastro", new { alertType = "error", alertMessage = $"{problemDetails.Title}: {problemDetails.Detail}" });
                            }
                            else if (!string.IsNullOrEmpty(responseContent))
                            {
                                return RedirectToRoute("usuario-cadastro", new { alertType = "error", alertMessage = $"Erro ao cadastrar: {responseContent}" });
                            }
                            else
                            {
                                return RedirectToRoute("usuario-cadastro", new { alertType = "error", alertMessage = "Ocorreu um erro ao cadastrar." });
                            }
                        }
                    }
                }
                catch (HttpRequestException ex)
                {
                    // Trata erros de conexão com o servidor
                    return RedirectToRoute("usuario-cadastro", new { alertType = "error", alertMessage = $"Não foi possível conectar ao servidor: {ex.Message}" });
                }
                catch (Exception ex)
                {
                    // Trata erros inesperados no processo de cadastro
                    return RedirectToRoute("usuario-cadastro", new { alertType = "error", alertMessage = $"Ocorreu um erro inesperado durante o cadastro. {ex.Message}" });
                }
            }
            else
            {
                return RedirectToRoute("usuario-home");
            }
        }

        [HttpGet]
        public async Task<JsonResult> VerificarEmailExistente([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return Json(false);
            }

            try
            {
                var httpClient = _httpClientFactory.CreateClient("ApiAjudeiMais");
                var response = await httpClient.GetAsync($"{Assistant.ServerURL()}api/Usuario/GetByEmail/{email}");

                if (response.IsSuccessStatusCode)
                {
                    // A API deve retornar um status 200 OK e um corpo com o usuário se encontrar,
                    // ou 404 Not Found se não encontrar.
                    // ContentLength > 0 indica que algo foi retornado, geralmente um usuário.
                    if (response.Content.Headers.ContentLength > 0)
                    {
                        return Json(new { exists = true, message = "Este e-mail já está cadastrado." });
                    }
                    else
                    {
                        // Se 200 OK mas sem conteúdo, significa que o e-mail não existe
                        return Json(new { exists = false });
                    }
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    // E-mail não encontrado na API
                    return Json(new { exists = false });
                }
                else
                {
                    // Outro erro da API (5xx, 4xx que não 404)
                    _logger?.LogWarning("Erro na API ao verificar e-mail {Email}: Status {StatusCode}", email, response.StatusCode);
                    return Json(new { exists = false, message = "Erro ao verificar e-mail. Tente novamente." });
                }
            }
            catch (HttpRequestException ex)
            {
                _logger?.LogError(ex, "Erro de conexão ao verificar e-mail {Email}.", email);
                return Json(new { exists = false, message = "Não foi possível conectar ao servidor para verificar o e-mail." });
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Erro inesperado ao verificar e-mail {Email}.", email);
                return Json(new { exists = false, message = "Ocorreu um erro inesperado ao verificar o e-mail." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AtualizarDadosPessoais(UsuarioPerfilModel model, IFormFile FotoDePerfil)
        {
            if (!User.Identity.IsAuthenticated)
            {
                try
                {
                    var httpClient = _httpClientFactory.CreateClient("ApiAjudeiMais");

                    using (var formData = new MultipartFormDataContent())
                    {
                        // Usa o método de extensão para adicionar automaticamente todas as propriedades simples do model
                        formData.AddObjectAsFormFields(model);

                        // Adiciona manualmente campos extras que não estão no model (exemplo: Role)
                        formData.Add(new StringContent("usuario"), "Role");

                        // Usa o método de extensão para adicionar o arquivo de forma limpa
                        formData.AddFileContent(FotoDePerfil, "FotoDePerfil");

                        // Envia a requisição POST para a API com o formulário multipart/form-data
                        var response = await httpClient.PostAsync($"{Tools.Assistant.ServerURL()}api/Usuario", formData);

                        if (response.IsSuccessStatusCode)
                        {
                            // Cadastro realizado com sucesso, redireciona para login com mensagem de sucesso
                            return RedirectToRoute("login", new { alertType = "success", alertMessage = response });
                        }
                        else
                        {
                            // Tenta ler e interpretar os detalhes do problema retornados pela API
                            var responseContent = await response.Content.ReadAsStringAsync();
                            ProblemDetails problemDetails = null;

                            try
                            {
                                problemDetails = System.Text.Json.JsonSerializer.Deserialize<ProblemDetails>(responseContent,
                                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                            }
                            catch (System.Text.Json.JsonException)
                            {
                                // Ignora se não for ProblemDetails, usa a mensagem crua
                            }


                            if (problemDetails?.Title != null && problemDetails?.Detail != null)
                            {
                                return RedirectToRoute("usuario-cadastro", new { alertType = "error", alertMessage = $"{problemDetails.Title}: {problemDetails.Detail}" });
                            }
                            else if (!string.IsNullOrEmpty(responseContent))
                            {
                                return RedirectToRoute("usuario-cadastro", new { alertType = "error", alertMessage = $"Erro ao cadastrar: {responseContent}" });
                            }
                            else
                            {
                                return RedirectToRoute("usuario-cadastro", new { alertType = "error", alertMessage = "Ocorreu um erro ao cadastrar." });
                            }
                        }
                    }
                }
                catch (HttpRequestException ex)
                {
                    // Trata erros de conexão com o servidor
                    return RedirectToRoute("usuario-cadastro", new { alertType = "error", alertMessage = $"Não foi possível conectar ao servidor: {ex.Message}" });
                }
                catch (Exception ex)
                {
                    // Trata erros inesperados no processo de cadastro
                    return RedirectToRoute("usuario-cadastro", new { alertType = "error", alertMessage = $"Ocorreu um erro inesperado durante o cadastro. {ex.Message}" });
                }
            }
            else
            {
                return RedirectToRoute("usuario-home");
            }
        }
        public IActionResult Anuncios()
        {
            return View();
        }
    }
}