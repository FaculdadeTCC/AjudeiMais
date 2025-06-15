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
using AjudeiMais.Ecommerce.Models.Pedido;
using System.Net.Http.Headers;
using System.Text; // Para CategoriaModel e InstituicaoPerfilModel

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

        /// <summary>
        /// Busca um produto pelo seu GUID na API.
        /// </summary>
        /// <param name="httpClientFactory">Factory para criar instâncias de HttpClient.</param>
        /// <param name="guid">O GUID do produto a ser buscado.</param>
        /// <returns>
        /// Uma tupla contendo o ProdutoEditarModel se encontrado, ou uma mensagem de erro.
        /// </returns>
        public static async Task<(ProdutoEditarModel? Produto, string? ErrorMessage)> GetProdutoByGuidAsync(
            IHttpClientFactory httpClientFactory, string guid)
        {
            // 1. Validação inicial do GUID
            if (string.IsNullOrEmpty(guid))
            {
                return (null, "GUID do produto não fornecido.");
            }

            try
            {
                // 2. Criação do HttpClient usando a factory (melhor prática)
                var httpClient = httpClientFactory.CreateClient("ApiAjudeiMais");
                // Opcional: Definir um timeout específico para esta requisição, se necessário.
                // httpClient.Timeout = TimeSpan.FromSeconds(30);

                // 3. Executa a requisição GET para a API
                var response = await httpClient.GetAsync($"{BASE_URL}api/Produto/{guid.ToString()}");

                // 4. Lê o conteúdo da resposta como string, independentemente do status HTTP.
                // Isso é feito aqui para que o conteúdo esteja disponível para depuração
                // e para diferentes cenários de erro (sucesso com corpo vazio, erro com detalhes no corpo).
                string content = string.Empty;
                if (response.Content != null)
                {
                    content = await response.Content.ReadAsStringAsync();
                }

                // 5. Verifica se a requisição HTTP foi bem-sucedida (status 2xx)
                if (response.IsSuccessStatusCode)
                {
                    // 5.1. Se o status é de sucesso, mas o conteúdo está vazio ou em branco
                    if (string.IsNullOrWhiteSpace(content))
                    {
                        return (null, "Resposta da API vazia ou sem conteúdo JSON para um status de sucesso.");
                    }

                    // 5.2. Tenta desserializar a resposta para um ApiResponse
                    ApiResponse<ProdutoEditarModel>? apiResponse = null;
                    try
                    {
                        apiResponse = JsonSerializer.Deserialize<ApiResponse<ProdutoEditarModel>>(
                            content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    }
                    catch (JsonException jsonEx)
                    {
                        // Loga o erro de desserialização JSON para depuração.
                        // Isso acontece se o JSON retornado for malformado ou não corresponder ao ApiResponse.
                        Console.WriteLine($"[ERRO] Desserialização JSON para ApiResponse falhou: {jsonEx.Message}. Conteúdo recebido: '{content}'");
                        return (null, $"Erro ao processar a resposta da API (formato JSON inválido). Detalhes: {jsonEx.Message}");
                    }

                    // 5.3. Verifica o status 'Success' dentro do ApiResponse
                    if (apiResponse != null && apiResponse.Success)
                    {
                        return (apiResponse.Data, null); // Retorna o produto e nenhum erro
                    }
                    else
                    {
                        // A API retornou sucesso HTTP (ex: 200 OK), mas a propriedade 'Success' dentro do JSON é 'false'.
                        // Isso é um erro de lógica de negócio da API.
                        string errorMsg = apiResponse?.Message ?? "Erro desconhecido da API ao buscar produtos.";
                        return (null, errorMsg);
                    }
                }
                else // 6. A requisição HTTP NÃO foi bem-sucedida (status 4xx ou 5xx)
                {
                    ProblemDetails? problemDetails = null;

                    // 6.1. Tenta desserializar o conteúdo como ProblemDetails (padrão para erros de API)
                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        try
                        {
                            problemDetails = JsonSerializer.Deserialize<ProblemDetails>(content,
                                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        }
                        catch (JsonException jsonEx)
                        {
                            // Loga o erro de desserialização JSON para ProblemDetails.
                            // Isso significa que o conteúdo do erro não era um ProblemDetails JSON válido.
                            Console.WriteLine($"[AVISO] Desserialização JSON para ProblemDetails falhou: {jsonEx.Message}. Conteúdo recebido: '{content}'");
                            // Continua, pois podemos usar o conteúdo bruto como mensagem de erro.
                        }
                    }

                    // 6.2. Retorna a mensagem de erro com base no ProblemDetails ou conteúdo bruto
                    if (problemDetails?.Title != null && problemDetails?.Detail != null)
                    {
                        return (null, $"{problemDetails.Title}: {problemDetails.Detail}");
                    }
                    else if (!string.IsNullOrEmpty(content))
                    {
                        // Se há conteúdo, mas não pôde ser desserializado como ProblemDetails,
                        // usa o conteúdo bruto como mensagem de erro.
                        return (null, $"Erro ao buscar produto: {content}");
                    }
                    else
                    {
                        // Fallback para status de erro HTTP sem conteúdo no corpo da resposta.
                        return (null, $"Erro ao buscar produto. Status: {response.StatusCode} {response.ReasonPhrase}. Resposta vazia.");
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                // 7. Captura erros de rede ou conexão (HttpRequestException)
                // Este é o erro 'Error while copying content to a stream.'
                // Incluir o InnerException é CRUCIAL para depuração.
                string errorMessage = $"Não foi possível conectar ao servidor da API. Verifique a conexão ou o status do servidor. Detalhes: {ex.Message}";
                if (ex.InnerException != null)
                {
                    errorMessage += $" Causa raiz: {ex.InnerException.Message}";
                }
                Console.WriteLine($"[ERRO] HttpRequestException: {errorMessage}");
                return (null, errorMessage);
            }
            catch (JsonException ex)
            {
                // 8. Captura qualquer outra exceção de JSON que possa ter escapado
                // (menos provável com os blocos try-catch específicos acima, mas bom para robustez)
                Console.WriteLine($"[ERRO] Erro de formato de dados inesperado (JSON): {ex.Message}");
                return (null, $"Erro de formato de dados inesperado (JSON): {ex.Message}");
            }
            catch (Exception ex)
            {
                // 9. Captura qualquer outra exceção inesperada
                Console.WriteLine($"[ERRO] Ocorreu um erro inesperado: {ex.Message}");
                return (null, $"Ocorreu um erro inesperado: {ex.Message}");
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

        public static async Task<(List<InstituicaoModel>? instituicoes, string? errorMessage)> ListAllInstituicoesAtivosAsync(
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
                    var instituicoes = JsonSerializer.Deserialize<List<InstituicaoModel>>(
                        content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (instituicoes != null)
                        return (instituicoes, null);
                    else
                        return (null, "A resposta da API veio vazia ou inválida.");
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
                return (null, $"Não foi possível conectar ao servidor da API: {httpEx.Message}. Verifique se a API está online.");
            }
            catch (JsonException jsonEx)
            {
                return (null, $"Erro de processamento da resposta da API (JSON inválido) ao listar instituições: {jsonEx.Message}.");
            }
            catch (Exception ex)
            {
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

        public static async Task<(List<GetPedidoModel>? pedidos, string? errorMessage)> ListAllPedidosAtivosInstiuicaoAsync(IHttpClientFactory httpClientFactory, string GUID)
        {
            try
            {
                var client = httpClientFactory.CreateClient("ApiAjudeiMais");
                string requestUri = $"{BASE_URL}api/Pedido/PedidosInstituicao/{GUID}";

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


        public static async Task<(ApiResponse<PedidoModel>?, string?)> CriarPedidoAsync(PedidoModel pedido, IHttpClientFactory _httpClientFactory, IHttpContextAccessor _httpContextAccessor)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ApiAjudeiMais");

                // 🧠 Recupera o token da Session
                var token = _httpContextAccessor.HttpContext?.Session.GetString("JwtToken"); // Note o "t" minúsculo

                if (string.IsNullOrEmpty(token))
                    return (null, "Token de autenticação não encontrado.");

                // 🔐 Adiciona o token JWT no cabeçalho Authorization
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                string requestUri = $"{BASE_URL}api/Pedido";

                var json = JsonSerializer.Serialize(pedido);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(requestUri, content);
                var responseBody = await response.Content.ReadAsStringAsync();

                var apiResponse = JsonSerializer.Deserialize<ApiResponse<PedidoModel>>(responseBody,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (response.IsSuccessStatusCode && apiResponse is not null)
                {
                    return (apiResponse, null);
                }

                string errorMsg = apiResponse?.Message ?? "Erro desconhecido ao criar pedido.";
                return (apiResponse, errorMsg);
            }
            catch (HttpRequestException ex)
            {
                return (null, $"Erro de conexão com a API: {ex.Message}");
            }
            catch (JsonException ex)
            {
                return (null, $"Erro ao processar resposta da API (JSON): {ex.Message}");
            }
            catch (Exception ex)
            {
                return (null, $"Erro inesperado ao criar pedido: {ex.Message}");
            }
        }





        #endregion
    }

    #endregion
}