using System; // Adicionado para Exception e TimeSpan
using System.Collections.Generic; // Necessário para List<CategoriaModel>
using System.Net;
using System.Net.Http; // Adicionado para HttpRequestException
using System.Text.Json; // Preferir System.Text.Json para .NET Core
using System.Threading.Tasks;
// using Newtonsoft.Json; // Manter se ainda usar em outros lugares, mas System.Text.Json é o padrão moderno
using Microsoft.AspNetCore.Mvc; // Para ProblemDetails
using AjudeiMais.Ecommerce.Models.Usuario;
using AjudeiMais.Ecommerce.Models.Instituicao; // Para CategoriaModel e InstituicaoPerfilModel

namespace AjudeiMais.Ecommerce.Tools
{
    public static class ApiHelper
    {
        private static string BASE_URL = Assistant.ServerURL(); // Usando o Helper existente

        public class ApiResponse<T>
        {
            public bool Success { get; set; }
            public string Type { get; set; } // "success", "error", "warning", etc.
            public string Message { get; set; }
            public T? Data { get; set; }
        }

        /// <summary>
        /// Busca todos as categorias da API.
        /// </summary>
        /// <param name="httpClientFactory">Factory para criar instâncias de HttpClient.</param>
        /// <returns>Uma tupla contendo uma lista de CategoriaModel se encontrado, e uma mensagem de erro caso contrário.</returns>
        public static async Task<(List<CategoriaDtoGet>? categorias, string? errorMessage)> ListAllCategoriasAsync(
            IHttpClientFactory httpClientFactory)
        {
            try
            {
                var client = httpClientFactory.CreateClient("ApiAjudeiMais"); // Usa o named client
                string requestUri = $"{BASE_URL}api/Categoria"; // Endpoint para listar todas as categorias

                var response = await client.GetAsync(requestUri);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // A API pode retornar diretamente a lista ou encapsulada em um ApiResponse.
                    // Assumindo que o endpoint /api/Categoria retorna diretamente List<CategoriaModel> ou similar.
                    // Se a API retornar um ApiResponse<List<CategoriaModel>>, a linha abaixo precisaria ser ajustada.
                    var categorias = JsonSerializer.Deserialize<List<CategoriaDtoGet>>(
                        content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    return (categorias, null);
                }
                else
                {
                    // Erro HTTP (4xx, 5xx)
                    ProblemDetails? problemDetails = null;
                    try
                    {
                        problemDetails = JsonSerializer.Deserialize<ProblemDetails>(
                            content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    }
                    catch (JsonException)
                    {
                        // Não é um ProblemDetails, usa o conteúdo bruto como mensagem de erro
                    }

                    string errorMsg = problemDetails?.Detail ?? problemDetails?.Title ?? content;
                    if (string.IsNullOrWhiteSpace(errorMsg))
                    {
                        errorMsg = $"Erro ao listar categorias: {response.ReasonPhrase} (Status: {response.StatusCode})";
                    }
                    return (null, errorMsg);
                }
            }
            catch (HttpRequestException httpEx)
            {
                // Erros de rede ou conexão
                return (null, $"Não foi possível conectar ao servidor da API: {httpEx.Message}. Verifique se a API está online.");
            }
            catch (JsonException jsonEx)
            {
                // Erros de desserialização JSON
                return (null, $"Erro de processamento da resposta da API (JSON inválido) ao listar categorias: {jsonEx.Message}.");
            }
            catch (Exception ex)
            {
                // Quaisquer outros erros inesperados
                return (null, $"Ocorreu um erro inesperado ao listar categorias: {ex.Message}");
            }
        }

        // Método estático para buscar usuário por GUID
        public static async Task<(UsuarioPerfilModel? Usuario, string? ErrorMessage)> GetUsuarioByGuidAsync(
            IHttpClientFactory httpClientFactory, string guid)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return (null, "GUID do usuário não fornecido.");
            }

