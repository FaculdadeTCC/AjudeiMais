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
            List<InstituicaoPerfilModel> instituicoes = new List<InstituicaoPerfilModel>();

            string errorMessage = null;

            try
            {
                var (apiResponse, errorMsg) = await ApiHelper.ListAllUsuariosAtivosAsync(_httpClientFactory);

                if (apiResponse != null) // If apiResponse (which is List<CategoriaDtoGet>) is not null, it means data was returned.
                {
                    usuarios = apiResponse; // Assign the directly returned list
                }
                else
                {
                    errorMessage = errorMsg ?? "Erro desconhecido ao carregar categorias.";

                    return RedirectToRoute("home", new { alertType = "error", alertMessage = errorMessage });

                }
            }
            catch (Exception ex)
            {
                errorMessage = "Ocorreu um erro inesperado ao carregar as categorias. Tente novamente mais tarde.";

                return RedirectToRoute("home", new { alertType = "error", alertMessage = errorMessage });

            }

            try
            {
                var (apiResponse, errorMsg) = await ApiHelper.ListAllInstituicoesAtivosAsync(_httpClientFactory);

                if (apiResponse != null) // If apiResponse (which is List<CategoriaDtoGet>) is not null, it means data was returned.
                {
                    // No need for apiResponse.Success or apiResponse.Data, as apiResponse IS the data.
                    instituicoes = apiResponse; // Assign the directly returned list
                }
                else
                {
                    errorMessage = errorMsg ?? "Erro desconhecido ao carregar categorias.";

                    return RedirectToRoute("home", new { alertType = "error", alertMessage = errorMessage });
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Ocorreu um erro inesperado ao carregar as categorias. Tente novamente mais tarde.";

                return RedirectToRoute("home", new { alertType = "error", alertMessage = errorMessage });

            }

            AdminModel model = new AdminModel();

            model.Usuarios = usuarios;
            model.Instituicoes = instituicoes;

            return View(model);

        }
    }
}
