//using AjudeiMais.API.Services;
//using AjudeiMais.Data.Models.ProdutoModel;
//using Microsoft.AspNetCore.Mvc;

//namespace AjudeiMais.API.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class CategoriaProdutoController : ControllerBase
//    {
//        private readonly CategoriaProdutoService _service;

//        public CategoriaProdutoController(CategoriaProdutoService categoriaProdutoService)
//        {
//            _service = categoriaProdutoService;
//        }

//        [HttpGet]
//        public async Task<IActionResult> GetCategorias()
//        {
//            try
//            {
//                var categoriaProdutos = await _service.GetAll();
//                return Ok(categoriaProdutos);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, ex.Message);
//            }
//        }

//        [HttpGet("ativos")]
//        public async Task<IActionResult> GetCategoriasAtivos()
//        {
//            try
//            {
//                var categoriaProdutosAtivas = await _service.GetItens();
//                return Ok(categoriaProdutosAtivas);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, ex.Message);
//            }
//        }

//        [HttpPost]
//        public async Task<IActionResult> SaveOrUpdate([FromBody] CategoriaProduto model)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            try
//            {
//                await _service.SaveOrUpdate(model);
//                return CreatedAtAction(nameof(GetCategorias), new { id = model.CategoriaProduto_ID }, model);
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
//    }
//}
