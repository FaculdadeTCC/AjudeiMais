using AjudeiMais.Ecommerce.Filters;
using AjudeiMais.Ecommerce.Models;
using AjudeiMais.Ecommerce.Tools;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Reflection;
using System.Text;

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
                    var response = await httpClient.PostAsync("http://localhost:5168/api/Instituicao", formData);

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
        public IActionResult AlterarDados()
        {
            return View();
        }
    }
}
