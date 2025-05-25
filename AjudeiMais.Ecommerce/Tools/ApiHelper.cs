using System.Net;
using System.Text.Json; // Preferir System.Text.Json para .NET Core
using System.Threading.Tasks;
using Newtonsoft.Json; // Manter se ainda usar em outros lugares, mas System.Text.Json é o padrão moderno
using Microsoft.AspNetCore.Mvc;
using AjudeiMais.Ecommerce.Models.Usuario; // Para ProblemDetails

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

        // Método estático para buscar usuário por GUID
        public static async Task<(UsuarioPerfilModel Usuario, string ErrorMessage)> GetUsuarioByGuidAsync(
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
                    // Usar System.Text.Json.JsonSerializer.Deserialize para desserialização
                    var usuario = System.Text.Json.JsonSerializer.Deserialize<UsuarioPerfilModel>(json,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return (usuario, null); // Retorna o usuário e null para a mensagem de erro
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    ProblemDetails problemDetails = null;

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
    }
}