            try
            {
                var httpClient = httpClientFactory.CreateClient("ApiAjudeiMais");
                var response = await httpClient.GetAsync($"{BASE_URL}api/Usuario/GetByGUID/{guid}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var usuario = System.Text.Json.JsonSerializer.Deserialize<UsuarioPerfilModel>(json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return (usuario, null); // Retorna o usuário e null para a mensagem de erro
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    ProblemDetails? problemDetails = null;

                    try
                    {
                        problemDetails = System.Text.Json.JsonSerializer.Deserialize<ProblemDetails>(responseContent,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    }
                    catch (System.Text.Json.JsonException)
                    {
                        // Se não for ProblemDetails, apenas usa o conteúdo bruto como mensagem
                    }

                    if (problemDetails?.Title != null && problemDetails?.Detail != null)
                    {
                        return (null, $"{problemDetails.Title}: {problemDetails.Detail}");
                    }
                    else if (!string.IsNullOrEmpty(responseContent))
                    {
                        return (null, $"Erro ao buscar usuário: {responseContent}");
                    }
                    else
                    {
                        return (null, "Ocorreu um erro desconhecido ao buscar usuário.");
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                return (null, $"Não foi possível conectar ao servidor da API: {ex.Message}");
            }
            catch (Exception ex)
            {
                return (null, $"Ocorreu um erro inesperado: {ex.Message}");
            }
        }

        public static async Task<(InstituicaoPerfilModel? instituicao, string? ErrorMessage)> GetInsituicaoByGuidAsync(IHttpClientFactory httpClientFactory, string guid)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return (null, "GUID do Instituição não fornecido.");
            }

            try
            {
                var httpClient = httpClientFactory.CreateClient("ApiAjudeiMais");
                var response = await httpClient.GetAsync($"{BASE_URL}api/Instituicao/GetByGUID/{guid}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    // Usar System.Text.Json.JsonSerializer.Deserialize para desserialização
                    var instituicao = System.Text.Json.JsonSerializer.Deserialize<InstituicaoPerfilModel>(json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return (instituicao, null); // Retorna o usuário e null para a mensagem de erro
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    ProblemDetails? problemDetails = null;

                    // Tenta desserializar como ProblemDetails
                    try
                    {
                        problemDetails = System.Text.Json.JsonSerializer.Deserialize<ProblemDetails>(responseContent,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    }
                    catch (System.Text.Json.JsonException)
                    {
                        // Se não for ProblemDetails, apenas usa o conteúdo bruto como mensagem
                    }

                    if (problemDetails?.Title != null && problemDetails?.Detail != null)
                    {
                        return (null, $"{problemDetails.Title}: {problemDetails.Detail}");
                    }
                    else if (!string.IsNullOrEmpty(responseContent))
                    {
                        return (null, $"Erro ao buscar instituição: {responseContent}");
                    }
                    else
                    {
                        return (null, "Ocorreu um erro desconhecido ao buscar instituição.");
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                return (null, $"Não foi possível conectar ao servidor da API: {ex.Message}");
            }
            catch (Exception ex)
            {
                return (null, $"Ocorreu um erro inesperado: {ex.Message}");
            }

        }

        /// <summary>
        /// Obtém os detalhes de um produto específico da API usando seu ID.
        /// </summary>
        /// <param name="httpClientFactory">Factory para criar instâncias de HttpClient.</param>
        /// <param name="id">O ID numérico do produto a ser buscado.</param>
        /// <returns>Uma tupla contendo o ProdutoModel se encontrado, e uma mensagem de erro caso contrário.</returns>
        public static async Task<(CategoriaModel? categoria, string? ErrorMessage)> GetCategoriaByIdAsync(
            IHttpClientFactory httpClientFactory, int id)
        {
            // Valida se o ID é maior que zero (um ID válido)
            if (id <= 0)
            {
                return (null, "ID do produto inválido.");
            }

            try
            {
                var httpClient = httpClientFactory.CreateClient("ApiAjudeiMais");
                // Modifique a URL para usar o ID no lugar do GUID, e o endpoint adequado
                var response = await httpClient.GetAsync($"{BASE_URL}api/Categoria/GetByID/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    // Desserializa a resposta para o seu ProdutoModel
                    var produto = System.Text.Json.JsonSerializer.Deserialize<CategoriaModel>(json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return (produto, null); // Retorna o produto e null para a mensagem de erro
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    ProblemDetails? problemDetails = null;

                    // Tenta desserializar como ProblemDetails (comum em APIs .NET Core para erros HTTP)
                    try
                    {
                        problemDetails = System.Text.Json.JsonSerializer.Deserialize<ProblemDetails>(responseContent,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    }
                    catch (System.Text.Json.JsonException)
                    {
                        // Ignora se não for ProblemDetails, apenas usa o conteúdo bruto como mensagem
                    }

                    if (problemDetails?.Title != null && problemDetails?.Detail != null)
                    {
                        // Retorna a mensagem de erro do ProblemDetails
                        return (null, $"{problemDetails.Title}: {problemDetails.Detail}");
                    }
                    else if (!string.IsNullOrEmpty(responseContent))
                    {
                        // Se houver conteúdo na resposta, usa-o como mensagem de erro
                        return (null, $"Erro ao buscar produto: {responseContent}");
                    }
                    else
                    {
                        // Mensagem de erro genérica se não houver conteúdo ou ProblemDetails
                        return (null, "Ocorreu um erro desconhecido ao buscar o produto.");
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                // Captura erros de rede ou conexão com a API
                return (null, $"Não foi possível conectar ao servidor da API: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Captura quaisquer outros erros inesperados
                return (null, $"Ocorreu um erro inesperado: {ex.Message}");
            }
        }

    }
}