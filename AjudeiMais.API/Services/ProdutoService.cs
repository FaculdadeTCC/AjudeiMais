using AjudeiMais.API.Repositories;
using AjudeiMais.Data.Models.ProdutoModel;
using Microsoft.Extensions.Logging;

namespace AjudeiMais.API.Services
{
    public class ProdutoService
    {
        private readonly ProdutoRepository _produtoRepository;
        private readonly ProdutoImagemRepository _produtoImagemRepository;
        private readonly ILogger<ProdutoService> _logger;

        public ProdutoService(
            ProdutoRepository produtoRepository, 
            ProdutoImagemRepository produtoImagemRepository, 
            ILogger<ProdutoService> logger)
        {
            _produtoRepository = produtoRepository;
            _produtoImagemRepository = produtoImagemRepository;
            _logger = logger;
        }

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

        public async Task SaveOrUpdate(Produto model)
        {
            try
            {
                await _produtoRepository.SaveOrUpdate(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar ou atualizar o produto");
                throw new Exception("Erro ao salvar ou atualizar o produto.");
            }
        }



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
    }
}
