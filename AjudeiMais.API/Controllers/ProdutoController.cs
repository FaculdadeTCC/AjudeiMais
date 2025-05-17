using AjudeiMais.API.Services;
using AjudeiMais.Data.Models.ProdutoModel;
using Microsoft.AspNetCore.Mvc;
using AjudeiMais.API.Tools;

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

        /// <summary>
        /// Retorna todos os produtos.
        /// </summary>
        /// <returns>Lista de produtos.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllProdutos()
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

        /// <summary>
        /// Retorna todos os produtos ativos (habilitados e não excluídos).
        /// </summary>
        /// <returns>Lista de produtos ativos.</returns>
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

        /// <summary>
        /// Cria ou atualiza um produto.
        /// </summary>
        /// <param name="model">Dados do produto.</param>
        /// <returns>Produto criado ou atualizado.</returns>
        [HttpPost]
        public async Task<IActionResult> SaveOrUpdate([FromBody] Produto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _service.SaveOrUpdate(model);
                return CreatedAtAction(nameof(GetById), new { id = model.Produto_ID }, model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Exclui logicamente um produto pelo ID.
        /// </summary>
        /// <param name="id">ID do produto.</param>
        /// <returns>Ok se excluído com sucesso.</returns>
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

        /// <summary>
        /// Retorna um produto pelo seu ID.
        /// </summary>
        /// <param name="id">ID do produto.</param>
        /// <returns>Produto correspondente.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var produto = await _service.GetById(id);
                if (produto == null)
                    return NotFound();

                return Ok(produto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Retorna todos os produtos pertencentes a um usuário.
        /// </summary>
        /// <param name="usuarioId">ID do usuário.</param>
        /// <returns>Lista de produtos do usuário.</returns>
        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> GetProdutosByUsuarioId(int usuarioId)
        {
            try
            {
                var produtos = await _service.GetByUsuarioId(usuarioId);
                return Ok(produtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Retorna os produtos (anúncios) próximos a uma localização (latitude e longitude).
        /// </summary>
        /// <param name="lat">Latitude do ponto de referência.</param>
        /// <param name="lng">Longitude do ponto de referência.</param>
        /// <returns>Lista de produtos próximos.</returns>
        [HttpGet("proximos")]
        public async Task<IActionResult> GetAnunciosProximos(double lat, double lng)
        {
            try
            {
                var todosUsuarios = await _usuarioService.GetItens();
                // Filtra usuários com latitude, longitude e que tenham produtos não excluídos
                var usuariosComProdutos = todosUsuarios
                    .Where(u => u.Latitude != null && u.Longitude != null)
                    .Where(u => u.Produtos != null && u.Produtos.Any(p => !p.Excluido))
                    .ToList();

                var raioBuscaKm = 30;

                // Filtra os usuários que estão dentro do raio de busca definido (em km) a partir da localização fornecida (lat, lng).
                // Para cada usuário, calcula a distância entre o ponto de referência e a localização do usuário,
                // incluindo-o na lista se a distância for menor ou igual ao raioBuscaKm.
                var usuariosProximos = usuariosComProdutos
                    .Where(u =>
                        // Calcula a distância em km entre a localização fornecida (lat, lng) 
                        // e a localização do usuário (u.Latitude, u.Longitude).
                        Helpers.CalcularDistancia(lat, lng, (double)u.Latitude, (double)u.Longitude) <= raioBuscaKm
                    )
                    .ToList();


                var produtosProximos = new List<Produto>();

                // Seleciona os produtos dos usuários próximos
                foreach (var usuario in usuariosProximos)
                {
                    var produtosDoUsuario = await _service.GetByUsuarioId(usuario.Usuario_ID);
                    produtosProximos.AddRange(produtosDoUsuario);
                }

                return Ok(produtosProximos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
