using AjudeiMais.API.Services;
using AjudeiMais.Data.Models.ProdutoModel;
using Microsoft.AspNetCore.Mvc;
using AjudeiMais.API.Tools;
using AjudeiMais.API.Models;
using AjudeiMais.API.DTO;

namespace AjudeiMais.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoController : ControllerBase
    {
        private readonly ProdutoService _service;
        private readonly UsuarioService _usuarioService;

        public ProdutoController(ProdutoService produtoService, UsuarioService usuarioService)
        {
            _service = produtoService;
            _usuarioService = usuarioService;
        }

        ///// <summary>
        ///// Retorna todos os produtos.
        ///// </summary>
        ///// <returns>Lista de produtos.</returns>
        //[HttpGet]
        //public async Task<IActionResult> GetAllProdutos()
        //{
        //    try
        //    {
        //        //var produtos = await _service.GetAll();
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //}

        ///// <summary>
        ///// Retorna todos os produtos ativos (habilitados e não excluídos).
        ///// </summary>
        ///// <returns>Lista de produtos ativos.</returns>
        //[HttpGet("ativos")]
        //public async Task<IActionResult> GetProdutosAtivos()
        //{
        //    try
        //    {
        //        //var produtosAtivos = await _service.GetItens();
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //}

        /// <summary>
        /// Cria ou atualiza um produto.
        /// </summary>
        /// <param name="model">Dados do produto.</param>
        /// <returns>Produto criado ou atualizado.</returns>
        [HttpPost]
        public async Task<IActionResult> SaveOrUpdate([FromBody] ProdutoPostDto model)
        {
            try
            {
                var result = await _service.SaveOrUpdate(model, null); // imagens virão depois

                if (result.Success)
                    return Ok(result);
                else
                    return BadRequest(result);
            }
            catch (Exception)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Type = "error",
                    Message = "Ocorreu um erro interno no servidor. Por favor, tente novamente mais tarde."
                });
            }
        }


        /// <summary>
        /// Exclui logicamente um produto pelo ID.
        /// </summary>
        /// <param name="guid">GUID do produto.</param>
        /// <returns>Ok se excluído com sucesso.</returns>
        [HttpDelete("{guid}")]
        public async Task<IActionResult> Delete(string guid)
        {
            try
            {
                var result = await _service.Delete(guid);

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
                var errorResponse = new ApiResponse<object>
                {
                    Success = false,
                    Type = "error",
                    Message = "Ocorreu um erro interno no servidor. Por favor, tente novamente mais tarde.",
                };

                return StatusCode(500, errorResponse);
            }
        }

        /// <summary>
        /// Retorna um produto pelo seu ID.
        /// </summary>
        /// <param name="id">ID do produto.</param>
        /// <returns>Produto correspondente.</returns>
        [HttpGet("{guid}")]
        public async Task<IActionResult> GetByGuid(string guid)
        {
            try
            {
                var result = await _service.GetByGuid(guid);

                if (result.Success)
                    return Ok(result);
                else
                    return BadRequest();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Retorna todos os produtos pertencentes a um usuário.
        /// </summary>
        /// <returns>Lista de produtos do usuário.</returns>
        [HttpGet("usuario/{guid}")]
        public async Task<IActionResult> GetProdutosByUsuarioGuid(string guid)
        {
            try
            {
                var result = await _service.GetProdutosByUsuarioGuid(guid);

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
                    Message = "Ocorreu um erro interno no servidor. Por favor, tente novamente mais tarde.",
                     Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Retorna os produtos (anúncios) próximos a uma localização (latitude e longitude).
        /// </summary>
        /// <param name="lat">Latitude do ponto de referência.</param>
        /// <param name="lng">Longitude do ponto de referência.</param>
        /// <returns>Lista de produtos próximos.</returns>
        [HttpGet("proximos")]
        public async Task<IActionResult> GetProdutosProximos(double lat, double lng)
        {
            try
            {
                var result = await _service.GetProdutosProximos(lat, lng);

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
    }
}
