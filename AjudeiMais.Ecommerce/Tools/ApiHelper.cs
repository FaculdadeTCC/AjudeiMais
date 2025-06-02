using System; // Adicionado para Exception e TimeSpan
using System.Collections.Generic; // Necessário para List<CategoriaModel>
using System.Net;
using System.Net.Http; // Adicionado para HttpRequestException
using System.Text.Json; // Preferir System.Text.Json para .NET Core
using System.Threading.Tasks;
// using Newtonsoft.Json; // Manter se ainda usar em outros lugares, mas System.Text.Json é o padrão moderno
using Microsoft.AspNetCore.Mvc; // Para ProblemDetails
using AjudeiMais.Ecommerce.Models.Usuario;
using AjudeiMais.Ecommerce.Models.Instituicao;
using AjudeiMais.Ecommerce.Models.CategoriaProduto;
using AjudeiMais.Ecommerce.Models.Produto;
using AjudeiMais.Ecommerce.Models.Pedido; // Para CategoriaModel e InstituicaoPerfilModel

namespace AjudeiMais.Ecommerce.Tools
{
    #region ApiHelper Class

    public static class ApiHelper
    {
        private static string BASE_URL = Assistant.ServerURL(); // Usando o Helper existente

        #region API Response Class

        public class ApiResponse<T>
        {
            public bool Success { get; set; }
            public string Type { get; set; } // "success", "error", "warning", etc.
            public string Message { get; set; }
            public T? Data { get; set; }
        }

        #endregion

        #region Produto API Calls

        /// <summary>
        /// Busca uma lista de produtos próximos a uma determinada localização.
        /// </summary>
        /// <param name="httpClientFactory">Factory para criar instâncias de HttpClient.</param>
        /// <param name="latitude">Latitude do ponto de referência para a busca.</param>
        /// <param name="longitude">Longitude do ponto de referência para a busca.</param>
        /// <returns>Uma tupla contendo a lista de ProdutoDtoGet e uma mensagem de erro, se houver.</returns>
        public static async Task<(List<ProdutosProximosDto>? produtos, string? errorMessage)> ListAllProdutosProximosAsync(
            IHttpClientFactory httpClientFactory, double latitude, double longitude)
        {
            try
            {
                var client = httpClientFactory.CreateClient("ApiAjudeiMais");
                // Endpoint para buscar produtos próximos, passando latitude e longitude como query parameters
                string requestUri = $"{BASE_URL}api/Produto/proximos?lat={latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}&lng={longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}";

                var response = await client.GetAsync(requestUri);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // Assumindo que o endpoint /api/Produto/proximos retorna um ApiResponse<IEnumerable<Produto>>
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse<IEnumerable<ProdutosProximosDto>>>(
                        content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (apiResponse != null && apiResponse.Success)
                    {
                        // Converte IEnumerable<ProdutoDtoGet> para List<ProdutoDtoGet>
                        return (apiResponse.Data?.ToList(), null);
                    }
                    else
                    {
                        // A API retornou sucesso HTTP, mas a propriedade 'Success' da ApiResponse é falsa
                        string errorMsg = apiResponse?.Message ?? "Erro desconhecido da API ao buscar produtos próximos.";
                        return (null, errorMsg);
                    }
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
                        errorMsg = $"Erro ao listar produtos próximos: {response.ReasonPhrase} (Status: {response.StatusCode})";
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
                return (null, $"Erro de processamento da resposta da API (JSON inválido) ao listar produtos próximos: {jsonEx.Message}.");
            }
            catch (Exception ex)
            {
                // Quaisquer outros erros inesperados
                return (null, $"Ocorreu um erro inesperado ao listar produtos próximos: {ex.Message}");
            }
        }

        public static async Task<(List<ProdutoResponse>?, string?)> ListAllAnunciosAtivosByGuidAsync(
            IHttpClientFactory httpClientFactory, string guid)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return (null, "GUID do usuário não fornecido.");
            }

