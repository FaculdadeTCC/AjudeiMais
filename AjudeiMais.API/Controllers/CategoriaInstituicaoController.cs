using AjudeiMais.API.DTO;
using AjudeiMais.API.Services;
using AjudeiMais.Data.Models.InstituicaoModel; // Certifique-se de que Categoria está neste namespace
using Microsoft.AspNetCore.Authorization; // Manter se necessário para futuras autorizações
using Microsoft.AspNetCore.Mvc;

namespace AjudeiMais.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaInstituicaoController : ControllerBase
    {
        private readonly CategoriaInstituicaoService _categoriaService;

        public CategoriaInstituicaoController(CategoriaInstituicaoService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoriaById(int id)
        {
            try
            {
                var result = await _categoriaService.GetById(id);
                if (result.Success)
                {
                    return Ok(result);
                }
                // Se a categoria não for encontrada, o serviço já retorna Success = false
                return NotFound(result); // Ou BadRequest, dependendo da sua preferência para "não encontrado"
            }
            catch (Exception ex)
            {
                // Logar a exceção para depuração
                // O logger já está no serviço, mas um log adicional na controller pode ser útil para ver o fluxo
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Type = "error",
                    Message = "Ocorreu um erro interno no servidor ao buscar a categoria. Por favor, tente novamente mais tarde."
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCategorias()
        {
            try
            {
                var result = await _categoriaService.GetAll();
                // O serviço já retorna um ApiResponse, então apenas o repassamos
                if (result.Success)
                {
                    return Ok(result);
                }
                return BadRequest(result); // Se por algum motivo GetAll() retornar falha, embora não seja esperado
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Type = "error",
                    Message = "Ocorreu um erro interno no servidor ao buscar todas as categorias. Por favor, tente novamente mais tarde."
                });
            }
        }

        [HttpGet("Ativos")]
        public async Task<IActionResult> GetCategoriasAtivas()
        {
            try
            {
                var result = await _categoriaService.GetItens();
                if (result.Success)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Type = "error",
                    Message = "Ocorreu um erro interno no servidor ao buscar categorias ativas. Por favor, tente novamente mais tarde."
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveOrUpdate(CategoriaInstituicaoDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                                           .SelectMany(v => v.Errors)
                                           .Select(e => e.ErrorMessage)
                                           .ToList();

                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Type = "error",
                        Message = "Erro de validação nos dados enviados.",
                        Data = errors
                    });
                }

                var result = await _categoriaService.SaveOrUpdate(model);

                if (result.Success)
                {
                    // Se for um novo registro, pode ser CreatedAtAction
                    // Para atualizações, Ok é apropriado.
                    // A decisão aqui é baseada no Categoria_ID ser 0 para novo, ou > 0 para atualização.
                    // O ApiResponse já contém a mensagem de sucesso apropriada.
                    return Ok(result);
                }
                else
                {
                    // Se o serviço indicar falha (ex: nome de categoria duplicado)
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Type = "error",
                    Message = "Ocorreu um erro interno no servidor ao salvar ou atualizar a categoria. Por favor, tente novamente mais tarde."
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _categoriaService.Delete(id);

                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    // Se a categoria não for encontrada para exclusão
                    if (result.Message == "Categoria não encontrada.") // Adapte se a mensagem for diferente
                    {
                        return NotFound(result);
                    }
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Type = "error",
                    Message = "Ocorreu um erro interno no servidor ao excluir a categoria. Por favor, tente novamente mais tarde."
                });
            }
        }
    }
}