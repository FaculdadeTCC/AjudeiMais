//using AjudeiMais.API.Services;
//using AjudeiMais.Data.Models.InstituicaoModel;
//using Microsoft.AspNetCore.Mvc;

//namespace AjudeiMais.API.Controllers
//{
//    [Route("Api/[controller]")]
//    [ApiController]
//    public class InstituicaoCategoriaController : ControllerBase
//    {
//        private readonly InstituicaoCategoriaService _instituicaoCategoriaService;

//        public InstituicaoCategoriaController(InstituicaoCategoriaService instituicaoCategoriaService)
//        {
//            _instituicaoCategoriaService = instituicaoCategoriaService;
//        }

//        [HttpGet]
//        public async Task<IActionResult> GetInstituicaoCategorias()
//        {
//            try
//            {
//                var lista = await _instituicaoCategoriaService.GetAll();
//                return Ok(lista);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, ex.Message);
//            }
//        }

//        [HttpGet("Ativos")]
//        public async Task<IActionResult> GetInstituicaoCategoriasAtivas()
//        {
//            try
//            {
//                var ativos = await _instituicaoCategoriaService.GetItens();
//                return Ok(ativos);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, ex.Message);
//            }
//        }

//        [HttpPost]
//        public async Task<IActionResult> SaveOrUpdate(InstituicaoCategoria model)
//        {
//            try
//            {
//                await _instituicaoCategoriaService.SaveOrUpdate(model);

//                if (model.InstituicaoCategoria_ID == 0)
//                {
//                    return CreatedAtAction(nameof(GetInstituicaoCategorias), new { id = model.InstituicaoCategoria_ID }, model);
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

//        [HttpDelete("{id}")]
//        public async Task<IActionResult> Delete(int id)
//        {
//            try
//            {
//                await _instituicaoCategoriaService.Delete(id);
//                return Ok();
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, ex.Message);
//            }
//        }
//    }
//}
