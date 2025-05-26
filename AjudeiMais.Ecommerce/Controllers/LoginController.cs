using AjudeiMais.Ecommerce.Models;
using AjudeiMais.Ecommerce.Models.Usuario;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Security.Claims;
using System.Text;

namespace AjudeiMais.Ecommerce.Controllers
{
    public class LoginController : Controller
    {
        string BASE_URL = Tools.Assistant.ServerURL();
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HomeController> _logger;

        public LoginController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Login()
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
            if (!User.Identity.IsAuthenticated)
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
                            alertMessage = response.ReasonPhrase
                        });
                    }

                    var json = await response.Content.ReadAsStringAsync();
                    var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(json);

                    // Armazena dados na sessão
                    HttpContext.Session.SetString("JwtToken", loginResponse.Token);
                    HttpContext.Session.SetString("UserRole", loginResponse.Role);
                    HttpContext.Session.SetString("UserId", loginResponse.Id);
                    HttpContext.Session.SetString("GUID", loginResponse.GUID);

                    var role = User.FindFirstValue(ClaimTypes.Role);

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
            } else
            {
                return RedirectToRoute("usuario-perfil", new { guid = User.FindFirst("GUID")?.Value });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToRoute("home");
        }

    }
}
