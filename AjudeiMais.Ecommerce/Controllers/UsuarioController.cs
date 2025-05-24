using System.Net;
using System.Text;
using System.Text.Json; // Já está sendo usado, mas vou reforçar para consistência
using System.Threading.Tasks;
using AjudeiMais.Ecommerce.Filters;
using AjudeiMais.Ecommerce.Models;
using AjudeiMais.Ecommerce.Tools; // Adicionado para acessar ApiHelper
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
// using Newtonsoft.Json; // Se System.Text.Json for suficiente, você pode remover este

namespace AjudeiMais.Ecommerce.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public UsuarioController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [RoleAuthorize("admin")]
        public async Task<IActionResult> Index()
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

            // Usando o método estático para obter o usuário
            var (usuario, errorMessage) = await ApiHelper.GetUsuarioByGuidAsync(_httpClientFactory, guid);

            if (usuario != null)
            {
                return View(usuario);
            }
            else
            {
                return RedirectToRoute("login", new { alertType = "error", alertMessage = errorMessage });
            }
        }

        public IActionResult Cadastro()
        {
            return View();
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
            } else
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
                // Usando Assistant.ServerURL() diretamente aqui, ou pode mover a BASE_URL para ApiHelper se for o único lugar
                var API_URL = $"{Tools.Assistant.ServerURL()}api/Usuario/GetByEmail/{email}";


                var response = await httpClient.GetAsync(API_URL);

                if (response.IsSuccessStatusCode)
                {
                    if (response.Content.Headers.ContentLength > 0)
                    {
                        return Json(new { exists = true, message = "Este e-mail já está cadastrado." });
                    }
                    else
                    {
                        return Json(new { exists = false });
                    }
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return Json(new { exists = false });
                }
                else
                {
                    return Json(new { exists = false });
                }
            }
            catch (Exception ex)
            {
                return Json(new { exists = false, message = ex.Message });
            }
        }

        public IActionResult Anuncios()
        {
            return View();
        }

        public async Task<IActionResult> AlterarDados(string guid)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToRoute("home");
            }

            // Usando o método estático para obter o usuário
            var (usuario, errorMessage) = await ApiHelper.GetUsuarioByGuidAsync(_httpClientFactory, guid);

            if (usuario != null)
            {
                return View(usuario);
            }
            else
            {
                return RedirectToRoute("usuario-perfil", new { alertType = "error", alertMessage = errorMessage, guid = guid });
            }
        }
    }
}