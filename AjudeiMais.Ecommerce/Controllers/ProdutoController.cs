﻿using System.Net.Http;
using System;
using AjudeiMais.Ecommerce.Filters;
using AjudeiMais.Ecommerce.Tools;
using Microsoft.AspNetCore.Mvc;
using AjudeiMais.Ecommerce.Models.Produto;
using System.Threading.Tasks;
using AjudeiMais.Ecommerce.Models.Usuario;
using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;
using System.Reflection; // Remova se não estiver usando
using AjudeiMais.Ecommerce.Models.CategoriaProduto;
using Microsoft.Extensions.Logging; // Certifique-se de que este using existe

namespace AjudeiMais.Ecommerce.Controllers
{
    public class ProdutoController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ProdutoController> _logger; // Tornar obrigatório

        string BASE_URL = Assistant.ServerURL();
        public ProdutoController(IHttpClientFactory httpClientFactory, ILogger<ProdutoController> logger) // Logger deve ser obrigatório aqui
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [RoleAuthorize("admin", "usuario")]
        public async Task<IActionResult> Index(string guid)
        {
            string loggedInUserGuid;
            var unauthorizedResult = ControllerHelpers.HandleUnauthorizedAccess(this, _logger, out loggedInUserGuid);

            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

            var profileAccessResult = ControllerHelpers.ValidateUserProfileAccess(this, guid, loggedInUserGuid);

            if (profileAccessResult != null)
            {
                return profileAccessResult;
            }

            var (produtos, errorMessage) = await ApiHelper.ListAllAnunciosAtivosByGuidAsync(
                           _httpClientFactory, guid);

            // Adicione a lógica para passar mensagens de alerta para a view Index
            // Estes são os parâmetros que você passa no RedirectToRoute do método Excluir
            if (TempData.ContainsKey("AlertType") && TempData.ContainsKey("AlertMessage"))
            {
                ViewBag.AlertType = TempData["AlertType"];
                ViewBag.AlertMessage = TempData["AlertMessage"];
            }


            if (produtos != null)
            {
                ProdutoIndex model = new ProdutoIndex
                {
                    Anuncios = produtos
                };

                return View(model);
            }
            else
            {
                // Se a listagem falhar, redirecione para o perfil com a mensagem de erro.
                // O GUID para este redirecionamento deve ser o do usuário logado.
                return RedirectToRoute("usuario-perfil", new { alertType = "error", alertMessage = errorMessage, guid = loggedInUserGuid });
            }
        }
        [RoleAuthorize("admin", "usuario")]
        public async Task<IActionResult> Imagens(string guid)
        {
            string loggedInUserGuid;
            var unauthorizedResult = ControllerHelpers.HandleUnauthorizedAccess(this, _logger, out loggedInUserGuid);

            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

            var (produto, errorMessage) = await ApiHelper.GetProdutoByGuidAsync(
                          _httpClientFactory, guid);

            ProdutoEditarModel model = new ProdutoEditarModel();

            model = produto;


            if (model.Nome == null)
            {
                // 6. Sucesso no cadastro
                return RedirectToRoute("anuncios", new
                {
                    alertType = "error",
                    alertMessage = errorMessage,
                    guid = loggedInUserGuid
                });
            }


            return View(model);
        }

        [RoleAuthorize("admin", "usuario")]
        public async Task<IActionResult> Editar(string guid)
        {
            string loggedInUserGuid;
            var unauthorizedResult = ControllerHelpers.HandleUnauthorizedAccess(this, _logger, out loggedInUserGuid);

            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

            var (produto, errorMessage) = await ApiHelper.GetProdutoByGuidAsync(
                          _httpClientFactory, guid);

            ProdutoEditarModel model = new ProdutoEditarModel();

            model = produto;

            var apiResponse = await ApiHelper.ListAllCategoriasProdutoAtivosAsync(_httpClientFactory);

            if (apiResponse.Success)
            {
                produto.Categorias = apiResponse.Data;
                errorMessage = null;
            }
            else
            {
                produto.Categorias = null;
                errorMessage = apiResponse.Message;
            }


            if (model.Nome == null)
            {
                // 6. Sucesso no cadastro
                return RedirectToRoute("anuncios", new
                {
                    alertType = "error",
                    alertMessage = errorMessage,
                    guid = loggedInUserGuid
                });
            }


            return View(model);
        }

