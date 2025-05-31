using AjudeiMais.API.DTO;
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


        public async Task<ApiResponse<ProdutoImagem>> SaveOrUpdate(ProdutoImagemUploadDto dto)
        {
            ProdutoImagem entidade;
            bool isNovo = false;

            if (dto.ProdutoImagem_ID > 0)
            {
                entidade = await _produtoImagemRepository.GetById(dto.ProdutoImagem_ID);

                if (entidade == null)
                {
                    return new ApiResponse<ProdutoImagem>
                    {
                        Success = false,
                        Type = "error",
                        Message = "Produto não encontrado para associar a imagem."
                    };
                }
            }
            else
            {
                entidade = new ProdutoImagem
                {
                    Habilitado = true,
                    Excluido = false,
                    Produto_ID = dto.Produto_ID
                };
                isNovo = true;
            }

            if (dto.Imagem != null && dto.Imagem.Length > 0)
            {
                var caminho = await Helpers.SalvarImagemComoWebpAsync(
                    dto.Imagem,
                    new[] { "images", "products", dto.Produto_ID.ToString() }
                );

                entidade.Imagem = caminho;

                await _produtoImagemRepository.SaveOrUpdate(entidade);

                var retorno = new ProdutoImagem
                {
                    ProdutoImagem_ID = entidade.ProdutoImagem_ID,
                    Produto_ID = entidade.Produto_ID,
                    Imagem = caminho,
                    IsPrincipal = entidade.IsPrincipal
                };

                return new ApiResponse<ProdutoImagem>
                {
                    Success = true,
                    Type = "success",
                    Message = isNovo ? "Imagem cadastrada com sucesso." : "Imagem atualizada com sucesso.",
                    Data = retorno
                };
            }
            else
            {
                // Sem nova imagem, apenas salva alterações (se houver)
                await _produtoImagemRepository.SaveOrUpdate(entidade);

                var retorno = new ProdutoImagem
                {
                    ProdutoImagem_ID = entidade.ProdutoImagem_ID,
                    Produto_ID = entidade.Produto_ID,
                    Imagem = entidade.Imagem,
                    IsPrincipal = entidade.IsPrincipal
                };

                return new ApiResponse<ProdutoImagem>
                {
                    Success = true,
                    Type = "success",
                    Message = isNovo ? "Imagem cadastrada com sucesso." : "Imagem atualizada com sucesso.",
                    Data = retorno
                };
            }
        }
    }
}

