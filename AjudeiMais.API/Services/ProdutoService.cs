using AjudeiMais.API.Models;
using AjudeiMais.API.Repositories;
using AjudeiMais.API.Tools;
using AjudeiMais.Data.Models.ProdutoModel;
using Microsoft.Extensions.Logging;

namespace AjudeiMais.API.Services
{
    public class ProdutoService
    {
        private readonly ProdutoRepository _produtoRepository;
        private readonly UsuarioRepository _usuarioRepository;
        private readonly ProdutoImagemService _produtoImagemService;
        private readonly ILogger<ProdutoService> _logger;

        public ProdutoService(
            ProdutoRepository produtoRepository,
            ProdutoImagemService produtoImagemService,
            UsuarioRepository usuarioRepository,
            ILogger<ProdutoService> logger)
        {
            _produtoRepository = produtoRepository;
            _usuarioRepository = usuarioRepository;
            _produtoImagemService = produtoImagemService;
            _logger = logger;
        }

        /// <summary>
        /// Busca um produto pelo seu ID.
        /// </summary>
        /// <param name="id">ID do produto.</param>
        /// <returns>O produto encontrado.</returns>
        public async Task<Produto> GetById(int id)
        {
            try
            {
                var produto = await _produtoRepository.GetById(id);
                return produto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar produtos por ID");
                throw new Exception("Erro ao buscar produtos.");
            }
        }

        /// <summary>
        /// Busca todos os produtos.
        /// </summary>
        /// <returns>Uma coleção de todos os produtos.</returns>
        public async Task<IEnumerable<Produto>> GetAll()
        {
            try
            {
                return await _produtoRepository.GetAll();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos os produtos");
                throw new Exception("Erro ao buscar produtos.");
            }
        }

        /// <summary>
        /// Busca todos os produtos ativos (habilitados e não excluídos).
        /// </summary>
        /// <returns>Uma coleção de produtos ativos.</returns>
        public async Task<IEnumerable<Produto>> GetItens()
        {
            try
            {
                return await _produtoRepository.GetItens();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos os produtos");
                throw new Exception("Erro ao buscar produtos.");
            }
        }
        public async Task SaveOrUpdate(ProdutoDto model, List<IFormFile> imagens)
        {
            if (model.Produto_ID == 0)
            {
                model.DataCriacao = DateTime.Now;
                model.Habilitado = true;
                model.Excluido = false;
            }
            else
            {
                model.DataUpdate = DateTime.Now;
            }

            // Mapear ProdutoDto para Produto (entidade)
            Produto produto = new Produto
            {
                Produto_ID = model.Produto_ID,
                Nome = model.Nome,
                Descricao = model.Descricao,
                Condicao = model.Condicao ?? null,
                Validade = model.Validade,
                Quantidade = model.Quantidade,
                Peso = model.Peso ?? null,
                Disponivel = model.Disponivel,
                Usuario_ID = model.Usuario_ID,
                CategoriaProduto_ID = model.CategoriaProduto_ID,
                DataCriacao = DateTime.Now,
                Habilitado = true, 
                Excluido = false
            };

            await _produtoRepository.SaveOrUpdate(produto);

            // Agora salva as imagens
            foreach (var imagem in imagens)
            {
                var dto = new ProdutoImagemUploadDto
                {
                    ProdutoId = produto.Produto_ID,
                    Imagem = imagem
                };

                await _produtoImagemService.SaveOrUpdate(dto);
            }
        }


        /// <summary>
        /// Busca todos os produtos pertencentes a um usuário específico.
        /// </summary>
        /// <param name="id">ID do usuário.</param>
        /// <returns>Uma coleção de produtos do usuário.</returns>
        public async Task<IEnumerable<Produto>> GetByUsuarioId(int id)
        {
            try
            {
                return await _produtoRepository.GetByUsuarioId(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir o produto com ID {ProdutoId}", id);
                throw new Exception("Erro ao excluir o produto.");
            }
        }

        /// <summary>
        /// Exclui logicamente um produto pelo seu ID.
        /// </summary>
        /// <param name="id">ID do produto a ser excluído.</param>
        public async Task Delete(int id)
        {
            try
            {
                await _produtoRepository.Delete(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir o produto com ID {ProdutoId}", id);
                throw new Exception("Erro ao excluir o produto.");
            }
        }

        /// <summary>
        /// Busca produtos próximos a uma determinada localização (latitude e longitude).
        /// </summary>
        /// <param name="lat">Latitude do ponto de referência.</param>
        /// <param name="lng">Longitude do ponto de referência.</param>
        /// <returns>Uma coleção de produtos próximos.</returns>
        public async Task<IEnumerable<Produto>> GetProdutosProximos(double lat, double lng)
        {
            var todosUsuarios = await _usuarioRepository.GetItens();
            var raioBuscaKm = 30;
            var produtosProximos = new List<Produto>();

            var usuariosProximos = todosUsuarios
                .Where(u => u.Latitude.HasValue && u.Longitude.HasValue && u.Produtos != null && u.Produtos.Any(p => !p.Excluido))
                .Where(u => Helpers.CalcularDistancia(lat, lng, u.Latitude.Value, u.Longitude.Value) <= raioBuscaKm)
                .ToList();

            foreach (var usuario in usuariosProximos)
            {
                var produtosDoUsuario = await _produtoRepository.GetByUsuarioId(usuario.Usuario_ID);
                produtosProximos.AddRange(produtosDoUsuario.Where(p => !p.Excluido)); // Garante que apenas produtos não excluídos sejam adicionados
            }

            return produtosProximos;
        }
    }
}