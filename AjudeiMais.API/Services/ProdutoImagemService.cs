using AjudeiMais.API.Repositories;
using AjudeiMais.Data.Models.ProdutoModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AjudeiMais.API.Services
{
    public class ProdutoImagemService
    {
        private readonly ProdutoImagemRepository _produtoImagemRepository;
        private readonly ILogger<ProdutoImagemService> _logger;

        public ProdutoImagemService(ProdutoImagemRepository produtoImagemRepository, ILogger<ProdutoImagemService> logger)
        {
            _produtoImagemRepository = produtoImagemRepository;
            _logger = logger;
        }

        public async Task<ProdutoImagem> GetById(int id)
        {
            try
            {
                var produtoImagem = await _produtoImagemRepository.GetById(id);

                return produtoImagem;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar imagens de produtos por ID");
                throw new Exception("Erro ao buscar imagens de produtos.");
            }
        }

        public async Task<IEnumerable<ProdutoImagem>> GetAll()
        {
            try
            {
                return await _produtoImagemRepository.GetAll();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos as imagens de produtos");
                throw new Exception("Erro ao buscar as imagens de produtos.");
            }
        }

        public async Task<IEnumerable<ProdutoImagem>> GetItens()
        {
            try
            {
                return await _produtoImagemRepository.GetItens();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar as imagens de produtos.");
                throw new Exception("Erro ao as imagens de produtos.");
            }
        }

        public async Task SaveOrUpdate(ProdutoImagem model)
        {
            try
            {
                await _produtoImagemRepository.SaveOrUpdate(model);
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
                await _produtoImagemRepository.Delete(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir o produto com ID {ProdutoId}", id);
                throw new Exception("Erro ao excluir o produto.");
            }
        }

        public async Task<IEnumerable<ProdutoImagem>> GetImagensPorProduto(int produtoId)
        {
            return await _produtoImagemRepository.GetImagensPorProduto(produtoId);
        }
    }
}
