using AjudeiMais.API.DTO;
using AjudeiMais.API.Services;
using AjudeiMais.Data.Models.InstituicaoModel;
using Microsoft.AspNetCore.Mvc;

namespace AjudeiMais.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstituicaoImagemController : ControllerBase
    {
        private readonly InstituicaoImagemService _instituicaoImagemService;

        public InstituicaoImagemController(InstituicaoImagemService instituicaoImagemService)
        {
            _instituicaoImagemService = instituicaoImagemService;
        }

        [HttpGet]
        public async Task<IActionResult> GetImagens()
        {
            try
            {
                var result = await _instituicaoImagemService.GetAll();
                return Ok(result); // Retorna o resultado diretamente do serviço
            }
            catch (Exception ex)
            {
                // Retorna um StatusCode 500 com o objeto ApiResponse de erro
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Type = "error",
                    Message = "Ocorreu um erro interno no servidor ao buscar imagens. Por favor, tente novamente mais tarde."
                });
            }
        }

        [HttpGet("ativos")]
        public async Task<IActionResult> GetImagensAtivas()
        {
            try
            {
                var result = await _instituicaoImagemService.GetItens();
                return Ok(result); // Retorna o resultado diretamente do serviço
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Type = "error",
                    Message = "Ocorreu um erro interno no servidor ao buscar imagens ativas. Por favor, tente novamente mais tarde."
                });
            }
        }

        [HttpGet("instituicao/{instituicaoId}")]
        public async Task<IActionResult> GetImagensPorInstituicao(int instituicaoId)
        {
            try
            {
                var result = await _instituicaoImagemService.GetImagensPorInstituicao(instituicaoId);
                return Ok(result); // Retorna o resultado diretamente do serviço
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Type = "error",
                    Message = "Ocorreu um erro interno no servidor ao buscar imagens da instituição. Por favor, tente novamente mais tarde."
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveOrUpdate([FromBody] InstituicaoImagem model)
        {
            try
            {
                // Assumo que o SaveOrUpdate no serviço retornará um ApiResponse ou similar
                // para indicar sucesso/falha e a mensagem.
                var result = await _instituicaoImagemService.SaveOrUpdate(model);

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
                    Message = "Ocorreu um erro interno no servidor ao salvar ou atualizar a imagem. Por favor, tente novamente mais tarde."
                });
            }
        }

        [HttpPost("AtualizarFotos")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AtualizarFotos([FromForm] AtualizarFotosInstituicaoDTO model)
        {
            try
            {
                // Assumo que AtualizarFotosInstituicaoAsync no serviço retornará um ApiResponse ou similar
                var result = await _instituicaoImagemService.AtualizarFotosInstituicaoAsync(model);

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
                    Message = "Ocorreu um erro interno no servidor ao atualizar as fotos. Por favor, tente novamente mais tarde."
                });
            }
        }

        [HttpDelete("{id}")] // Use {id} para deletar por ID, um padrão mais RESTful
        public async Task<IActionResult> Delete(int id) // Altere para receber o ID
        {
            try
            {
                // Assumo que o Delete no serviço retornará um ApiResponse ou similar
                var result = await _instituicaoImagemService.Delete(id); // Passe o ID para o serviço

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
                    Message = "Ocorreu um erro interno no servidor ao excluir a imagem. Por favor, tente novamente mais tarde."
                });
            }
        }
    }
}