using AjudeiMais.Ecommerce.Filters;
using AjudeiMais.Ecommerce.Models;
using AjudeiMais.Ecommerce.Tools;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Reflection;

namespace AjudeiMais.Ecommerce.Controllers
{
    public class InstituicaoController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public InstituicaoController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }
        [RoleAuthorize("admin", "instituicao")]
        public IActionResult Perfil()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Cadastro(InstituicaoModel model, List<IFormFile> Fotos)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("ApiAjudeiMais");

                using (var formData = new MultipartFormDataContent())
                {
                    // Adiciona os dados simples da instituição
                    formData.AddObjectAsFormFields(model);

                    // Adiciona a role
                    formData.Add(new StringContent("instituicao"), "Role");

                    // Adiciona as fotos, se houver
                    if (Fotos != null && Fotos.Count > 0)
                    {
                        for (int i = 0; i < Fotos.Count; i++)
                        {
                            var foto = Fotos[i];
                            if (foto != null && foto.Length > 0)
                            {
                                // Usa um nome como Fotos[0], Fotos[1], etc.
                                formData.AddFileContent(foto, $"Fotos[{i}]");
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
