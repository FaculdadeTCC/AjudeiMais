using AjudeiMais.API.Repositories;
using AjudeiMais.Data.Models.ProdutoModel;
using Microsoft.Extensions.Logging;

namespace AjudeiMais.API.Services
{
    public class CategoriaProdutoService
    {
        private readonly CategoriaProdutoRepository _categoriaProdutoRepository;
        private readonly ILogger<CategoriaProdutoService> _logger;

        public CategoriaProdutoService(
            CategoriaProdutoRepository categoriaProdutoRepository,
            ILogger<CategoriaProdutoService> logger)
        {
            _categoriaProdutoRepository = categoriaProdutoRepository;
            _logger = logger;
        }

        public async Task<CategoriaProduto> GetById(int id)
        {
            try
            {
                var categoriaProduto = await _categoriaProdutoRepository.GetById(id);
                return categoriaProduto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar categoria por ID");
                throw new Exception("Erro ao buscar categorias.");
            }
        }

        public async Task<IEnumerable<CategoriaProduto>> GetAll()
        {
            try
            {
                return await _categoriaProdutoRepository.GetAll();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todas as categorias");
                throw new Exception("Erro ao buscar categorias.");
            }
        }

        public async Task<IEnumerable<CategoriaProduto>> GetItens()
        {
            try
            {
                return await _categoriaProdutoRepository.GetItens();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todas as categorias");
                throw new Exception("Erro ao buscar categorias.");
            }
        }

        public async Task SaveOrUpdate(CategoriaProduto model)
        {
            try
            {
                await _categoriaProdutoRepository.SaveOrUpdate(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar ou atualizar produto");
                throw new Exception("Erro ao salvar ou atualizar o produto.");
            }
        }



        public async Task Delete(int id)
        {
            try
            {
                await _categoriaProdutoRepository.Delete(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir o categoria com ID {CategoriaProduto_Id}", id);
                throw new Exception("Erro ao excluir a categoria.");
            }
        }
    }
}
