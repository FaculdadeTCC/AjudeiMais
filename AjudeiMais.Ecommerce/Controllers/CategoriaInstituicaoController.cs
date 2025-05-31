using System;
using System.Collections.Generic; // Necessário para List<CategoriaModel>
using System.Net.Http;
using System.Text;
using System.Text.Json; // Usando System.Text.Json para deserialização
using System.Threading.Tasks;
using AjudeiMais.Ecommerce.Filters;
using AjudeiMais.Ecommerce.Models;
using AjudeiMais.Ecommerce.Models.Instituicao; // Para CategoriaModel
using AjudeiMais.Ecommerce.Tools; // Para Assistant.ServerURL()
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
// Se você está usando Newtonsoft.Json em algumas partes, mantenha:
// using Newtonsoft.Json;

namespace AjudeiMais.Ecommerce.Controllers
{
    public class CategoriaInstituicaoController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<CategoriaInstituicaoController> _logger;
        // private readonly string _apiBaseUrl; // Não precisa mais se Assistant.ServerURL() já retornar a base completa

        public CategoriaInstituicaoController(IHttpClientFactory httpClientFactory, ILogger<CategoriaInstituicaoController> logger) // Removido parametro opcional para logger, pois é injetado
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            // Se você já tem Assistant.ServerURL() configurado no Tools, não precisa desta linha:
            // _apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? throw new InvalidOperationException("ApiSettings:BaseUrl não configurado.");
        }

        // --- Ações do Controlador ---

        /// <summary>
        /// Exibe a página de gerenciamento de categorias, carregando todas as categorias da API.
        /// </summary>
        /// <returns>A View com a lista de categorias ou uma lista vazia em caso de erro.</returns>
        [RoleAuthorize("admin")]
        [HttpGet] // Adicionei o HttpGet explicitamente, boa prática
        public async Task<IActionResult> Index()
        {
            List<CategoriaDtoGet> categorias = new List<CategoriaDtoGet>();
            string errorMessage = null;

            try
            {
                // CORRECTED CALL: Remove Assistant.ServerURL() as it's not expected
                var (apiResponse, errorMsg) = await ApiHelper.ListAllCategoriasAsync(_httpClientFactory);

                // --- IMPORTANT ---
                // The ApiHelper.ListAllCategoriasAsync returns (List<CategoriaDtoGet>? categorias, string? errorMessage)
                // It does NOT return an ApiResponse<T>. So your 'if' condition logic needs to change.
                // It directly returns the list of DTOs or null.

                if (apiResponse != null) // If apiResponse (which is List<CategoriaDtoGet>) is not null, it means data was returned.
                {
                    // No need for apiResponse.Success or apiResponse.Data, as apiResponse IS the data.
                    categorias = apiResponse; // Assign the directly returned list
                }
                else
                {
                    // If apiResponse is null, then an error occurred and errorMsg will contain the details.
                    errorMessage = errorMsg ?? "Erro desconhecido ao carregar categorias.";
                    _logger?.LogError("Erro ao obter lista de categorias: {ErrorMessage}", errorMessage);
                    TempData["alertType"] = "error";
                    TempData["alertMessage"] = errorMessage;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Exceção inesperada ao tentar obter categorias.");
                errorMessage = "Ocorreu um erro inesperado ao carregar as categorias. Tente novamente mais tarde.";
                TempData["alertType"] = "error";
                TempData["alertMessage"] = errorMessage;
            }

            return View(categorias);
        }

        //[RoleAuthorize("admin")]
        //[HttpGet]
        //public async Task<IActionResult> AlterarDados(int id)
        //{
        //    if (id <= 0)
        //    {
        //        TempData["alertType"] = "error";
        //        TempData["alertMessage"] = "ID da categoria não fornecido.";
        //        return RedirectToAction("Index");
        //    }

        //    var (apiResponse, errorMessage) = await ApiHelper.GetCategoriaByIdAsync(_httpClientFactory, Assistant.ServerURL(), id);

        //    if (apiResponse != null && apiResponse.Success && apiResponse.Data != null)
        //    {
        //        Mapeia o DTO da API para o CategoriaModel da View
        //        var categoriaModel = new CategoriaDtoGet
        //        {
        //            Categoria_ID = apiResponse.Data.Categoria_ID,
        //            Nome = apiResponse.Data.Nome,
        //            Habilitado = apiResponse.Data.Habilitado,
        //            Excluido = apiResponse.Data.Excluido,
        //            Icone = apiResponse.Data.Icone
        //        };
        //        return View(categoriaModel);
        //    }
        //    else
        //    {
        //        errorMessage ??= "Categoria não encontrada ou erro na API."; // Define msg se for nula
        //        _logger?.LogError("Erro ao obter dados da categoria {Id}: {ErrorMessage}", id, errorMessage);
        //        TempData["alertType"] = "error";
        //        TempData["alertMessage"] = errorMessage;
        //        return RedirectToAction("Index");
        //    }
        //}

        [RoleAuthorize("admin")]
        [HttpGet]
        public IActionResult Adicionar()
        {
            return View();
        }

        /// <summary>
        /// Adiciona uma nova categoria via API.
        /// </summary>
        /// <param name="model">Objeto com os dados da categoria a serem adicionados.</param>
        /// <returns>Redireciona para a página de listagem de categorias com mensagem de sucesso ou erro.</returns>
        /// 
        [RoleAuthorize("admin")]
        [HttpPost]
        public async Task<IActionResult> Adicionar(CategoriaModel model)
        {
            //Validação de modelo no lado do cliente(opcional, mas boa prática)
             if (!ModelState.IsValid)
             {
                TempData["alertType"] = "error";
                TempData["alertMessage"] = "Dados inválidos. Por favor, verifique os campos.";
                return View(model);
             }

            try
            {
                //Converte CategoriaModel para CategoriaDTO para enviar à API
                var categoriaDto = new CategoriaModel// Use o namespace completo do DTO da API
                {
                    Categoria_ID = model.Categoria_ID, // Será 0 para nova categoria
                    Nome = model.Nome,
                    Habilitado = model.Habilitado,
                    Icone = model.Icone // Se o ícone for uma string (URL ou classe CSS)
                    // Não há propriedade para IFormFile aqui, se o ícone for upload, a lógica abaixo precisa mudar.
                };

                //No seu Controller da API, o SaveOrUpdate espera JSON com[FromBody]
                // Portanto, precisamos enviar um JSON, não MultipartFormDataContent

                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(categoriaDto, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                    Encoding.UTF8,
                    "application/json");

                var httpClient = _httpClientFactory.CreateClient("ApiAjudeiMais"); // Recomenda-se configurar um Named Client no Program.cs
                var response = await httpClient.PostAsync($"{Tools.Assistant.ServerURL()}api/Categoria", jsonContent);

                var apiResponseContent = await response.Content.ReadAsStringAsync();
                var apiResponse = System.Text.Json.JsonSerializer.Deserialize<ApiHelper.ApiResponse<object>>( // Pode ser CategoriaDTO se a API retornar o objeto
                    apiResponseContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                if (response.IsSuccessStatusCode && apiResponse != null && apiResponse.Success)
                {
                    TempData["alertType"] = apiResponse.Type;
                    TempData["alertMessage"] = apiResponse.Message;
                    return RedirectToAction("Index"); // Redireciona para o Index após adicionar
                }
                else
                {
                    string alertType = apiResponse?.Type ?? "error";
                    string alertMessage = apiResponse?.Message ?? "Ocorreu um erro ao cadastrar a categoria.";
                    _logger?.LogError("Erro ao adicionar categoria: {Message}. Detalhes: {Content}", alertMessage, apiResponseContent);
                    TempData["alertType"] = alertType;
                    TempData["alertMessage"] = alertMessage;
                    return View(model); // Permanece na view de adicionar com os dados e erros
                }
            }
            catch (HttpRequestException ex)
            {
                _logger?.LogError(ex, "Erro de conexão ao adicionar categoria.");
                TempData["alertType"] = "error";
                TempData["alertMessage"] = $"Não foi possível conectar ao servidor da API: {ex.Message}";
                return View(model);
            }
            catch (JsonException ex)
            {
                _logger?.LogError(ex, "Erro de deserialização JSON ao adicionar categoria.");
                TempData["alertType"] = "error";
                TempData["alertMessage"] = $"Erro no formato da resposta da API ao adicionar: {ex.Message}";
                return View(model);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Ocorreu um erro inesperado ao adicionar categoria.");
                TempData["alertType"] = "error";
                TempData["alertMessage"] = $"Ocorreu um erro inesperado durante o cadastro. {ex.Message}";
                return View(model);
            }
        }

        /// <summary>
        /// Atualiza os dados de uma categoria existente via API.
        /// </summary>
        /// <param name="model">Objeto com os dados da categoria a serem atualizados.</param>
        /// <returns>Redireciona para a página de alteração de dados com mensagem de sucesso ou erro.</returns>
        [RoleAuthorize("admin")]
        [HttpPost]
        public async Task<IActionResult> AtualizarDados(CategoriaModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["alertType"] = "error";
                TempData["alertMessage"] = "Dados inválidos. Por favor, verifique os campos.";
                return View("AlterarDados", model);
            }

            try
            {
                // Converte CategoriaModel para CategoriaDTO para enviar à API
                var categoriaDto = new CategoriaModel
				{
                    Categoria_ID = model.Categoria_ID,
                    Nome = model.Nome,
                    Habilitado = model.Habilitado,
                    Icone = model.Icone
                };

                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(categoriaDto, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
                    Encoding.UTF8,
                    "application/json");

                var httpClient = _httpClientFactory.CreateClient("ApiAjudeiMais");
                // A requisição POST para SaveOrUpdate na API não precisa do ID na URL se o ID estiver no corpo
                // Se a sua API espera PUT para atualização com ID na URL, você pode mudar para .PutAsync
                var response = await httpClient.PostAsync($"{Tools.Assistant.ServerURL()}api/Categoria", jsonContent);

                var apiResponseContent = await response.Content.ReadAsStringAsync();
                ApiHelper.ApiResponse<object> apiResponse = null;

                try
                {
                    apiResponse = JsonSerializer.Deserialize<ApiHelper.ApiResponse<object>>(
                        apiResponseContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
                }
                catch (JsonException jsonEx)
                {
                    _logger?.LogError(jsonEx, "Erro de deserialização da API ao atualizar categoria: {ApiResponseContent}", apiResponseContent);
                    TempData["alertType"] = "error";
                    TempData["alertMessage"] = "Ocorreu um erro no formato da resposta da API. Contacte o suporte.";
                    return View("AlterarDados", model);
                }

                if (response.IsSuccessStatusCode && apiResponse != null && apiResponse.Success)
                {
                    TempData["alertType"] = apiResponse.Type;
                    TempData["alertMessage"] = apiResponse.Message;
                    return RedirectToAction("Index"); // Redireciona para o Index após a atualização
                }
                else
                {
                    string alertType = apiResponse?.Type ?? "error";
                    string alertMessage = apiResponse?.Message ?? "Não foi possível atualizar a categoria. Tente novamente.";
                    _logger?.LogError("Falha ao atualizar categoria {Id}: {Message}. Detalhes: {Content}", model.Categoria_ID, alertMessage, apiResponseContent);
                    TempData["alertType"] = alertType;
                    TempData["alertMessage"] = alertMessage;
                    return View("AlterarDados", model);
                }
            }
            catch (HttpRequestException ex)
            {
                _logger?.LogError(ex, "Erro de conexão ao atualizar categoria {Id}.", model.Categoria_ID);
                TempData["alertType"] = "error";
                TempData["alertMessage"] = $"Não foi possível conectar ao servidor da API: {ex.Message}";
                return View("AlterarDados", model);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Erro inesperado ao atualizar categoria {Id}.", model.Categoria_ID);
                TempData["alertType"] = "error";
                TempData["alertMessage"] = $"Ocorreu um erro inesperado durante a atualização da categoria. {ex.Message}";
                return View("AlterarDados", model);
            }
        }

        /// <summary>
        /// Exclui uma categoria via API.
        /// </summary>
        /// <param name="id">O ID da categoria a ser excluída.</param>
        /// <returns>Redireciona para a página de listagem de categorias com mensagem de sucesso ou erro.</returns>
        [RoleAuthorize("admin")]
        [HttpPost]
        public async Task<IActionResult> Excluir(int id)
        {
            if (id <= 0)
            {
                TempData["alertType"] = "error";
                TempData["alertMessage"] = "ID da categoria não fornecido para exclusão.";
                return RedirectToAction("Index");
            }

            try
            {
                var httpClient = _httpClientFactory.CreateClient("ApiAjudeiMais");
                var response = await httpClient.DeleteAsync($"{Tools.Assistant.ServerURL()}api/Categoria/{id}");

                var apiResponseContent = await response.Content.ReadAsStringAsync();
                ApiHelper.ApiResponse<object> apiResponse = null;

                try
                {
                    apiResponse = JsonSerializer.Deserialize<ApiHelper.ApiResponse<object>>(
                        apiResponseContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
                }
                catch (JsonException jsonEx)
                {
                    _logger?.LogError(jsonEx, "Erro de deserialização da API ao excluir categoria: {ApiResponseContent}", apiResponseContent);
                    TempData["alertType"] = "error";
                    TempData["alertMessage"] = "Ocorreu um erro no formato da resposta da API ao excluir. Contacte o suporte.";
                    return RedirectToAction("Index");
                }

                if (response.IsSuccessStatusCode && apiResponse != null && apiResponse.Success)
                {
                    TempData["alertType"] = apiResponse.Type;
                    TempData["alertMessage"] = apiResponse.Message;
                    return RedirectToAction("Index");
                }
                else
                {
                    string alertType = apiResponse?.Type ?? "error";
                    string alertMessage = apiResponse?.Message ?? "Não foi possível excluir a categoria. Tente novamente.";
                    _logger?.LogError("Falha ao excluir categoria {Id}: {Message}. Detalhes: {Content}", id, alertMessage, apiResponseContent);
                    TempData["alertType"] = alertType;
                    TempData["alertMessage"] = alertMessage;
                    return RedirectToAction("Index");
                }
            }
            catch (HttpRequestException ex)
            {
                _logger?.LogError(ex, "Erro de conexão ao excluir categoria {Id}.", id);
                TempData["alertType"] = "error";
                TempData["alertMessage"] = $"Não foi possível conectar ao servidor da API: {ex.Message}";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Erro inesperado ao excluir categoria {Id}.", id);
                TempData["alertType"] = "error";
                TempData["alertMessage"] = $"Ocorreu um erro inesperado durante a exclusão da categoria. {ex.Message}";
                return RedirectToAction("Index");
            }
        }
    }
}