using System.Text;
using System.Threading.Tasks;
using AjudeiMais.Ecommerce.Models;
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

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public IActionResult Perfil()
        {
            return View();
        }

        public IActionResult Cadastro()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Cadastro(UsuarioModel model)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("ApiAjudeiMais");
                var jsonContent = JsonConvert.SerializeObject(model);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("http://localhost:5168/api/Usuario", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToRoute("usuario-cadastrar", new { alertType = "error", alertMessage = "Ocorreu um erro ao cadastrar." });
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseContent);
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
            catch (HttpRequestException ex)
            {
                return RedirectToRoute("usuario-cadastrar", new { alertType = "error", alertMessage = $"Não foi possível conectar ao servidor: {ex.Message}" });
            }
            catch (Exception ex)
            {
                return RedirectToRoute("usuario-cadastrar", new { alertType = "error", alertMessage = "Ocorreu um erro inesperado durante o cadastro." });
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
