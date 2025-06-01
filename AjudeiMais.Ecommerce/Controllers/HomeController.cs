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

namespace AjudeiMais.Ecommerce.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory HttpClientFactory)
        {
            _httpClientFactory = HttpClientFactory;
        }

        // Este m�todo Index agora aceitar� latitude e longitude via query string ou form data
        // Ele precisa ser async porque chamar� uma API de forma ass�ncrona.
        [HttpGet] // Pode ser HttpPost tamb�m, dependendo de como voc� envia os dados do JS
        public async Task<IActionResult> Index(double? lat, double? lng)
        {
            var model = new HomeModel();

            if (lat.HasValue && lng.HasValue)
            {
                // Chamar a API para buscar produtos pr�ximos
                var (produtos, errorMessage) = await ApiHelper.ListAllProdutosProximosAsync(
                    _httpClientFactory, lat.Value, lng.Value);

                if (produtos != null)
                {
                    model.Anuncios = produtos;
                }
                else
                {
                    // Se houver um erro, armazena a mensagem no modelo
                    model.ErrorMessage = errorMessage ?? "N�o foi poss�vel carregar os produtos pr�ximos.";
                }
            }
            else
            {
                // Se latitude e longitude n�o foram fornecidas, voc� pode:
                // 1. Carregar todos os produtos (se a API permitir)
                // 2. Deixar Anuncios como null e exibir uma mensagem na view para o usu�rio permitir a localiza��o
                // 3. Definir uma localiza��o padr�o
                model.ErrorMessage = "Por favor, permita o acesso � sua localiza��o para exibir produtos pr�ximos.";
                // Ou, para uma localiza��o padr�o: 
                // var (produtosPadrao, erroPadrao) = await ApiHelper.ListAllProdutosProximosAsync(_httpClientFactory, -23.5505, -46.6333); // Exemplo: S�o Paulo
                // if (produtosPadrao != null) model.Anuncios = produtosPadrao;
                // else model.ErrorMessage = erroPadrao;
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

		private async Task<(double? lat, double? lng)> ObterLatLngPorCepAsync(string cep)
		{
			var apiKey = "SUA_CHAVE_API_GOOGLE_MAPS";
			var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={cep}&key={apiKey}";

			var httpClient = _httpClientFactory.CreateClient();
			var response = await httpClient.GetAsync(url);
			if (!response.IsSuccessStatusCode) return (null, null);

			var content = await response.Content.ReadAsStringAsync();
			var json = JsonDocument.Parse(content);

			var results = json.RootElement.GetProperty("results");
			if (results.GetArrayLength() == 0) return (null, null);

			var location = results[0].GetProperty("geometry").GetProperty("location");
			var lat = location.GetProperty("lat").GetDouble();
			var lng = location.GetProperty("lng").GetDouble();

			return (lat, lng);
		}
	}

}

