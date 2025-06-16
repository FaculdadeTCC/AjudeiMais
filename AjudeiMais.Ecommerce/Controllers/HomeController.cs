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
using AjudeiMais.Ecommerce.Models.Home;
using AjudeiMais.Ecommerce.Tools;
using System.Text.Json;
using AjudeiMais.Ecommerce.Models.Produto;
using AjudeiMais.Ecommerce.Models.Instituicao;
using static AjudeiMais.Ecommerce.Tools.ApiHelper;
using AjudeiMais.Ecommerce.Models.Usuario;
using System.Globalization;

namespace AjudeiMais.Ecommerce.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomeController(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new HomeModel();

            try
            {
                var userGuid = User?.Claims?.FirstOrDefault(c => c.Type == "GUID")?.Value;
                var userRole = User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(userGuid) || string.IsNullOrEmpty(userRole))
                {
                    model.ErrorMessage = "Usuário não autenticado ou sem informações suficientes.";
                    return View(model);
                }

                var client = _httpClientFactory.CreateClient("AjudeiMaisApi");
                HttpResponseMessage response;

                // Faz a chamada correta dependendo da role
                if (userRole == "Usuario")
                {
                    response = await client.GetAsync($"http://localhost:5168/api/Usuario/GetByGUID/{userGuid}");
                }
                else if (userRole == "instituicao")
                {
                    response = await client.GetAsync($"http://localhost:5168/api/Instituicao/GetByGUID/{userGuid}");
                }
                else
                {
                    model.ErrorMessage = "Tipo de usuário não reconhecido.";
                    return View(model);
                }

                if (!response.IsSuccessStatusCode)
                {
                    model.ErrorMessage = "Não foi possível carregar os dados do usuário.";
                    return View(model);
                }

                var content = await response.Content.ReadAsStringAsync();

                double? latitude = null;
                double? longitude = null;

                if (userRole == "Usuario")
                {
                    var usuario = System.Text.Json.JsonSerializer.Deserialize<UsuarioPerfilModel>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    latitude = Convert.ToDouble(usuario?.Latitude);
                    longitude = Convert.ToDouble(usuario?.Longitude);
                }
                else if (userRole == "instituicao")
                {
                    var instituicao = System.Text.Json.JsonSerializer.Deserialize<InstituicaoPerfilModel>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    latitude = Convert.ToDouble(instituicao?.Latitude, CultureInfo.InvariantCulture);
                    longitude = Convert.ToDouble(instituicao?.Longitude, CultureInfo.InvariantCulture);

                }

                // Busca produtos se tiver lat/lng válidos
                if (latitude != null && longitude != null)
                {
                    var (produtos, errorMsg) = await ApiHelper.ListAllProdutosProximosAsync(_httpClientFactory, latitude.Value, longitude.Value);

                    if (produtos != null)
                    {
                        model.Anuncios = produtos;
                    }
                    else
                    {
                        model.ErrorMessage = errorMsg ?? "Não foi possível carregar os produtos próximos.";
                    }
                }

            }
            catch (Exception ex)
            {
                model.ErrorMessage = "Erro ao carregar os dados da página inicial: " + ex.Message;
            }

            return View(model);
        }

        public async Task<IActionResult> AcessoNegado()
        {
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToRoute("home");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult _Alertas()
        {
            return PartialView();
        }
        
        public IActionResult _PopupLocalizacao()
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

