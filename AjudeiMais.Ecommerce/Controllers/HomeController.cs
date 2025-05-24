using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AjudeiMais.Ecommerce.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace AjudeiMais.Ecommerce.Controllers
{
    public class HomeController : Controller
    {
        string BASE_URL = Tools.Assistant.ServerURL();
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

        /// <summary>
        /// Realiza a autenticação do usuário com base nas credenciais informadas.
        /// Envia os dados para a API de autenticação e, se bem-sucedido, cria a identidade com cookies e claims.
        /// </summary>
        /// <param name="model">Modelo com os dados de login (e-mail e senha).</param>
        /// <returns>
        /// Redireciona o usuário para a tela correspondente ao seu perfil (Admin, Instituição, Usuário)
        /// ou retorna para a tela de login em caso de erro.
        /// </returns>
        /// <remarks>
        /// Esse método utiliza autenticação baseada em cookie e armazena dados relevantes em claims.
        /// Também utiliza sessão para armazenar o token JWT e outros dados auxiliares.
        /// </remarks>
        /// <exception cref="Exception">Captura erros de comunicação com a API ou falhas inesperadas.</exception>
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("ApiAjudeiMais");

                httpClient.DefaultRequestHeaders.ConnectionClose = true;

                var jsonContent = JsonConvert.SerializeObject(model);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync($"{BASE_URL}api/Auth/login", content);
                if (!response.IsSuccessStatusCode)
                {
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
                HttpContext.Session.SetString("GUID", loginResponse.GUID);

                var usuario = await httpClient.GetAsync($"{BASE_URL}api/Usuario/GetByGUID/{loginResponse.GUID}");
                json = await usuario.Content.ReadAsStringAsync();

                var usuarioResponse = JsonConvert.DeserializeObject<UsuarioPerfilModel>(json);

                // Cria as claims do usuário autenticado
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, usuarioResponse.NomeCompleto),
            new Claim(ClaimTypes.Role, loginResponse.Role),
            new Claim("UserId", loginResponse.Id),
            new Claim("GUID", loginResponse.GUID),
        };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                // Redireciona com base no perfil (role)
                switch (loginResponse.Role.ToLower())
                {
                    case "admin":
                        return RedirectToAction("Index", "Admin");
                    case "instituicao":
                        return RedirectToAction("Perfil", "Instituicao");
                    case "usuario":
                        return RedirectToRoute("usuario-perfil", new { guid = loginResponse.GUID.ToString() });
                    default:
                        return RedirectToAction("AcessoNegado", "Home");
                }
            }
            catch (Exception ex)
            {
                return RedirectToRoute("login", new { alertType = "error", alertMessage = ex.Message });

            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToRoute("home");
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult AcessoNegado()
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

