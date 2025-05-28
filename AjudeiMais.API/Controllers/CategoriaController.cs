using AjudeiMais.API.Services;
using AjudeiMais.Data.Models.InstituicaoModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AjudeiMais.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private readonly CategoriaService _categoriaService;

        public CategoriaController(CategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategorias()
        {
            try
            {
                var categorias = await _categoriaService.GetAll();
                return Ok(categorias);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("Ativos")]
        public async Task<IActionResult> GetCategoriasAtivas()
        {
            try
            {
                var categorias = await _categoriaService.GetItens();
                return Ok(categorias);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost]
        public async Task<IActionResult> SaveOrUpdate(Categoria model)
        {
            try
            {
                await _categoriaService.SaveOrUpdate(model);

                if (model.Categoria_ID == 0)
                {
                    return CreatedAtAction(nameof(GetCategorias), new { id = model.Categoria_ID }, model);
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
                await _categoriaService.Delete(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
