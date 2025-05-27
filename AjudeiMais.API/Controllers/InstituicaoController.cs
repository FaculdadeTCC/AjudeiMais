using AjudeiMais.API.DTO;
using AjudeiMais.API.Services;
using AjudeiMais.Data.Models.InstituicaoModel;
using Microsoft.AspNetCore.Mvc;

namespace AjudeiMais.API.Controllers
{
    [Route("Api/[controller]")]
    [ApiController]
    public class InstituicaoController : ControllerBase
    {
        private readonly InstituicaoService _instituicaoService;

        public InstituicaoController(InstituicaoService instituicaoService)
        {
            _instituicaoService = instituicaoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetInstituicoes()
        {
            try
            {
                var instituicoes = await _instituicaoService.GetAll();
                return Ok(instituicoes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("GetByGUID/{GUID}")]
        public async Task<IActionResult> GetInstituicaoByGuid(string GUID)
        {
            try
            {
                var instituicao = await _instituicaoService.GetByGUID(GUID);
                return Ok(instituicao);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);  // Retorna erro em caso de falha
            }
        }

        [HttpGet("Ativos")]

        public async Task<IActionResult> GetInstituicoesAtivas()
        {
            try
            {
                var instituicoes = await _instituicaoService.GetItens();
                return Ok(instituicoes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveOrUpdate(InstituicaoPostDTO model)
        {
            try
            {
                await _instituicaoService.SaveOrUpdate(model);

                if (model.Instituicao_ID == 0)
                {
                    // Retorna 201 para criação
                    return CreatedAtAction(nameof(GetInstituicoes), new { id = model.Instituicao_ID }, model);
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

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete(int Id)
        //{
        //    try
        //    {
        //        await _instituicaoService.Delete(Id);
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //}
    }
}
