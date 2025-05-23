using System.Text;
using System.Threading.Tasks;
using AjudeiMais.Ecommerce.Filters;
using AjudeiMais.Ecommerce.Models;
using AjudeiMais.Ecommerce.Tools;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AjudeiMais.Ecommerce.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        UsuarioModel model = new UsuarioModel();

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
            model.Role = "Usuario";
            try
            {
                var httpClient = _httpClientFactory.CreateClient("ApiAjudeiMais");

                var response = await httpClient.GetAsync($"http://localhost:5168/api/Usuario/GetByGUID/{guid}");

                if (response.IsSuccessStatusCode)
                {
                    // 1. Lê o conteúdo do corpo da resposta como uma string (JSON)
                    var json = await response.Content.ReadAsStringAsync();

                    // 2. Desserializa a string JSON para o seu objeto UsuarioModel
                    var usuario = JsonConvert.DeserializeObject <UsuarioPerfilModel>(json);

                    return View(usuario);
                }
                else
                {
                    // Tenta ler e interpretar os detalhes do problema retornados pela API
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(responseContent);

                    if (problemDetails?.Title != null && problemDetails?.Detail != null)
                    {
                        return RedirectToRoute("usuario-cadastrar", new { alertType = "error", alertMessage = $"{problemDetails.Title}: {problemDetails.Detail}" });
                    }
                    else if (!string.IsNullOrEmpty(responseContent))
                    {
                        return RedirectToRoute("usuario-cadastrar", new { alertType = "error", alertMessage = $"Erro ao buscar usuário: {responseContent}" });
                    }
                    else
                    {
                        return RedirectToRoute("usuario-cadastrar", new { alertType = "error", alertMessage = "Ocorreu um erro ao buscar usuário." });
                    }
                }
            }
            catch (Exception ex)
            {
                // Trata erros inesperados no processo de cadastro
                return RedirectToRoute("usuario-cadastrar", new { alertType = "error", alertMessage = $"Ocorreu um erro inesperado. {ex.Message}" });
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
                    var response = await httpClient.PostAsync("http://localhost:5168/api/Usuario", formData);

                    if (response.IsSuccessStatusCode)
                    {
                        // Cadastro realizado com sucesso, redireciona para login com mensagem de sucesso
                        return RedirectToRoute("login", new { alertType = "success", alertMessage = "Cadastro realizado com sucesso. Faça o Login." });
                    }
                    else
                    {
                        // Tenta ler e interpretar os detalhes do problema retornados pela API
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(responseContent);

                        if (problemDetails?.Title != null && problemDetails?.Detail != null)
                        {
                            return RedirectToRoute("usuario-cadastrar", new { alertType = "error", alertMessage = $"{problemDetails.Title}: {problemDetails.Detail}" });
                        }
                        else if (!string.IsNullOrEmpty(responseContent))
                        {
                            return RedirectToRoute("usuario-cadastrar", new { alertType = "error", alertMessage = $"Erro ao cadastrar: {responseContent}" });
                        }
                        else
                        {
                            return RedirectToRoute("usuario-cadastrar", new { alertType = "error", alertMessage = "Ocorreu um erro ao cadastrar." });
                        }
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                // Trata erros de conexão com o servidor
                return RedirectToRoute("usuario-cadastrar", new { alertType = "error", alertMessage = $"Não foi possível conectar ao servidor: {ex.Message}" });
            }
            catch (Exception ex)
            {
                // Trata erros inesperados no processo de cadastro
                return RedirectToRoute("usuario-cadastrar", new { alertType = "error", alertMessage = $"Ocorreu um erro inesperado durante o cadastro. {ex.Message}" });
            }
        }




        public IActionResult Anuncios()
        {
            return View();
        }

        public IActionResult AlterarDados()
        {
            return View();
        }
    }
}