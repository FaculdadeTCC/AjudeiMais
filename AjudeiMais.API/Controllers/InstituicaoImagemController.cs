using AjudeiMais.API.Services;
using AjudeiMais.Data.Models.InstituicaoModel;
using Microsoft.AspNetCore.Mvc;

namespace AjudeiMais.API.Controllers
{
    [Route("Api/[controller]")]
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
                var instituicaoImagens = await _instituicaoImagemService.GetAll();
                return Ok(instituicaoImagens);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("Ativos")]
        public async Task<IActionResult> GetImagensAtivas()
        {
            try
            {
                var instituicaoImagens = await _instituicaoImagemService.GetItens();
                return Ok(instituicaoImagens);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("Instituicao/{instituicaoId}")]
        public async Task<IActionResult> GetImagensPorInstituicao(int instituicaoId)
        {
            try
            {
                var instituicaoImagens = await _instituicaoImagemService.GetImagensPorInstituicao(instituicaoId);
                return Ok(instituicaoImagens);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveOrUpdate(InstituicaoImagem model)
        {
            try
            {
                await _instituicaoImagemService.SaveOrUpdate(model);

                if (model.InsituicaoImagem_ID == 0)
                {
                    return CreatedAtAction(nameof(GetImagens), new { id = model.InsituicaoImagem_ID }, model);
                }
                else
                {
                    return Ok(model);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _instituicaoImagemService.Delete(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
