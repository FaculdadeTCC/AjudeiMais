using System; // Para Exception
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json; // Usar System.Text.Json consistentemente
using System.Threading.Tasks;
using AjudeiMais.Ecommerce.Filters; // Para RoleAuthorize
using AjudeiMais.Ecommerce.Models.Usuario;
using AjudeiMais.Ecommerce.Tools; // Para Assistant e ApiHelper, e o novo ControllerHelpers
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http; // Para IFormFile
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json; // Para logging (opcional, mas boa prática)

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
        [RoleAuthorize("usuario", "admin")]
        public async Task<IActionResult> Index(string guid)
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

            // Se chegou até aqui, o usuário está autenticado, tem GUID e tem permissão para o perfil solicitado
            var (usuario, errorMessage) = await ApiHelper.GetUsuarioByGuidAsync(_httpClientFactory, guid);

            if (usuario != null)
            {

                return View(usuario);
            }
            else
            {
                return RedirectToRoute("usuario-perfil", new { alertType = "error", alertMessage = errorMessage, guid = loggedInUserGuid });
            }
        }

        [RoleAuthorize("usuario", "admin")]
        [HttpGet]
        public async Task<IActionResult> Perfil(string guid)
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

            // Se chegou até aqui, o usuário está autenticado, tem GUID e tem permissão para o perfil solicitado
            var (usuario, errorMessage) = await ApiHelper.GetUsuarioByGuidAsync(_httpClientFactory, guid);

            if (usuario != null)
            {
                return View(usuario);
            }
            else
            {
                _logger?.LogError("Erro ao obter dados do perfil do usuário {Guid}: {ErrorMessage}", guid, errorMessage);
                // Em caso de erro na API, redireciona para o próprio perfil do usuário logado com a mensagem de erro
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
                        formData.AddObjectAsFormFields(model);
                        formData.Add(new StringContent("usuario"), "Role");
                        formData.AddFileContent(FotoDePerfil, "FotoDePerfil");

                        var response = await httpClient.PostAsync($"{Tools.Assistant.ServerURL()}api/Usuario", formData);

                        // Sempre tentaremos deserializar para ApiResponse<Usuario>
                        var apiResponseContent = await response.Content.ReadAsStringAsync();
                        ApiHelper.ApiResponse<UsuarioPerfilModel> apiResponse = null;

                        try
                        {
                            // Prioriza System.Text.Json
                            apiResponse = System.Text.Json.JsonSerializer.Deserialize<ApiHelper.ApiResponse<UsuarioPerfilModel>>(
                                apiResponseContent,
                                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                            );
                        }
                        catch (System.Text.Json.JsonException jsonEx)
                        {
                            apiResponse = new ApiHelper.ApiResponse<UsuarioPerfilModel>
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
        [HttpGet]
        public async Task<IActionResult> AlterarDados(string guid)
        {
            // Unificado o tratamento de não autenticado/GUID inválido
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToRoute("home"); // Redireciona para home se não autenticado
            }

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

            // Se chegou até aqui, o usuário está autenticado, tem GUID e tem permissão para o perfil solicitado
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

        /// <summary>
        /// Método para realizar o cadastro de um usuário enviando os dados e a foto de perfil para a API.
        /// </summary>
        /// <param name="model">Objeto com os dados do usuário a serem cadastrados.</param>
        /// <param name="FotoDePerfil">Arquivo da foto de perfil enviada pelo usuário.</param>
        /// <returns>Redireciona para a página de login em caso de sucesso, ou para a página de cadastro com mensagem de erro em caso de falha.</returns>
        [RoleAuthorize("usuario", "admin")]
        [HttpPost]
        public async Task<IActionResult> AtualizarDadosPessoais(UsuarioDadosPessoais model, IFormFile FotoDePerfil)
        {

            if (User.Identity.IsAuthenticated)
            {
                string guid = model.GUID;

                try
                {
                    var httpClient = _httpClientFactory.CreateClient("ApiAjudeiMais");

                    using (var formData = new MultipartFormDataContent())
                    {
                        formData.AddObjectAsFormFields(model);
                        formData.AddFileContent(FotoDePerfil, "FotoDePerfil");

                        var response = await httpClient.PostAsync($"{Tools.Assistant.ServerURL()}api/Usuario/AtualizarDadosPessoais", formData);

                        var apiResponseContent = await response.Content.ReadAsStringAsync();
                        ApiHelper.ApiResponse<UsuarioPerfilModel> apiResponse = null;

                        try
                        {
                            // Prioriza System.Text.Json
                            apiResponse = System.Text.Json.JsonSerializer.Deserialize<ApiHelper.ApiResponse<UsuarioPerfilModel>>(
                                apiResponseContent,
                                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                            );
                        }
                        catch (System.Text.Json.JsonException jsonEx)
                        {
                            apiResponse = new ApiHelper.ApiResponse<UsuarioPerfilModel>
                            {
                                Success = false,
                                Type = "error",
                                Message = "Ocorreu um erro no formato da resposta da API. Contacte o suporte."
                            };

                            return RedirectToRoute("usuario-alterar-dados", new { alertType = apiResponse.Type, alertMessage = apiResponse.Message, guid = guid });

                        }

                        if (response.IsSuccessStatusCode && apiResponse != null && apiResponse.Success)
                        {
                            return RedirectToRoute("usuario-alterar-dados", new { alertType = apiResponse.Type, alertMessage = apiResponse.Message, guid = guid });
                        }
                        else
                        {
                            string alertType = "error";
                            string alertMessage = apiResponse?.Message ?? "Não foi possível atualizar os dados. Tente novamente.";

                            return RedirectToRoute("usuario-alterar-dados", new { guid = guid, alertType = alertType, alertMessage = alertMessage });
                        }
                    }
                }
                catch (HttpRequestException ex)
                {
                    // Este catch pegará erros de rede, API fora do ar, etc.
                    return RedirectToRoute("usuario-alterar-dados", new { guid = guid, alertType = "error", alertMessage = $"Não foi possível conectar ao servidor da API: {ex.Message}" });
                }
                catch (Exception ex)
                {
                    // Erros inesperados no próprio Controller (ex: problema com IFormFile)
                    return RedirectToRoute("usuario-alterar-dados", new { guid = guid, alertType = "error", alertMessage = $"Ocorreu um erro inesperado durante o cadastro. {ex.Message}" });
                }
            }
            else
            {
                return RedirectToRoute("home");
            }
        }

        [RoleAuthorize("usuario", "admin")]
        [HttpPost]
        public async Task<IActionResult> AtualizarEndereco(UsuarioEndereco model)
        {

            if (User.Identity.IsAuthenticated)
            {
                string guid = model.GUID;

                try
                {
                    var httpClient = _httpClientFactory.CreateClient("ApiAjudeiMais");

                    using (var formData = new MultipartFormDataContent())
                    {
                        formData.AddObjectAsFormFields(model);

                        var response = await httpClient.PostAsync($"{Tools.Assistant.ServerURL()}api/Usuario/AtualizarEndereco", formData);

                        var apiResponseContent = await response.Content.ReadAsStringAsync();
                        ApiHelper.ApiResponse<UsuarioPerfilModel> apiResponse = null;

                        try
                        {
                            // Prioriza System.Text.Json
                            apiResponse = System.Text.Json.JsonSerializer.Deserialize<ApiHelper.ApiResponse<UsuarioPerfilModel>>(
                                apiResponseContent,
                                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                            );
                        }
                        catch (System.Text.Json.JsonException jsonEx)
                        {
                            apiResponse = new ApiHelper.ApiResponse<UsuarioPerfilModel>
                            {
                                Success = false,
                                Type = "error",
                                Message = "Ocorreu um erro no formato da resposta da API. Contacte o suporte."
                            };

                            return RedirectToRoute("usuario-alterar-dados", new { alertType = apiResponse.Type, alertMessage = apiResponse.Message, guid = guid });

                        }

                        if (response.IsSuccessStatusCode && apiResponse != null && apiResponse.Success)
                        {
                            return RedirectToRoute("usuario-alterar-dados", new { alertType = apiResponse.Type, alertMessage = apiResponse.Message, guid = guid });
                        }
                        else
                        {
                            string alertType = "error";
                            string alertMessage = apiResponse?.Message ?? "Não foi possível atualizar os dados. Tente novamente.";

                            return RedirectToRoute("usuario-alterar-dados", new { guid = guid, alertType = alertType, alertMessage = alertMessage });
                        }
                    }
                }
                catch (HttpRequestException ex)
                {
                    // Este catch pegará erros de rede, API fora do ar, etc.
                    return RedirectToRoute("usuario-alterar-dados", new { guid = guid, alertType = "error", alertMessage = $"Não foi possível conectar ao servidor da API: {ex.Message}" });
                }
                catch (Exception ex)
                {
                    // Erros inesperados no próprio Controller (ex: problema com IFormFile)
                    return RedirectToRoute("usuario-alterar-dados", new { guid = guid, alertType = "error", alertMessage = $"Ocorreu um erro inesperado durante o cadastro. {ex.Message}" });
                }
            }
            else
            {
                return RedirectToRoute("home");
            }
        }

        [RoleAuthorize("usuario", "admin")]
        [HttpPost]
        public async Task<IActionResult> AtualizarSenha(UsuarioSenha model)
        {

            if (User.Identity.IsAuthenticated)
            {
                string guid = model.GUID;

                try
                {
                    var httpClient = _httpClientFactory.CreateClient("ApiAjudeiMais");

                    using (var formData = new MultipartFormDataContent())
                    {
                        formData.AddObjectAsFormFields(model);

                        var response = await httpClient.PostAsync($"{Tools.Assistant.ServerURL()}api/Usuario/AtualizarSenha", formData);

                        var apiResponseContent = await response.Content.ReadAsStringAsync();
                        ApiHelper.ApiResponse<UsuarioPerfilModel> apiResponse = null;

                        try
                        {
                            // Prioriza System.Text.Json
                            apiResponse = System.Text.Json.JsonSerializer.Deserialize<ApiHelper.ApiResponse<UsuarioPerfilModel>>(
                                apiResponseContent,
                                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                            );
                        }
                        catch (System.Text.Json.JsonException jsonEx)
                        {
                            apiResponse = new ApiHelper.ApiResponse<UsuarioPerfilModel>
                            {
                                Success = false,
                                Type = "error",
                                Message = "Ocorreu um erro no formato da resposta da API. Contacte o suporte."
                            };

                            return RedirectToRoute("usuario-alterar-dados", new { alertType = apiResponse.Type, alertMessage = apiResponse.Message, guid = guid });

                        }

                        if (response.IsSuccessStatusCode && apiResponse != null && apiResponse.Success)
                        {
                            return RedirectToRoute("usuario-alterar-dados", new { alertType = apiResponse.Type, alertMessage = apiResponse.Message, guid = guid });
                        }
                        else
                        {
                            string alertType = "error";
                            string alertMessage = apiResponse?.Message ?? "Não foi possível atualizar os dados. Tente novamente.";

                            return RedirectToRoute("usuario-alterar-dados", new { guid = guid, alertType = alertType, alertMessage = alertMessage });
                        }
                    }
                }
                catch (HttpRequestException ex)
                {
                    // Este catch pegará erros de rede, API fora do ar, etc.
                    return RedirectToRoute("usuario-alterar-dados", new { guid = guid, alertType = "error", alertMessage = $"Não foi possível conectar ao servidor da API: {ex.Message}" });
                }
                catch (Exception ex)
                {
                    // Erros inesperados no próprio Controller (ex: problema com IFormFile)
                    return RedirectToRoute("usuario-alterar-dados", new { guid = guid, alertType = "error", alertMessage = $"Ocorreu um erro inesperado durante o cadastro. {ex.Message}" });
                }
            }
            else
            {
                return RedirectToRoute("home");
            }
        }

        [RoleAuthorize("usuario", "admin")]
        [HttpPost]
        public async Task<IActionResult> ExcluirConta(UsuarioExcluirConta model)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToRoute("home");

            string guid = model.GUID;

            try
            {
                var httpClient = _httpClientFactory.CreateClient("ApiAjudeiMais");

                using (var formData = new MultipartFormDataContent())
                {
                    formData.AddObjectAsFormFields(model);

                    var response = await httpClient.PostAsync($"{Tools.Assistant.ServerURL()}api/Usuario/VerificarSenha", formData);
                    var apiResponseContent = await response.Content.ReadAsStringAsync();

                    ApiHelper.ApiResponse<UsuarioPerfilModel> apiResponse;

                    try
                    {
                        apiResponse = System.Text.Json.JsonSerializer.Deserialize<ApiHelper.ApiResponse<UsuarioPerfilModel>>(
                            apiResponseContent,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                        );
                    }
                    catch (System.Text.Json.JsonException)
                    {
                        return RedirectToRoute("usuario-alterar-dados", new
                        {
                            alertType = "error",
                            alertMessage = "Ocorreu um erro no formato da resposta da API. Contacte o suporte.",
                            guid = guid
                        });
                    }

                    if (response.IsSuccessStatusCode && apiResponse != null && apiResponse.Success)
                    {
                        // 2) Se senha ok, deleta a conta
                        var responseDelete = await httpClient.DeleteAsync($"{Tools.Assistant.ServerURL()}api/Usuario/{guid}");
                        var apiDeleteContent = await responseDelete.Content.ReadAsStringAsync();

                        ApiHelper.ApiResponse<UsuarioPerfilModel> apiDeleteResponse;
                        try
                        {
                            apiDeleteResponse = System.Text.Json.JsonSerializer.Deserialize<ApiHelper.ApiResponse<UsuarioPerfilModel>>(
                                apiDeleteContent,
                                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                            );
                        }
                        catch (System.Text.Json.JsonException)
                        {
                            return RedirectToRoute("usuario-alterar-dados", new
                            {
                                alertType = "error",
                                alertMessage = "Ocorreu um erro no formato da resposta da API ao deletar. Contacte o suporte.",
                                guid = guid
                            });
                        }

                        if (responseDelete.IsSuccessStatusCode && apiDeleteResponse != null && apiDeleteResponse.Success)
                        {
                            await HttpContext.SignOutAsync();

                            return RedirectToRoute("home", new
                            {
                                alertType = apiDeleteResponse.Type,
                                alertMessage = apiDeleteResponse.Message
                            });
                        }
                        else
                        {
                            return RedirectToRoute("usuario-alterar-dados", new
                            {
                                guid = guid,
                                alertType = "error",
                                alertMessage = apiDeleteResponse?.Message ?? "Não foi possível deletar a conta. Tente novamente."
                            });
                        }
                    }
                    else
                    {
                        return RedirectToRoute("usuario-alterar-dados", new
                        {
                            guid = guid,
                            alertType = "error",
                            alertMessage = "Senha incorreta. Tente novamente."
                        });
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                return RedirectToRoute("usuario-alterar-dados", new
                {
                    guid = guid,
                    alertType = "error",
                    alertMessage = $"Não foi possível conectar ao servidor da API: {ex.Message}"
                });
            }
            catch (Exception ex)
            {
                return RedirectToRoute("usuario-alterar-dados", new
                {
                    guid = guid,
                    alertType = "error",
                    alertMessage = $"Ocorreu um erro inesperado durante a operação. {ex.Message}"
                });
            }
        }

        public IActionResult Anuncios()
        {
            return View();
        }
    }
}