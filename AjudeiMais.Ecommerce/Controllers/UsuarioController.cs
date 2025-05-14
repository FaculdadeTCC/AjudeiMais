using System.Text;
using System.Threading.Tasks;
using AjudeiMais.Ecommerce.Models;
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
            var httpClient = _httpClientFactory.CreateClient("ApiAjudeiMais");

            var jsonContent = JsonConvert.SerializeObject(model);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("http://localhost:5168/api/Usuario", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Sucesso");
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseContent);  // Imprime o conteúdo para verificar detalhes do erro
                var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(responseContent);  // Deserializa a resposta
                return View(model);  // Retorna a view com o modelo, ou pode exibir a mensagem de erro
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
