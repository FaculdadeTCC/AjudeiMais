using AjudeiMais.API.Services;
using AjudeiMais.Data.Models.ProdutoModel;
using Microsoft.AspNetCore.Mvc;

namespace AjudeiMais.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoController : ControllerBase
    {
        private readonly ProdutoService _service;

        public ProdutoController(ProdutoService produtoService)
        {
            _service = produtoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProdutos()
        {
            try
            {
                var produtos = await _service.GetAll();
                return Ok(produtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("ativos")]
        public async Task<IActionResult> GetProdutosAtivos()
        {
            try
            {
                var produtosAtivos = await _service.GetItens();
                return Ok(produtosAtivos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveOrUpdate(Produto model)
        {
            // Verifica se o modelo está válido antes de continuar
            if (!ModelState.IsValid)
            {
                // Retorna os erros de validação
                return BadRequest(ModelState);
            }

            try
            {
                await _service.SaveOrUpdate(model);

                if (model.Produto_ID == 0)
                {
                    return CreatedAtAction(nameof(GetProdutos), new { id = model.Produto_ID }, model);
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
                await _service.Delete(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
