using AjudeiMais.Ecommerce.Filters;
using AjudeiMais.Ecommerce.Models.Instituicao;
using AjudeiMais.Ecommerce.Models.Usuario;
using AjudeiMais.Ecommerce.Tools;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace AjudeiMais.Ecommerce.Controllers
{
    public class InstituicaoController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<UsuarioController> _logger; // Injeção de logger (opcional)
        public InstituicaoController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public IActionResult Index()
        {
            return View();
        }
        [RoleAuthorize("admin", "instituicao")]
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
                return RedirectToRoute("instituicao-perfil", new { alertType = "error", alertMessage = "Você não tem permissão para acessar este perfil.", guid = loggedInUserGuid });
            }

            var (instituicao, errorMessage) = await ApiHelper.GetInsituicaoByGuidAsync(_httpClientFactory, guid); // Use o GUID da URL aqui

            if (instituicao != null)
            {
                return View(instituicao);
            }
            else
            {
                _logger?.LogError("Erro ao obter dados do perfil do usuário {Guid}: {ErrorMessage}", guid, errorMessage);
                // Redireciona para o perfil do usuário logado em caso de erro ao obter os dados
                return RedirectToRoute("usuario-perfil", new { alertType = "error", alertMessage = errorMessage, guid = loggedInUserGuid });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Cadastro(InstituicaoModel model, IFormFile[] Fotos, IFormFile FotoPerfil)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("ApiAjudeiMais");

                using (var formData = new MultipartFormDataContent())
                {
                    // Adiciona os dados simples da instituição
                    formData.AddObjectAsFormFields(model);

                    // Usa o método de extensão para adicionar o arquivo de forma limpa
                    formData.AddFileContent(FotoPerfil, "FotoPerfil");
                    // Adiciona a role
                    formData.Add(new StringContent("instituicao"), "Role");

                    var enderecosJson = JsonConvert.SerializeObject(new List<EnderecoModel> { model.Endereco });

                    formData.Add(new StringContent(enderecosJson, Encoding.UTF8, "application/json"), "Enderecos");


                    // Adiciona as fotos, se houver
                    if (Fotos != null && Fotos.Length > 0)
                    {
                        for (int i = 0; i < Fotos.Length; i++)
                        {
                            var foto = Fotos[i];
                            if (foto != null && foto.Length > 0)
                            {
                                // Usa um nome como Fotos[0], Fotos[1], etc.
                                formData.AddFileContent(foto, $"Fotos");
                            }
                        }
                    }

                    // Faz a chamada para a API
                    var response = await httpClient.PostAsync($"{Assistant.ServerURL}api/Instituicao", formData);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToRoute("login", new { alertType = "success", alertMessage = "Cadastro da instituição realizado com sucesso. Faça o Login." });
                    }
                    else
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(responseContent);

                        if (problemDetails?.Title != null && problemDetails?.Detail != null)
                        {
                            return RedirectToRoute("instituicao-cadastrar", new { alertType = "error", alertMessage = $"{problemDetails.Title}: {problemDetails.Detail}" });
                        }
                        else if (!string.IsNullOrEmpty(responseContent))
                        {
                            return RedirectToRoute("instituicao-cadastrar", new { alertType = "error", alertMessage = $"Erro ao cadastrar: {responseContent}" });
                        }
                        else
                        {
                            return RedirectToRoute("instituicao-cadastrar", new { alertType = "error", alertMessage = "Ocorreu um erro ao cadastrar a instituição." });
                        }
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                return RedirectToRoute("instituicao-cadastrar", new { alertType = "error", alertMessage = $"Não foi possível conectar ao servidor: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return RedirectToRoute("instituicao-cadastrar", new { alertType = "error", alertMessage = $"Ocorreu um erro inesperado durante o cadastro. {ex.Message}" });
            }
        }


        [HttpGet]
        public IActionResult Cadastro()
        {
            return View();
        }

        [RoleAuthorize("admin", "instituicao")]
        [HttpGet]
        public async Task<IActionResult> AlterarDados(string guid)
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
                return RedirectToRoute("instituicao-perfil", new { alertType = "error", alertMessage = "Você não tem permissão para acessar este perfil.", guid = loggedInUserGuid });
            }

            var (instituicao, errorMessage) = await ApiHelper.GetInsituicaoByGuidAsync(_httpClientFactory, guid); // Use o GUID da URL aqui

            if (instituicao != null)
            {
                return View(instituicao);
            }
            else
            {
                _logger?.LogError("Erro ao obter dados do perfil do usuário {Guid}: {ErrorMessage}", guid, errorMessage);
                // Redireciona para o perfil do usuário logado em caso de erro ao obter os dados
                return RedirectToRoute("usuario-perfil", new { alertType = "error", alertMessage = errorMessage, guid = loggedInUserGuid });
            }
        }

        [HttpPost("AlterarDados")]
        [RoleAuthorize("instituicao", "admin")]
        public async Task<IActionResult> AlterarDados(InstituicaoDadosModel model, IFormFile? FotoPerfil)
        {
            if (User.Identity.IsAuthenticated)
            {
                string GUID = model.GUID;
                try
                {
                    var httpClient = _httpClientFactory.CreateClient("ApiAjudeiMais");

                    using (var formData = new MultipartFormDataContent())
                    {
                        // Adiciona os dados simples da instituição
                        formData.AddObjectAsFormFields(model);

                        formData.AddFileContent(FotoPerfil, "FotoPerfil");

                        // Adiciona a role
                        formData.Add(new StringContent("instituicao"), "Role");

                        // Faz a chamada para a API
                        var response = await httpClient.PostAsync($"{Assistant.ServerURL()}api/Instituicao/AtualizarDados", formData);

                        if (response.IsSuccessStatusCode)
                        {
                            return RedirectToRoute($"instituicao-perfil", new { guid = model.GUID, alertType = "success", alertMessage = "Dados da instituição realizado com sucesso." });
                        }
                        else
                        {
                            var responseContent = await response.Content.ReadAsStringAsync();
                            var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(responseContent);

                            if (problemDetails?.Title != null && problemDetails?.Detail != null)
                            {
                                return RedirectToRoute("instituicao-cadastrar", new { guid = model.GUID, alertType = "error", alertMessage = $"{problemDetails.Title}: {problemDetails.Detail}" });
                            }
                            else if (!string.IsNullOrEmpty(responseContent))
                            {
                                return RedirectToRoute("instituicao-cadastrar", new { alertType = "error", alertMessage = $"Erro ao cadastrar: {responseContent}" });
                            }
                            else
                            {
                                return RedirectToRoute("instituicao-cadastrar", new { alertType = "error", alertMessage = "Ocorreu um erro ao cadastrar a instituição." });
                            }
                        }
                    }
                }
                catch (HttpRequestException ex)
                {
                    return RedirectToRoute("instituicao-cadastrar", new { alertType = "error", alertMessage = $"Não foi possível conectar ao servidor: {ex.Message}" });
                }
                catch (Exception ex)
                {
                    return RedirectToRoute("instituicao-cadastrar", new { alertType = "error", alertMessage = $"Ocorreu um erro inesperado durante o cadastro. {ex.Message}" });
                }
            }
            else
            {
                return RedirectToRoute("home");

            }
          
        }

        [HttpPost("AtualizarFotos")]
        [RoleAuthorize("instituicao", "admin")]
        public async Task<IActionResult> AtualizarFotos(AtualizaFotosModel model, IFormFile[] Fotos)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("ApiAjudeiMais");

                using (var formData = new MultipartFormDataContent())
                {
                    formData.Add(new StringContent(model.Instituicao_GUID), "Instituicao_GUID");

                    if (Fotos != null && Fotos.Length > 0)
                    {
                        foreach (var foto in Fotos)
                        {
                            if (foto != null && foto.Length > 0)
                            {
                                var streamContent = new StreamContent(foto.OpenReadStream());
                                streamContent.Headers.ContentType = new MediaTypeHeaderValue(foto.ContentType);
                                formData.Add(streamContent, "Fotos", foto.FileName);
                            }
                        }
                    }

                    var response = await httpClient.PostAsync($"{Assistant.ServerURL()}api/InstituicaoImagem/AtualizarFotos", formData);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToRoute("instituicao-perfil", new { guid = model.Instituicao_GUID, alertType = "success", alertMessage = "Fotos atualizadas com sucesso!" });
                    }
                    else
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(responseContent);

                        var mensagemErro = problemDetails?.Title != null
                            ? $"{problemDetails.Title}: {problemDetails.Detail}"
                            : $"Erro ao atualizar fotos: {responseContent}";

                        return RedirectToRoute("instituicao-perfil",new { guid = model.Instituicao_GUID, alertType = "error", alertMessage = mensagemErro });
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                return RedirectToRoute("instituicao-perfil", new { alertType = "error", alertMessage = $"Não foi possível conectar ao servidor: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return RedirectToRoute("instituicao-perfil", new { alertType = "error", alertMessage = $"Erro inesperado ao atualizar fotos. {ex.Message}" });
            }
        }


        [HttpPost("AtualizarEndereco")]
        [RoleAuthorize("instituicao", "admin")]
        public async Task<IActionResult> AtualizarEndereco(List<EnderecoModel> enderecos)
        {
            if (User.Identity.IsAuthenticated)
            {
                var model = enderecos.FirstOrDefault();
                
                string GUID = model.instituicao_GUID;
                try
                {
                    var httpClient = _httpClientFactory.CreateClient("ApiAjudeiMais");

                    using (var formData = new MultipartFormDataContent())
                    {
                        // Adiciona os dados simples da instituição

                        var json = JsonConvert.SerializeObject(model);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        // Faz a chamada para a API
                        var response = await httpClient.PostAsync($"{Assistant.ServerURL()}api/Endereco/AtualizarEndereco", content);

                        if (response.IsSuccessStatusCode)
                        {
                            return RedirectToRoute($"instituicao-perfil", new { alertType = "success", alertMessage = "Dados da instituição realizado com sucesso." });
                        }
                        else
                        {
                            var responseContent = await response.Content.ReadAsStringAsync();
                            var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(responseContent);

                            if (problemDetails?.Title != null && problemDetails?.Detail != null)
                            {
                                return RedirectToRoute("instituicao-cadastrar", new { alertType = "error", alertMessage = $"{problemDetails.Title}: {problemDetails.Detail}" });
                            }
                            else if (!string.IsNullOrEmpty(responseContent))
                            {
                                return RedirectToRoute("instituicao-cadastrar", new { alertType = "error", alertMessage = $"Erro ao cadastrar: {responseContent}" });
                            }
                            else
                            {
                                return RedirectToRoute("instituicao-cadastrar", new { alertType = "error", alertMessage = "Ocorreu um erro ao cadastrar a instituição." });
                            }
                        }
                    }
                }
                catch (HttpRequestException ex)
                {
                    return RedirectToRoute("instituicao-cadastrar", new { alertType = "error", alertMessage = $"Não foi possível conectar ao servidor: {ex.Message}" });
                }
                catch (Exception ex)
                {
                    return RedirectToRoute("instituicao-cadastrar", new { alertType = "error", alertMessage = $"Ocorreu um erro inesperado durante o cadastro. {ex.Message}" });
                }
            }
            else
            {
                return RedirectToRoute("home");

            }
        }

        [RoleAuthorize("instituicao", "admin")]
        [HttpPost]
        public async Task<IActionResult> ExcluirConta(InstituicaoExcluirContaDTO model) // Use InstituicaoExcluirContaModel
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToRoute("home");

            // Validação de segurança: garantir que o usuário só pode deletar sua própria conta (a menos que seja admin)
            var loggedInUserGuid = Assistant.GetUserGuidFromClaims(User, "GUID");
            if (string.IsNullOrEmpty(loggedInUserGuid) || (!User.IsInRole("admin") && !string.Equals(model.GUID, loggedInUserGuid, StringComparison.OrdinalIgnoreCase)))
            {
                TempData["alertType"] = "error";
                TempData["alertMessage"] = "Você não tem permissão para excluir esta conta.";
                return RedirectToRoute("instituicao-alterar-dados", new { guid = loggedInUserGuid });
            }

            string guid = model.GUID;

            try
            {
                var httpClient = _httpClientFactory.CreateClient("ApiAjudeiMais");

                // 1) Verificar a senha da instituição
                var jsonContentVerify = System.Text.Json.JsonSerializer.Serialize(model);
                var contentVerify = new StringContent(jsonContentVerify, Encoding.UTF8, "application/json");

                // Ajuste o endpoint da API se necessário. No exemplo do usuário, era 'ValidarSenha'.
                var responseVerify = await httpClient.PostAsync($"{Assistant.ServerURL()}api/Instituicao/ValidarSenha", contentVerify);
                var apiResponseContentVerify = await responseVerify.Content.ReadAsStringAsync();

                ApiHelper.ApiResponse<InstituicaoPerfilModel> apiVerifyResponse; // Use InstituicaoPerfilModel aqui

                try
                {
                    apiVerifyResponse = System.Text.Json.JsonSerializer.Deserialize<ApiHelper.ApiResponse<InstituicaoPerfilModel>>(
                        apiResponseContentVerify,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
                }
                catch (System.Text.Json.JsonException jsonEx)
                {
                    _logger?.LogError(jsonEx, "Erro ao desserializar resposta da API (VerificarSenha da Instituicao).");
                    TempData["alertType"] = "error";
                    TempData["alertMessage"] = "Ocorreu um erro no formato da resposta da API ao verificar a senha. Contacte o suporte.";
                    return RedirectToRoute("instituicao-alterar-dados", new { guid = guid });
                }

                if (responseVerify.IsSuccessStatusCode && apiVerifyResponse != null && apiVerifyResponse.Success)
                {
                    // 2) Se a senha estiver ok, prosseguir para a exclusão
                    var responseDelete = await httpClient.DeleteAsync($"{Assistant.ServerURL()}api/Instituicao/{guid}");
                    var apiDeleteContent = await responseDelete.Content.ReadAsStringAsync();

                    ApiHelper.ApiResponse<InstituicaoPerfilModel> apiDeleteResponse;

                    try
                    {
                        apiDeleteResponse = System.Text.Json.JsonSerializer.Deserialize<ApiHelper.ApiResponse<InstituicaoPerfilModel>>(
                            apiDeleteContent,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                        );
                    }
                    catch (System.Text.Json.JsonException jsonEx)
                    {
                        _logger?.LogError(jsonEx, "Erro ao desserializar resposta da API (Excluir Instituicao).");
                        TempData["alertType"] = "error";
                        TempData["alertMessage"] = "Ocorreu um erro no formato da resposta da API ao tentar excluir a conta. Contacte o suporte.";
                        return RedirectToRoute("instituicao-alterar-dados", new { guid = guid });
                    }

                    if (responseDelete.IsSuccessStatusCode && apiDeleteResponse != null && apiDeleteResponse.Success)
                    {
                        await HttpContext.SignOutAsync(); // Desloga a instituição
                        TempData["alertType"] = apiDeleteResponse.Type;
                        TempData["alertMessage"] = apiDeleteResponse.Message;
                        return RedirectToRoute("home"); // Redireciona para a home após a exclusão
                    }
                    else
                    {
                        // Erro ao tentar deletar (APÓS a senha ser verificada)
                        TempData["alertType"] = "error";
                        TempData["alertMessage"] = apiDeleteResponse?.Message ?? "Não foi possível excluir a conta da instituição. Tente novamente.";
                        return RedirectToRoute("instituicao-alterar-dados", new { guid = guid });
                    }
                }
				else // Este é o bloco que você precisa ajustar
				{
					// Senha incorreta ou outra falha na verificação da senha
					string alertType = "error";
					string alertMessage = "Ocorreu um erro na verificação da senha. Tente novamente."; // Mensagem padrão para falha

					// Tente desserializar o erro se a resposta não foi de sucesso, mas contém JSON
					if (responseVerify.Content.Headers.ContentType?.MediaType == "application/json")
					{
						try
						{
							var errorResponse = System.Text.Json.JsonSerializer.Deserialize<ApiHelper.ApiResponse<object>>(
								apiResponseContentVerify,
								new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
							);

							if (errorResponse != null && !string.IsNullOrEmpty(errorResponse.Message))
							{
								alertMessage = errorResponse.Message; // Use a mensagem específica da API
																	  // Se houver Data (erros de validação), você pode formatá-los também
								if (errorResponse.Data is JsonElement dataElement && dataElement.ValueKind == JsonValueKind.Array)
								{
									var validationErrors = dataElement.EnumerateArray()
																	  .Select(e => e.GetString())
																	  .Where(s => !string.IsNullOrEmpty(s))
																	  .ToList();
									if (validationErrors.Any())
									{
										alertMessage += " Detalhes: " + string.Join(", ", validationErrors);
									}
								}
							}
						}
						catch (System.Text.Json.JsonException jsonEx)
						{
							_logger?.LogError(jsonEx, "Erro ao desserializar o BAD REQUEST da API para verificação de senha.");
							alertMessage = "Erro inesperado ao processar a resposta da API de validação de senha.";
						}
					}

					TempData["alertType"] = alertType;
					TempData["alertMessage"] = alertMessage;
					return RedirectToRoute("instituicao-alterar-dados", new { guid = guid });
				}
			}
            catch (HttpRequestException ex)
            {
                _logger?.LogError(ex, "Erro de requisição HTTP ao tentar excluir conta da Instituição.");
                TempData["alertType"] = "error";
                TempData["alertMessage"] = $"Não foi possível conectar ao servidor da API: {ex.Message}";
                return RedirectToRoute("instituicao-alterar-dados", new { guid = guid });
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Erro inesperado ao excluir conta da Instituição.");
                TempData["alertType"] = "error";
                TempData["alertMessage"] = $"Ocorreu um erro inesperado ao excluir a conta da instituição. {ex.Message}";
                return RedirectToRoute("instituicao-alterar-dados", new { guid = guid });
            }
        }

        [RoleAuthorize("instituicao", "admin")]
        [HttpPost]
        public async Task<IActionResult> AtualizarSenha(InstituicaoSenhaModel model)
        {
            if (User.Identity.IsAuthenticated)
            {
                string guid = model.GUID;

                try
                {
                    var httpClient = _httpClientFactory.CreateClient("ApiAjudeiMais");

                    // Use System.Text.Json for serialization
                    var jsonContent = System.Text.Json.JsonSerializer.Serialize(model);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    var response = await httpClient.PostAsync($"{Assistant.ServerURL()}api/Instituicao/AtualizarSenha", content);

                    var apiResponseContent = await response.Content.ReadAsStringAsync();
                    ApiHelper.ApiResponse<InstituicaoPerfilModel> apiResponse = null;

                    try
                    {
                        // CORRECTED: Use System.Text.Json for deserialization
                        apiResponse = System.Text.Json.JsonSerializer.Deserialize<ApiHelper.ApiResponse<InstituicaoPerfilModel>>(
                            apiResponseContent,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                        );
                    }
                    catch (System.Text.Json.JsonException jsonEx) // Catch specific System.Text.JsonException
                    {
                        // Log the deserialization error for debugging
                        _logger?.LogError(jsonEx, "Erro ao desserializar resposta da API em AtualizarSenha.");

                        apiResponse = new ApiHelper.ApiResponse<InstituicaoPerfilModel>
                        {
                            Success = false,
                            Type = "error",
                            Message = "Ocorreu um erro no formato da resposta da API. Contacte o suporte."
                        };
                        TempData["alertType"] = apiResponse.Type;
                        TempData["alertMessage"] = apiResponse.Message;
                        return RedirectToRoute("instituicao-alterar-dados", new { guid = guid });
                    }

                    if (response.IsSuccessStatusCode && apiResponse != null && apiResponse.Success)
                    {
                        TempData["alertType"] = apiResponse.Type;
                        TempData["alertMessage"] = apiResponse.Message;
                        return RedirectToRoute("instituicao-alterar-dados", new { guid = guid });
                    }
                    else
                    {
                        string alertType = "error";
                        string alertMessage = apiResponse?.Message ?? "Não foi possível atualizar a senha. Tente novamente.";
                        TempData["alertType"] = alertType;
                        TempData["alertMessage"] = alertMessage;
                        return RedirectToRoute("instituicao-alterar-dados", new { guid = guid });
                    }
                }
                catch (HttpRequestException ex)
                {
                    _logger?.LogError(ex, "Erro de requisição HTTP ao atualizar senha.");
                    TempData["alertType"] = "error";
                    TempData["alertMessage"] = $"Não foi possível conectar ao servidor da API: {ex.Message}";
                    return RedirectToRoute("instituicao-alterar-dados", new { guid = guid });
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "Erro inesperado ao atualizar senha.");
                    TempData["alertType"] = "error";
                    TempData["alertMessage"] = $"Ocorreu um erro inesperado durante a atualização da senha. {ex.Message}";
                    return RedirectToRoute("instituicao-alterar-dados", new { guid = guid });
                }
            }
            else
            {
                return RedirectToRoute("home");
            }
        }

    }
}
