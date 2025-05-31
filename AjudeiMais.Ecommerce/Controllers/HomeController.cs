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

namespace AjudeiMais.Ecommerce.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory HttpClientFactory)
        {
            _httpClientFactory = HttpClientFactory;
        }

        // Este método Index agora aceitará latitude e longitude via query string ou form data
        // Ele precisa ser async porque chamará uma API de forma assíncrona.
        [HttpGet] // Pode ser HttpPost também, dependendo de como você envia os dados do JS
        public async Task<IActionResult> Index(double? lat, double? lng)
        {
            var model = new HomeModel();

            if (lat.HasValue && lng.HasValue)
            {
                // Chamar a API para buscar produtos próximos
                var (produtos, errorMessage) = await ApiHelper.ListAllProdutosProximosAsync(
                    _httpClientFactory, lat.Value, lng.Value);

                if (produtos != null)
                {
                    model.Anuncios = produtos;
                }
                else
                {
                    // Se houver um erro, armazena a mensagem no modelo
                    model.ErrorMessage = errorMessage ?? "Não foi possível carregar os produtos próximos.";
                }
            }
            else
            {
                // Se latitude e longitude não foram fornecidas, você pode:
                // 1. Carregar todos os produtos (se a API permitir)
                // 2. Deixar Anuncios como null e exibir uma mensagem na view para o usuário permitir a localização
                // 3. Definir uma localização padrão
                model.ErrorMessage = "Por favor, permita o acesso à sua localização para exibir produtos próximos.";
                // Ou, para uma localização padrão: 
                // var (produtosPadrao, erroPadrao) = await ApiHelper.ListAllProdutosProximosAsync(_httpClientFactory, -23.5505, -46.6333); // Exemplo: São Paulo
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
    }

}

