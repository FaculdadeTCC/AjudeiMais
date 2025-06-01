using AjudeiMais.API.DTO;
using AjudeiMais.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace AjudeiMais.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AvaliacaoController : ControllerBase
    {
        private readonly AvaliacaoService _avaliacaoService;

        public AvaliacaoController(AvaliacaoService avaliacaoService)
        {
            _avaliacaoService = avaliacaoService;
        }

        [HttpPost]
        public async Task<IActionResult> SaveOrUpdate(AvaliacaoPostDTO dto)
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

                var result = await _avaliacaoService.SaveOrUpdate(dto);

                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
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
    }
}
