using AjudeiMais.API.Services;
using AjudeiMais.Data.Models.InstituicaoModel;
using Microsoft.AspNetCore.Mvc;
using AjudeiMais.API.DTO;

namespace AjudeiMais.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnderecoController : ControllerBase
    {
        private readonly EnderecoService _enderecoService;

        public EnderecoController(EnderecoService enderecoService)
        {
            _enderecoService = enderecoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetEnderecos()
        {
            try
            {
                var enderecos = await _enderecoService.GetAll();
                return Ok(enderecos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("Ativos")]
        public async Task<IActionResult> GetEnderecosAtivos()
        {
            try
            {
                var enderecos = await _enderecoService.GetItens();
                return Ok(enderecos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] Endereco model)
        {
            try
            {
                await _enderecoService.SaveOrUpdate(model);
                var response = new ApiResponse<Endereco>
                {
                    Success = true,
                    Type = "success",
                    Message = "Endereço salvo com sucesso",
                    Data = model
                };
                return CreatedAtAction(nameof(GetEnderecos), new { id = model.Endereco_ID }, response);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<Endereco>
                {
                    Success = false,
                    Type = "error",
                    Message = "Erro ao salvar o endereço",
                    Errors = new List<string> { ex.Message }
                };
                return StatusCode(500, response);
            }
        }

        [HttpPost("AtualizarEndereco")]
        public async Task<IActionResult> AtualizarEndereco([FromBody] EnderecoDTO model)
        {
            try
            {
                await _enderecoService.AtualizarEndereçoInstituicaoAsync(model);
                var response = new ApiResponse<EnderecoDTO>
                {
                    Success = true,
                    Type = "success",
                    Message = "Endereço da instituição atualizado com sucesso",
                    Data = model
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<EnderecoDTO>
                {
                    Success = false,
                    Type = "error",
                    Message = "Erro ao atualizar endereço da instituição",
                    Errors = new List<string> { ex.Message }
                };
                return StatusCode(500, response);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _enderecoService.Delete(id);
                var response = new ApiResponse<string>
                {
                    Success = true,
                    Type = "success",
                    Message = "Endereço deletado com sucesso",
                    Data = null
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ApiResponse<string>
                {
                    Success = false,
                    Type = "error",
                    Message = "Erro ao deletar endereço",
                    Errors = new List<string> { ex.Message }
                };
                return StatusCode(500, response);
            }
        }
    }
}
