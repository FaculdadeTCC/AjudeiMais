using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text.Json;
using System;
using AjudeiMais.Ecommerce.Models; // Assumindo que ProblemDetails está aqui, ou ajuste o namespace

namespace AjudeiMais.Ecommerce.Tools
{
    public static class ControllerHelpers
    {
        public static IActionResult HandleApiResponseError(Controller controller, ILogger logger, HttpResponseMessage response, string defaultErrorMessage, string redirectRoute, string guid = null)
        {
            var errorMessageFromApi = defaultErrorMessage;
            try
            {
                var responseContent = response.Content.ReadAsStringAsync().Result;
                var problemDetails = System.Text.Json.JsonSerializer.Deserialize<ProblemDetails>(responseContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (problemDetails?.Title != null && problemDetails?.Detail != null)
                {
                    errorMessageFromApi = $"{problemDetails.Title}: {problemDetails.Detail}";
                }
                else if (!string.IsNullOrEmpty(responseContent))
                {
                    errorMessageFromApi = $"Detalhes do erro: {responseContent}";
                }
            }
            catch (JsonException ex)
            {
                logger?.LogError(ex, "Falha ao desserializar ProblemDetails da API. Conteúdo: {ResponseContent}", response.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Erro inesperado ao processar resposta da API.");
            }

            controller.TempData["alertType"] = "error";
            controller.TempData["alertMessage"] = errorMessageFromApi;

            if (guid != null)
            {
                return controller.RedirectToRoute(redirectRoute, new { guid = guid });
            }
            return controller.RedirectToRoute(redirectRoute);
        }

        /// <summary>
        /// Lida com a validação básica de acesso (autenticação e GUID do usuário logado).
        /// </summary>
        /// <param name="controller">A instância do Controller atual.</param>
        /// <param name="logger">O logger para registrar mensagens de erro/aviso.</param>
        /// <param name="loggedInUserGuid">O GUID do usuário logado (out parameter).</param>
        /// <returns>Um IActionResult de redirecionamento em caso de acesso não autorizado, ou null se o acesso for permitido.</returns>
        public static IActionResult HandleUnauthorizedAccess(Controller controller, ILogger logger, out string loggedInUserGuid)
        {
            loggedInUserGuid = Assistant.GetUserGuidFromClaims(controller.User, "GUID");

            // Se o usuário não estiver autenticado ou o GUID não puder ser recuperado
            if (!controller.User.Identity.IsAuthenticated || string.IsNullOrEmpty(loggedInUserGuid))
            {
                logger?.LogWarning("Tentativa de acesso de usuário não autenticado ou sem GUID válido.");
                // Redireciona para home se não autenticado, ou login se o GUID for inválido (pode ser uma sessão expirada)
                return controller.RedirectToRoute("login", new { alertType = "error", alertMessage = "Sua sessão expirou ou seu GUID de usuário não foi encontrado. Por favor, faça login novamente." });
            }
            return null; // Indica sucesso
        }


        /// <summary>
        /// Lida com a validação de acesso a um perfil de usuário, garantindo que
        /// apenas o próprio usuário ou um administrador possam acessar.
        /// Este método assume que HandleUnauthorizedAccess já foi chamado e validou o usuário.
        /// </summary>
        /// <param name="controller">A instância do Controller atual.</param>
        /// <param name="requestedGuid">O GUID do perfil que está sendo solicitado na URL.</param>
        /// <param name="loggedInUserGuid">O GUID do usuário que está logado e foi validado por HandleUnauthorizedAccess.</param>
        /// <returns>Um IActionResult de redirecionamento em caso de acesso não autorizado, ou null se o acesso for permitido.</returns>
        public static IActionResult ValidateUserProfileAccess(Controller controller, string requestedGuid, string loggedInUserGuid)
        {
            // Validação de segurança para garantir que o usuário só veja seu próprio perfil ou seja admin
            if (!controller.User.IsInRole("admin") && !string.Equals(requestedGuid, loggedInUserGuid, StringComparison.OrdinalIgnoreCase))
            {
                // Se não for admin e o GUID solicitado não corresponder ao do usuário logado
                return controller.RedirectToRoute("usuario-perfil", new { alertType = "error", alertMessage = "Você não tem permissão para acessar este perfil.", guid = loggedInUserGuid });
            }

            return null; // Acesso permitido
        }
    }
}
