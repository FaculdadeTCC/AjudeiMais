using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AjudeiMais.Ecommerce.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AjudeiMais.Ecommerce.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory; 
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("ApiAjudeiMais");

                httpClient.DefaultRequestHeaders.ConnectionClose = true;

                var jsonContent = JsonConvert.SerializeObject(model);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("http://localhost:5168/api/Auth/login", content); // ajuste a URL se necessário
                if (!response.IsSuccessStatusCode)
                {
                    // Redireciona para a mesma action Login (GET), enviando alerta por query string
                    return RedirectToAction("Login", new
                    {
                        alertType = "error",
                        alertMessage = "E-mail ou senha inválidos."
                    });
                }

                var json = await response.Content.ReadAsStringAsync();
                var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(json);

                // Armazena dados na sessão
                HttpContext.Session.SetString("JwtToken", loginResponse.Token);
                HttpContext.Session.SetString("UserRole", loginResponse.Role);
                HttpContext.Session.SetString("UserId", loginResponse.Id);

                return RedirectToAction("Perfil", "Usuario"); // Redireciona para home após login
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar fazer login");
                TempData["ErroLogin"] = "Erro ao tentar fazer login.";

                return View(model);
            }
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Privacy()
        {   
            return View();
        }

        public IActionResult _Alertas()
        {
            return PartialView();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

}