        [RoleAuthorize("admin", "usuario")]
        [HttpPost]
        public async Task<IActionResult> Cadastro(ProdutoModel model, IFormFile[] ProdutoImagens)
        {
            // 1. Validação de sessão/autenticação
            var userGuidFromClaims = User.FindFirst("GUID")?.Value;
            if (string.IsNullOrEmpty(userGuidFromClaims))
            {
                return RedirectToRoute("login", new
                {
                    alertType = "error",
                    alertMessage = "Sua sessão expirou ou não foi possível identificar o usuário. Faça login novamente."
                });
            }

            model.Usuario_GUID = userGuidFromClaims;

            // 2. Validação do modelo enviado pelo formulário
            if (!ModelState.IsValid)
            {
                var erros = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                string mensagemErro = "Corrija os erros no formulário: " + string.Join(" ", erros);

                return RedirectToRoute("usuario-anuncio-cadastrar", new
                {
                    alertType = "error",
                    alertMessage = mensagemErro,
                    guid = userGuidFromClaims
                });
            }

            // 3. Validação obrigatória da imagem principal
            if (ProdutoImagens == null || ProdutoImagens.Length == 0 || ProdutoImagens[0] == null || ProdutoImagens[0].Length == 0)
            {
                return RedirectToRoute("usuario-anuncio-cadastrar", new
                {
                    alertType = "error",
                    alertMessage = "A imagem principal do produto é obrigatória.",
                    guid = userGuidFromClaims
                });
            }

            try
            {
                var httpClient = _httpClientFactory.CreateClient("ApiAjudeiMais");

                // 4. Envia os dados do produto para a API
                var json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync($"{Tools.Assistant.ServerURL()}api/Produto", content);

                var responseBody = await response.Content.ReadAsStringAsync();

                ApiHelper.ApiResponse<ProdutoResponse> apiResponse;
                try
                {
                    apiResponse = JsonSerializer.Deserialize<ApiHelper.ApiResponse<ProdutoResponse>>(responseBody,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao desserializar resposta da API no cadastro do produto.");
                    return RedirectToRoute("usuario-anuncio-cadastrar", new
                    {
                        alertType = "error",
                        alertMessage = "Erro inesperado no cadastro. Tente novamente.",
                        guid = userGuidFromClaims
                    });
                }

                if (apiResponse == null || !apiResponse.Success)
                {
                    return RedirectToRoute("usuario-anuncio-cadastrar", new
                    {
                        alertType = "error",
                        alertMessage = apiResponse?.Message ?? "Erro ao cadastrar produto.",
                        guid = userGuidFromClaims
                    });
                }

                var produtoId = apiResponse.Data.Produto_ID;

                // 5. Envia cada imagem para a API
                foreach (var imagem in ProdutoImagens)
                {
                    if (imagem != null && imagem.Length > 0)
                    {
                        using var formData = new MultipartFormDataContent();

                        formData.Add(new StringContent(produtoId.ToString()), "Produto_ID");

                        var imageContent = new StreamContent(imagem.OpenReadStream());
                        imageContent.Headers.ContentType = new MediaTypeHeaderValue(imagem.ContentType);

                        formData.Add(imageContent, "Imagem", imagem.FileName);

                        var uploadResponse = await httpClient.PostAsync($"{Tools.Assistant.ServerURL()}api/ProdutoImagem", formData);
                        if (!uploadResponse.IsSuccessStatusCode)
                        {
                            var uploadResponseBody = await uploadResponse.Content.ReadAsStringAsync();
                            return RedirectToRoute("usuario-anuncio-cadastrar", new
                            {
                                alertType = "error",
                                alertMessage = $"Erro ao enviar imagem: {uploadResponseBody}",
                                guid = userGuidFromClaims
                            });
                        }
                    }
                }

                // 6. Sucesso no cadastro
                return RedirectToRoute("anuncios", new
                {
                    alertType = "success",
                    alertMessage = apiResponse.Message,
                    guid = userGuidFromClaims
                });
            }
            catch (Exception ex)
            {
                return RedirectToRoute("usuario-anuncio-cadastrar", new
                {
                    alertType = "error",
                    alertMessage = "Erro inesperado no cadastro. Tente novamente.",
                    guid = userGuidFromClaims
                });
            }
        }

        [RoleAuthorize("admin", "usuario")]
        [HttpGet]
        public async Task<IActionResult> Cadastro([FromRoute] Guid guid)
        {
            string loggedInUserGuid;
            var unauthorizedResult = ControllerHelpers.HandleUnauthorizedAccess(this, _logger, out loggedInUserGuid);
            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

            var model = new ProdutoModel();
            var response = await ApiHelper.ListAllCategoriasProdutoAtivosAsync(_httpClientFactory);

            model.Categorias = response.Data.Where(c => c.Habilitado == true);
            model.Usuario_GUID = loggedInUserGuid;

            return View(model);
        }

        [RoleAuthorize("admin", "usuario")]
        public IActionResult _DeletarProduto()
        {
            return PartialView();
        }

        [HttpPost]
        [RoleAuthorize("admin", "usuario")]
        public async Task<IActionResult> Excluir(string guid)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var loggedInUserGuid = User.FindFirst("GUID")?.Value;

            if (string.IsNullOrEmpty(loggedInUserGuid))
            {
                if (isAjax)
                    return Json(new { success = false, message = "Sessão expirada.", redirect = "/login" });

                TempData["AlertType"] = "error";
                TempData["AlertMessage"] = "Sua sessão expirou. Faça login novamente.";
                return RedirectToRoute("login");
            }

            if (string.IsNullOrEmpty(guid))
            {
                if (isAjax)
                    return Json(new { success = false, message = "Nenhum anúncio selecionado para exclusão." });

                TempData["AlertType"] = "warning";
                TempData["AlertMessage"] = "Nenhum anúncio selecionado para exclusão.";
                return RedirectToRoute("anuncios", new { guid = loggedInUserGuid });
            }

            try
            {
                var httpClient = _httpClientFactory.CreateClient("ApiAjudeiMais");
                var response = await httpClient.DeleteAsync($"{BASE_URL}api/Produto/{guid}");

                var apiResponseContent = await response.Content.ReadAsStringAsync();
                var apiContentType = response.Content.Headers.ContentType?.MediaType;

                ApiHelper.ApiResponse<ProdutoModel> apiResponse = null;

                if (!string.IsNullOrEmpty(apiResponseContent) && apiContentType == "application/json")
                {
                    apiResponse = JsonSerializer.Deserialize<ApiHelper.ApiResponse<ProdutoModel>>(
                        apiResponseContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
                }
                else if (apiContentType == "text/html")
                {
                    _logger.LogError("API retornou HTML inesperado: {Content}", apiResponseContent);

                    if (isAjax)
                        return Json(new { success = false, message = "Resposta inválida da API (HTML em vez de JSON)." });

                    TempData["AlertType"] = "error";
                    TempData["AlertMessage"] = "Erro na API: resposta inesperada.";
                    return RedirectToRoute("anuncios", new { guid = loggedInUserGuid });
                }

                if (response.IsSuccessStatusCode && apiResponse?.Success == true)
                {
                    if (isAjax)
                        return Json(new { success = true, message = apiResponse.Message });

                    TempData["AlertType"] = apiResponse.Type;
                    TempData["AlertMessage"] = apiResponse.Message;
                    return RedirectToRoute("anuncios", new { guid = loggedInUserGuid });
                }
                else
                {
                    var errorMsg = apiResponse?.Message ?? "Erro desconhecido.";
                    if (isAjax)
                        return Json(new { success = false, message = errorMsg });

                    TempData["AlertType"] = apiResponse?.Type ?? "error";
                    TempData["AlertMessage"] = errorMsg;
                    return RedirectToRoute("anuncios", new { guid = loggedInUserGuid });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado na exclusão do produto.");

                if (isAjax)
                    return Json(new { success = false, message = "Erro inesperado ao excluir o produto." });

                TempData["AlertType"] = "error";
                TempData["AlertMessage"] = "Ocorreu um erro inesperado durante a exclusão.";
                return RedirectToRoute("anuncios", new { guid = loggedInUserGuid });
            }
        }


        [RoleAuthorize("admin", "usuario", "instituicao")]
        public async Task<IActionResult> Detalhe(string guid)
        {
            string loggedInUserGuid;
            var unauthorizedResult = ControllerHelpers.HandleUnauthorizedAccess(this, _logger, out loggedInUserGuid);

            if (unauthorizedResult != null)
            {
                return unauthorizedResult;
            }

            var (produto, errorMessage) = await ApiHelper.GetProdutoByGuidAsync(
                          _httpClientFactory, guid);

            ProdutoEditarModel model = new ProdutoEditarModel();

            model = produto;

            var apiResponse = await ApiHelper.ListAllCategoriasProdutoAtivosAsync(_httpClientFactory);

            if (apiResponse.Success)
            {
                produto.Categorias = apiResponse.Data;
                errorMessage = null;
            }
            else
            {
                produto.Categorias = null;
                errorMessage = apiResponse.Message;
            }


            if (model.Nome == null)
            {
                // 6. Sucesso no cadastro
                return RedirectToRoute("anuncios", new
                {
                    alertType = "error",
                    alertMessage = errorMessage,
                    guid = loggedInUserGuid
                });
            }


            return View(model);
        }

        //[RoleAuthorize("admin", "usuario")]
        //[HttpPost]
        //public async Task<IActionResult> AdicionarImagens(ProdutoModel model, ProdutoImagemModel[] ProdutoImagens)
        //{
        //    var userGuidFromClaims = User.FindFirst("GUID")?.Value;
        //    if (string.IsNullOrEmpty(userGuidFromClaims))
        //    {
        //        return RedirectToRoute("login", new
        //        {
        //            alertType = "error",
        //            alertMessage = "Sua sessão expirou ou não foi possível identificar o usuário. Faça login novamente."
        //        });
        //    }

        //    if (model == null || model.Produto_ID == 0)
        //    {
        //        return RedirectToRoute("anuncios", new
        //        {
        //            alertType = "error",
        //            alertMessage = "ID do produto não fornecido para adicionar imagens."
        //        });
        //    }

        //    if (ProdutoImagens == null || ProdutoImagens.Length == 0 || ProdutoImagens.All(f => f == null || f.Length == 0))
        //    {
        //        return RedirectToRoute("usuario-anuncio-editar", new
        //        {
        //            alertType = "warning",
        //            alertMessage = "Nenhuma nova imagem foi selecionada para envio.",
        //            guid = model.Produto_ID
        //        });
        //    }

        //    int produtoId = (int)model.Produto_ID;

        //    try
        //    {
        //        var httpClient = _httpClientFactory.CreateClient("ApiAjudeiMais");

        //        foreach (var imagem in ProdutoImagens)
        //        {
        //            if (imagem != null && imagem.Length > 0)
        //            {
        //                using var formData = new MultipartFormDataContent();
        //                formData.Add(new StringContent(produtoId.ToString()), "guidString");

        //                var imageContent = new StreamContent(imagem.OpenReadStream());
        //                imageContent.Headers.ContentType = new MediaTypeHeaderValue(imagem.ContentType);
        //                formData.Add(imageContent, "Imagem", imagem.FileName);

        //                var uploadResponse = await httpClient.PostAsync($"{Tools.Assistant.ServerURL()}api/ProdutoImagem", formData);

        //                if (!uploadResponse.IsSuccessStatusCode)
        //                {
        //                    var uploadResponseBody = await uploadResponse.Content.ReadAsStringAsync();
        //                    _logger.LogError($"Erro ao enviar imagem para o produto {produtoId}: {uploadResponseBody}");
        //                    return RedirectToRoute("usuario-anuncio-editar", new
        //                    {
        //                        alertType = "error",
        //                        alertMessage = $"Erro ao enviar imagem '{imagem.FileName}': {uploadResponseBody}",
        //                        guid = produtoId
        //                    });
        //                }
        //            }
        //        }

        //        return RedirectToRoute("usuario-anuncio-editar", new
        //        {
        //            alertType = "success",
        //            alertMessage = "Novas imagens enviadas com sucesso!",
        //            guid = produtoId
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, $"Erro inesperado ao adicionar imagens para o produto {produtoId}.");
        //        return RedirectToRoute("usuario-anuncio-editar", new
        //        {
        //            alertType = "error",
        //            alertMessage = "Erro inesperado ao enviar novas imagens. Tente novamente.",
        //            guid = produtoId
        //        });
        //    }
        //}

        [RoleAuthorize("admin", "usuario")]
        [HttpPost]
        public async Task<IActionResult> SalvarAlteracoesImagens(List<ProdutoImagemModel> imagens, string guid)
        {
            var userGuidFromClaims = User.FindFirst("GUID")?.Value;
            if (string.IsNullOrEmpty(userGuidFromClaims))
            {
                return RedirectToRoute("login", new
                {
                    alertType = "error",
                    alertMessage = "Sua sessão expirou ou não foi possível identificar o usuário. Faça login novamente."
                });
            }

            if (imagens == null || imagens.Count == 0)
            {
                return RedirectToRoute("anuncio-imagem-editar", new
                {
                    alertType = "error",
                    alertMessage = "Não há nenhuma imagem para atualização.",
                    guid = guid
                });
            }

            try
            {
                var httpClient = _httpClientFactory.CreateClient("ApiAjudeiMais");
                var url = $"{Tools.Assistant.ServerURL()}api/ProdutoImagem";

                foreach (var imagem in imagens)
                {
                    using var formData = new MultipartFormDataContent();

                    // Adiciona os campos do objeto como campos do form
                    Assistant.AddObjectAsFormFields(formData, imagem);

                    var response = await httpClient.PutAsync(url, formData);

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorMsg = await response.Content.ReadAsStringAsync();
                        _logger.LogWarning("Erro ao atualizar imagem ID {ImagemId}: {Erro}", imagem.ProdutoImagem_ID, errorMsg);

                        return RedirectToRoute("anuncio-imagem-editar", new
                        {
                            alertType = "error",
                            alertMessage = "Falha ao salvar alterações em uma das imagens. Tente novamente.",
                            guid = guid
                        });
                    } 
                }

                return RedirectToRoute("anuncio-imagem-editar", new
                {
                    alertType = "success",
                    alertMessage = "Alterações nas imagens salvas com sucesso!",
                    guid = guid
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro inesperado ao salvar alterações nas imagens do produto.");
                return RedirectToRoute("anuncio-imagem-editar", new
                {
                    alertType = "error",
                    alertMessage = "Erro inesperado ao salvar alterações nas imagens. Tente novamente.",
                    guid = guid
                });
            }
        }


        [RoleAuthorize("admin", "usuario")]
        [HttpPost]
        public async Task<IActionResult> AdicionarImagens(IFormFile[] ProdutoImagens, string guid, int produtoId)
        {
            var userGuidFromClaims = User.FindFirst("GUID")?.Value;
            if (string.IsNullOrEmpty(userGuidFromClaims))
            {
                return RedirectToRoute("login", new
                {
                    alertType = "error",
                    alertMessage = "Sua sessão expirou ou não foi possível identificar o usuário. Faça login novamente."
                });
            }

            // Validação obrigatória da imagem principal
            if (ProdutoImagens == null || ProdutoImagens.Length == 0 || ProdutoImagens[0] == null || ProdutoImagens[0].Length == 0)
            {
                return RedirectToRoute("usuario-anuncio-cadastrar", new
                {
                    alertType = "error",
                    alertMessage = "A imagem principal do produto é obrigatória.",
                    guid = guid
                });
            }

            try
            {
                var httpClient = _httpClientFactory.CreateClient("ApiAjudeiMais");

                foreach (var imagem in ProdutoImagens)
                {
                    if (imagem != null && imagem.Length > 0)
                    {
                        using var formData = new MultipartFormDataContent();

                        formData.Add(new StringContent(produtoId.ToString()), "Produto_ID");

                        var imageContent = new StreamContent(imagem.OpenReadStream());
                        imageContent.Headers.ContentType = new MediaTypeHeaderValue(imagem.ContentType);

                        formData.Add(imageContent, "Imagem", imagem.FileName);

                        var uploadResponse = await httpClient.PostAsync($"{Tools.Assistant.ServerURL()}api/ProdutoImagem", formData);

                        if (!uploadResponse.IsSuccessStatusCode)
                        {
                            var uploadResponseBody = await uploadResponse.Content.ReadAsStringAsync();

                            return RedirectToRoute("usuario-anuncio-cadastrar", new
                            {
                                alertType = "error",
                                alertMessage = $"Erro ao enviar imagem: {uploadResponseBody}",
                                guid = guid
                            });
                        }
                    }
                }

                // Sucesso no envio de todas as imagens
                return RedirectToRoute("anuncios", new
                {
                    alertType = "success",
                    alertMessage = "Imagens enviadas com sucesso!",
                    guid = userGuidFromClaims
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro inesperado ao enviar imagens para o produto {produtoId}");

                return RedirectToRoute("usuario-anuncio-cadastrar", new
                {
                    alertType = "error",
                    alertMessage = "Erro inesperado no envio das imagens. Tente novamente.",
                    guid = guid
                });
            }
        }
    }
}