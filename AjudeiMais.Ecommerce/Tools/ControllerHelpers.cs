//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;
//using System.Net.Http;
//using System.Text.Json;
//using System;
//using AjudeiMais.Ecommerce.Models; // Assumindo que ProblemDetails está aqui, ou ajuste o namespace

//namespace AjudeiMais.Ecommerce.Tools
//{
//    public static class ControllerHelpers
//    {
//        public static IActionResult HandleUnauthorizedAccess(Controller controller, ILogger logger, out string loggedInUserGuid)
//        {
//            loggedInUserGuid = Assistant.GetUserGuidFromClaims(controller.User, "GUID");

//            if (string.IsNullOrEmpty(loggedInUserGuid))
//            {
//                logger?.LogWarning("Tentativa de acesso de usuário não autenticado ou sem GUID válido.");
//                return controller.RedirectToRoute("login", new { alertType = "error", alertMessage = "Sua sessão expirou ou seu GUID de usuário não foi encontrado. Por favor, faça login novamente." });
//            }
//            return null; // Indica sucesso
//        }

//        public static IActionResult HandleApiResponseError(Controller controller, ILogger logger, HttpResponseMessage response, string defaultErrorMessage, string redirectRoute, string guid = null)
//        {
//            var errorMessageFromApi = defaultErrorMessage;
//            try
//            {
//                var responseContent = response.Content.ReadAsStringAsync().Result;
//                var problemDetails = System.Text.Json.JsonSerializer.Deserialize<ProblemDetails>(responseContent,
//                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

//                if (problemDetails?.Title != null && problemDetails?.Detail != null)
//                {
//                    errorMessageFromApi = $"{problemDetails.Title}: {problemDetails.Detail}";
//                }
//                else if (!string.IsNullOrEmpty(responseContent))
//                {
//                    errorMessageFromApi = $"Detalhes do erro: {responseContent}";
//                }
//            }
//            catch (JsonException ex)
//            {
//                logger?.LogError(ex, "Falha ao desserializar ProblemDetails da API. Conteúdo: {ResponseContent}", response.Content.ReadAsStringAsync().Result);
//            }
//            catch (Exception ex)
//            {
//                logger?.LogError(ex, "Erro inesperado ao processar resposta da API.");
//            }

//            controller.TempData["alertType"] = "error";
//            controller.TempData["alertMessage"] = errorMessageFromApi;

//            if (guid != null)
//            {
//                return controller.RedirectToRoute(redirectRoute, new { guid = guid });
//            }
//            return controller.RedirectToRoute(redirectRoute);
//        }
//    }
//}