using AjudeiMais.API.DTO;
using AjudeiMais.API.Services;
using AjudeiMais.Data.Models.ProdutoModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AjudeiMais.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaProdutoController : ControllerBase
    {
        private readonly CategoriaProdutoService _service;

        public CategoriaProdutoController(CategoriaProdutoService categoriaProdutoService)
        {
            _service = categoriaProdutoService;
        }

        [HttpGet("ativos")]
        public async Task<IActionResult> GetCategoriasAtivos()
        {
            try
            {
                var categoriaProdutos = await _service.GetItens();
                return Ok(categoriaProdutos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _service.GetById(id);

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
                var errorResponse = new ApiResponse<object>
                {
                    Success = false,
                    Type = "error",
                    Message = "Ocorreu um erro interno no servidor. Por favor, tente novamente mais tarde.",
                };

                return StatusCode(500, errorResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveOrUpdate([FromForm] CategoriaProdutoDTO model)
        {
            if (!ModelState.IsValid)
            {
                // Extrai as mensagens de erro do ModelState
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

            try
            {
                var result = await _service.SaveOrUpdate(model);

                if (result.Success) // Se a operação no serviço foi um sucesso
                {
                    return Ok(result); // Retorna 200 OK com a mensagem de sucesso
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<object>
                {
                    Success = false,
                    Type = "error",
                    Message = "Ocorreu um erro interno no servidor. Por favor, tente novamente mais tarde.",
                };

                return StatusCode(500, errorResponse);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _service.Delete(id);

                if (result.Success) // Se a operação no serviço foi um sucesso
                {
                    return Ok(result); // Retorna 200 OK com a mensagem de sucesso
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                var errorResponse = new ApiResponse<object>
                {
                    Success = false,
                    Type = "error",
                    Message = "Ocorreu um erro interno no servidor. Por favor, tente novamente mais tarde.",
                };

                return StatusCode(500, errorResponse);
            }
        }
    }
}