            try
            {
                var client = httpClientFactory.CreateClient("ApiAjudeiMais");
                string requestUri = $"{BASE_URL}api/Produto/usuario/{guid}";

                var response = await client.GetAsync(requestUri);
                var apiResponseContent = await response.Content.ReadAsStringAsync();

                ApiResponse<List<ProdutoResponse>> apiResponse;

                try
                {
                    apiResponse = JsonSerializer.Deserialize<ApiResponse<List<ProdutoResponse>>>(
                        apiResponseContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
                }
                catch (JsonException)
                {
                    return (null, "Erro no formato da resposta da API ao listar os anúncios. Contate o suporte.");
                }

                if (!response.IsSuccessStatusCode || apiResponse == null || !apiResponse.Success)
                {
                    return (null, apiResponse?.Message ?? "Não foi possível listar os anúncios. Tente novamente.");
                }

                var produtos = apiResponse.Data;
                // Para cada produto, busca suas imagens
                foreach (var produto in produtos)
                {
                    try
                    {
                        string imagemRequestUri = $"{BASE_URL}api/ProdutoImagem/imagensProduto/{produto.Produto_ID}";
                        var imagemResponse = await client.GetAsync(imagemRequestUri);
                        var imagemApiResponseContent = await imagemResponse.Content.ReadAsStringAsync();

                        ApiResponse<List<ProdutoImagemModel>> imagemApiResponse;

                        try
                        {
                            imagemApiResponse = JsonSerializer.Deserialize<ApiResponse<List<ProdutoImagemModel>>>(
                                imagemApiResponseContent,
                                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                            );
                        }
                        catch (JsonException)
                        {
                            // Logar o erro, mas não impedir a listagem do produto se a imagem falhar
                            Console.WriteLine($"Erro ao desserializar imagens para o produto {produto.Produto_ID}.");
                            produto.ProdutoImagens = new List<ProdutoImagemModel>(); // Inicializa como vazia
                            continue; // Pula para o próximo produto
                        }

                        if (imagemResponse.IsSuccessStatusCode && imagemApiResponse != null && imagemApiResponse.Success && imagemApiResponse.Data != null)
                        {
                            produto.ProdutoImagens = imagemApiResponse.Data;
                        }
                        else
                        {
                            // Logar o erro se a API de imagens retornar falha
                            Console.WriteLine($"Falha ao buscar imagens para o produto {produto.Produto_ID}: {imagemApiResponse?.Message ?? "Erro desconhecido"}");
                            produto.ProdutoImagens = new List<ProdutoImagemModel>(); // Inicializa como vazia
                        }
                    }
                    catch (HttpRequestException httpExImagem)
                    {
                        Console.WriteLine($"Erro de conexão ao buscar imagens para o produto {produto.Produto_ID}: {httpExImagem.Message}");
                        produto.ProdutoImagens = new List<ProdutoImagemModel>(); // Inicializa como vazia
                    }
                    catch (Exception exImagem)
                    {
                        Console.WriteLine($"Erro inesperado ao buscar imagens para o produto {produto.Produto_ID}: {exImagem.Message}");
                        produto.ProdutoImagens = new List<ProdutoImagemModel>(); // Inicializa como vazia
                    }
                }

                return (produtos, null);
            }
            catch (HttpRequestException httpEx)
            {
                return (null, $"Não foi possível conectar ao servidor da API: {httpEx.Message}. Verifique se a API está online.");
            }
            catch (Exception ex)
            {
                return (null, $"Ocorreu um erro inesperado ao listar os anúncios: {ex.Message}");
            }
        }

        #endregion

          #region Categoria API Calls

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

