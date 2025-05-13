using AjudeiMais.API.Services;
using AjudeiMais.Data.Models.InstituicaoModel;
using Microsoft.AspNetCore.Mvc;

namespace AjudeiMais.API.Controllers
{
    [Route("Api/[controller]")]
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
        public async Task<IActionResult> SaveOrUpdate(Endereco model)
        {
            try
            {
                await _enderecoService.SaveOrUpdate(model);

                if (model.Endereco_ID == 0)
                {
                    return CreatedAtAction(nameof(GetEnderecos), new { id = model.Endereco_ID }, model);
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
                await _enderecoService.Delete(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
