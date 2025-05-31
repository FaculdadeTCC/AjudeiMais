using AjudeiMais.API.DTO;
using AjudeiMais.API.Models;
using AjudeiMais.API.Services;
using AjudeiMais.Data.Models.ProdutoModel;
using Microsoft.AspNetCore.Mvc;

namespace AjudeiMais.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoImagemController : ControllerBase
    {
        private readonly ProdutoImagemService _service;

        public ProdutoImagemController(ProdutoImagemService produtoImagemService)
        {
            _service = produtoImagemService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProdutoImagem()
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
        public async Task<IActionResult> GetProdutoImagemAtivos()
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
        public async Task<IActionResult> SaveOrUpdate([FromForm] ProdutoImagemUploadDto model)
        {
            try
            {
                var result = await _service.SaveOrUpdate(model);

                if (result.Success)
                    return Ok(result);
                else
                    return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Type = "error",
                    Message = "Ocorreu um erro interno no servidor. Por favor, tente novamente mais tarde."
                });
            }
        }


        [HttpGet("imagensProduto")]
        public async Task<IActionResult> GetImagensPorProduto(int produtoId)
        {
            try
            {
                var imagensDoProduto = await _service.GetImagensPorProduto(produtoId);
                return Ok(imagensDoProduto);
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

        [HttpPost("upload-imagem")]
        public async Task<IActionResult> UploadImagem([FromForm] ProdutoImagemUploadDto dto)
        {
            await _service.SaveOrUpdate(dto);
            return Ok();
        }
    }
}