        /// <summary>
        /// Lista todas as categorias de produto ativas da API.
        /// </summary>
        /// <param name="httpClientFactory">Fábrica para criar instâncias de HttpClient.</param>
        /// <returns>Um ApiResponse contendo a lista de categorias ou informações de erro.</returns>
        public static async Task<ApiResponse<List<CategoriaProdutoResponse>>> ListAllCategoriasProdutoAtivosAsync(
            IHttpClientFactory httpClientFactory)
        {
            try
            {
                var client = httpClientFactory.CreateClient("ApiAjudeiMais");
                string requestUri = $"{BASE_URL}api/CategoriaProduto/ativos";

                var response = await client.GetAsync(requestUri);
                var apiResponseContent = await response.Content.ReadAsStringAsync();

                ApiResponse<List<CategoriaProdutoResponse>> apiResponse;

                try
                {
                    // Tenta desserializar diretamente para ApiResponse<List<CategoriaProdutoResponse>>
                    apiResponse = JsonSerializer.Deserialize<ApiResponse<List<CategoriaProdutoResponse>>>(
                        apiResponseContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
                }
                catch (JsonException jsonEx)
                {
                    // Se houver um erro na desserialização JSON, retorna um ApiResponse de erro
                    return new ApiResponse<List<CategoriaProdutoResponse>>
                    {
                        Success = false,
                        Type = "error",
                        Message = "Ocorreu um erro no formato da resposta da API ao listar categorias. Contacte o suporte."
                    };
                }

                // Verifica o status HTTP e o sucesso da resposta da API
                if (!response.IsSuccessStatusCode || apiResponse == null || !apiResponse.Success)
                {
                    // Retorna a mensagem de erro da API, ou uma mensagem genérica se não disponível
                    return new ApiResponse<List<CategoriaProdutoResponse>>
                    {
                        Success = false,
                        Type = apiResponse?.Type ?? "error",
                        Message = apiResponse?.Message ?? "Não foi possível listar as categorias. Tente novamente."
                    };
                }

                // Se tudo estiver OK, retorna o ApiResponse completo com os dados
                return apiResponse;
            }
            catch (HttpRequestException httpEx)
            {
                // Captura erros de rede ou conexão com a API
                return new ApiResponse<List<CategoriaProdutoResponse>>
                {
                    Success = false,
                    Type = "error",
                    Message = $"Não foi possível conectar ao servidor da API: {httpEx.Message}. Verifique se a API está online."
                };
            }
            catch (Exception ex)
            {
                // Captura quaisquer outros erros inesperados
                return new ApiResponse<List<CategoriaProdutoResponse>>
                {
                    Success = false,
                    Type = "error",
                    Message = $"Ocorreu um erro inesperado ao listar categorias: {ex.Message}"
                };
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

        #endregion

        #region Usuario API Calls

        public static async Task<(List<UsuarioPerfilModel>? usuarios, string? errorMessage)> ListAllUsuariosAtivosAsync(
           IHttpClientFactory httpClientFactory)
        {
            try
            {
                var client = httpClientFactory.CreateClient("ApiAjudeiMais");
                string requestUri = $"{BASE_URL}api/Usuario/ativos";

                var response = await client.GetAsync(requestUri);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // A API pode retornar diretamente a lista ou encapsulada em um ApiResponse.
                    var usuarios = JsonSerializer.Deserialize<List<UsuarioPerfilModel>>(
                        content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    return (usuarios, null);
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
                        errorMsg = $"Erro ao listar usuários: {response.ReasonPhrase} (Status: {response.StatusCode})";
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
                return (null, $"Erro de processamento da resposta da API (JSON inválido) ao listar usuários: {jsonEx.Message}.");
            }
            catch (Exception ex)
            {
                // Quaisquer outros erros inesperados
                return (null, $"Ocorreu um erro inesperado ao listar usuários: {ex.Message}");
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

        #endregion

        #region Instituicao API Calls

        public static async Task<(List<InstituicaoPerfilModel>? instituicoes, string? errorMessage)> ListAllInstituicoesAtivosAsync(
           IHttpClientFactory httpClientFactory)
        {
            try
            {
                var client = httpClientFactory.CreateClient("ApiAjudeiMais");
                string requestUri = $"{BASE_URL}api/Instituicao/ativos";

                var response = await client.GetAsync(requestUri);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // A API pode retornar diretamente a lista ou encapsulada em um ApiResponse.
                    var instituicoes = JsonSerializer.Deserialize<List<InstituicaoPerfilModel>>(
                        content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    return (instituicoes, null);
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
                        errorMsg = $"Erro ao listar instituições: {response.ReasonPhrase} (Status: {response.StatusCode})";
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
                return (null, $"Erro de processamento da resposta da API (JSON inválido) ao listar instituições: {jsonEx.Message}.");
            }
            catch (Exception ex)
            {
                // Quaisquer outros erros inesperados
                return (null, $"Ocorreu um erro inesperado ao listar instituições: {ex.Message}");
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

        #endregion

        #region PEDIDO API Calls

        public static async Task<(List<GetPedidoModel>? pedidos, string? errorMessage)> ListAllPedidosAtivosAsync(IHttpClientFactory httpClientFactory)
        {
            try
            {
                var client = httpClientFactory.CreateClient("ApiAjudeiMais");
                string requestUri = $"{BASE_URL}api/Pedido/Ativos";

                var response = await client.GetAsync(requestUri);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // A API pode retornar diretamente a lista ou encapsulada em um ApiResponse.
                    var resposta = JsonSerializer.Deserialize<ApiResponse<List<GetPedidoModel>>>(
                        content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    var pedidos = resposta.Data;
                    return (pedidos, null);
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
                        errorMsg = $"Erro ao listar pedidos: {response.ReasonPhrase} (Status: {response.StatusCode})";
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
                return (null, $"Erro de processamento da resposta da API (JSON inválido) ao listar pedidos: {jsonEx.Message}.");
            }
            catch (Exception ex)
            {
                // Quaisquer outros erros inesperados
                return (null, $"Ocorreu um erro inesperado ao listar pedidos: {ex.Message}");
            }
        }
        #endregion
    }

    #endregion
}