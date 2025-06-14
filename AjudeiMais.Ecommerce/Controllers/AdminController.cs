using AjudeiMais.Ecommerce.Filters;
using AjudeiMais.Ecommerce.Models.Admin;
using AjudeiMais.Ecommerce.Models.Instituicao;
using AjudeiMais.Ecommerce.Models.Usuario;
using AjudeiMais.Ecommerce.Tools;
using Microsoft.AspNetCore.Mvc;

namespace AjudeiMais.Ecommerce.Controllers
{
    public class AdminController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<AdminController> _logger;

        string BASE_URL = Assistant.ServerURL();

        public AdminController(IHttpClientFactory httpClientFactory, ILogger<AdminController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [RoleAuthorize("admin")]
        public async Task<IActionResult> IndexAsync()
        {
            List<UsuarioPerfilModel> usuarios = new List<UsuarioPerfilModel>();
            List<InstituicaoModel> instituicoes = new List<InstituicaoModel>();

            string errorMessage = null;

            // --- Fetching Users ---
            try
            {
                var (apiResponseUsuarios, errorMsgUsuarios) = await ApiHelper.ListAllUsuariosAtivosAsync(_httpClientFactory);

                if (apiResponseUsuarios != null)
                {
                    usuarios = apiResponseUsuarios;
                }
                else
                {
                    // Log the error for debugging purposes
                    _logger.LogError("Failed to load active users: {ErrorMsg}", errorMsgUsuarios ?? "Unknown error.");
                    errorMessage = errorMsgUsuarios ?? "Ocorreu um erro ao carregar os usuários. Tente novamente mais tarde.";
                    return RedirectToRoute("home", new { alertType = "error", alertMessage = errorMessage });
                }
            }
            catch (Exception ex)
            {
                // Log the exception details
                _logger.LogError(ex, "An unexpected error occurred while fetching active users.");
                errorMessage = "Ocorreu um erro inesperado ao carregar os usuários. Tente novamente mais tarde.";
                return RedirectToRoute("home", new { alertType = "error", alertMessage = errorMessage });
            }

            // --- Fetching Institutions ---
            try
            {
                var (apiResponseInstituicoes, errorMsgInstituicoes) = await ApiHelper.ListAllInstituicoesAtivosAsync(_httpClientFactory);

                // **Validation for Instituicoes: Check if apiResponseInstituicoes is null**
                if (apiResponseInstituicoes != null)
                {
                    instituicoes = apiResponseInstituicoes;
                }
                else
                {
                    // Log the error for debugging purposes
                    _logger.LogError("Failed to load active institutions: {ErrorMsg}", errorMsgInstituicoes ?? "Unknown error.");
                    errorMessage = errorMsgInstituicoes ?? "Ocorreu um erro ao carregar as instituições. Tente novamente mais tarde.";
                    // Instead of redirecting and losing user data, maybe display a partial error or allow the page to load
                    // with an empty list for institutions if users loaded successfully.
                    // For now, I'll keep the redirect as per your previous pattern, but with a more specific message.
                    return RedirectToRoute("home", new { alertType = "error", alertMessage = errorMessage });
                }
            }
            catch (Exception ex)
            {
                // Log the exception details
                _logger.LogError(ex, "An unexpected error occurred while fetching active institutions.");
                errorMessage = "Ocorreu um erro inesperado ao carregar as instituições. Tente novamente mais tarde.";
                return RedirectToRoute("home", new { alertType = "error", alertMessage = errorMessage });
            }

            // --- Prepare and Return Model ---
            AdminModel model = new AdminModel
            {
                Usuarios = usuarios,
                Instituicoes = instituicoes
            };

            return View(model);
        }
    }
}