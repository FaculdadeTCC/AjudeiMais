using System.Runtime.ConstrainedExecution;
using AjudeiMais.API.DTO;
using AjudeiMais.API.Models;
using AjudeiMais.API.Repositories;
using AjudeiMais.API.Tools;
using AjudeiMais.Data.Models.ProdutoModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Azure.Core.HttpHeader;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public async Task<ApiResponse<IEnumerable<ProdutoImagem>>> GetImagensPorProduto(int produtoId)
        {
            // 1. Validação básica do ID
            if (produtoId <= 0)
            {
                return new ApiResponse<IEnumerable<ProdutoImagem>> // Cria a instância
                {
                    Success = false, // Define o sucesso como falso
                    Message = "ID do produto inválido.",
                    Type = "validation_error",
                    Errors = new List<string> { "ID do produto inválido." } // Adiciona o erro à lista
                };
            }

            try
            {
                // 2. Busca as imagens no repositório
                var imagens = await _produtoImagemRepository.GetImagensPorProduto(produtoId);

                // 3. Lida com casos onde nenhuma imagem é encontrada
                if (imagens == null || !imagens.Any())
                {
                    // Retorna sucesso, mas com lista vazia e mensagem informativa
                    return new ApiResponse<IEnumerable<ProdutoImagem>> // Cria a instância
                    {
                        Success = true, // Ainda é sucesso, mas sem dados
                        Data = new List<ProdutoImagem>(), // Dados vazios
                        Message = "Nenhuma imagem encontrada para este produto.",
                        Type = "not_found" // Tipo específico para "não encontrado"
                    };
                }

                // 4. Retorna sucesso com as imagens encontradas
                return new ApiResponse<IEnumerable<ProdutoImagem>> // Cria a instância
                {
                    Success = true, // Sucesso
                    Data = imagens, // Os dados
                    Message = "Imagens do produto retornadas com sucesso.",
                    Type = "success" // Tipo de sucesso
                };
            }
            catch (Exception ex)
            {
                // 5. Captura erros inesperados
                // Em uma aplicação real, você deve logar 'ex' com um logger adequado
                Console.WriteLine($"[ERRO SERVICE] Erro ao buscar imagens para o produto {produtoId}: {ex.Message}");
                return new ApiResponse<IEnumerable<ProdutoImagem>> // Cria a instância
                {
                    Success = false, // Erro
                    Message = $"Ocorreu um erro interno ao buscar as imagens: {ex.Message}",
                    Type = "server_error", // Tipo de erro de servidor
                    Errors = new List<string> { $"Ocorreu um erro interno ao buscar as imagens: {ex.Message}" } // Adiciona o erro
                };
            }
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

