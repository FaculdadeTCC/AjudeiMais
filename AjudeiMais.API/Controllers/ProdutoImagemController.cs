//using AjudeiMais.API.Models;
//using AjudeiMais.API.Services;
//using AjudeiMais.Data.Models.ProdutoModel;
//using Microsoft.AspNetCore.Mvc;

//namespace AjudeiMais.API.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class ProdutoImagemController : ControllerBase
//    {
//        private readonly ProdutoImagemService _service;

//        public ProdutoImagemController(ProdutoImagemService produtoImagemService)
//        {
//            _service = produtoImagemService;
//        }

//        [HttpGet]
//        public async Task<IActionResult> GetProdutoImagem()
//        {
//            try
//            {
//                var produtos = await _service.GetAll();
//                return Ok(produtos);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, ex.Message);
//            }
//        }

//        [HttpGet("ativos")]
//        public async Task<IActionResult> GetProdutoImagemAtivos()
//        {
//            try
//            {
//                var produtosAtivos = await _service.GetItens();
//                return Ok(produtosAtivos);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, ex.Message);
//            }
//        }

//        [HttpPost]
//        public async Task<IActionResult> SaveOrUpdate(ProdutoImagemUploadDto model)
//        {
//            try
//            {
//                await _service.SaveOrUpdate(model);

//                if (model.ProdutoImagem_ID == 0)
//                {
//                    return CreatedAtAction(nameof(GetProdutoImagem), new { id = model.ProdutoImagem_ID }, model);
//                }
//                else
//                {
//                    return Ok(model);
//                }
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, ex.Message);
//            }
//        }

//        [HttpGet("imagensProduto")]
//        public async Task<IActionResult> GetImagensPorProduto(int produtoId)
//        {
//            try
//            {
//                var imagensDoProduto = await _service.GetImagensPorProduto(produtoId);
//                return Ok(imagensDoProduto);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, ex.Message);
//            }
//        }

//        [HttpDelete("{id}")]
//        public async Task<IActionResult> Delete(int id)
//        {
//            try
//            {
//                await _service.Delete(id);
//                return Ok();
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, ex.Message);
//            }
//        }

//        [HttpPost("upload-imagem")]
//        public async Task<IActionResult> UploadImagem([FromForm] ProdutoImagemUploadDto dto)
//        {
//            await _service.SaveOrUpdate(dto);
//            return Ok();
//        }
//    }
//}
