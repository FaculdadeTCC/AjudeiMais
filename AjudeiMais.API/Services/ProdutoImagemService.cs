using AjudeiMais.API.Models;
using AjudeiMais.API.Repositories;
using AjudeiMais.API.Tools;
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


        public async Task SaveOrUpdate(ProdutoImagemUploadDto dto)
        {
            ProdutoImagem entidade;

            if (dto.ProdutoImagem_ID > 0)
            {
                entidade = await _produtoImagemRepository.GetById(dto.ProdutoImagem_ID);
                if (entidade == null)
                    throw new Exception("ProdutoImagem não encontrada");
            }
            else
            {
                entidade = new ProdutoImagem
                {
                    Habilitado = true,
                    Excluido = false,
                    Produto_ID = dto.ProdutoId
                };
            }

            if (dto.Imagem != null && dto.Imagem.Length > 0)
            {
                var caminho = await Helpers.SalvarImagemComoWebpAsync(
                    dto.Imagem,
                    new[] { "images", "products", dto.ProdutoId.ToString() }
                );

                entidade.Imagem = caminho;
            }

            await _produtoImagemRepository.SaveOrUpdate(entidade);
        }
    }
}

