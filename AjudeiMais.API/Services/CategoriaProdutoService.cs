using AjudeiMais.API.DTO;
using AjudeiMais.API.Repositories;
using AjudeiMais.API.Tools;
using AjudeiMais.Data.Models.ProdutoModel;
using AjudeiMais.Data.Models.UsuarioModel;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
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

        public async Task<ApiResponse<CategoriaProduto>> GetById(int id)
        {
            try
            {
                var categoriaProduto = await _categoriaProdutoRepository.GetById(id);

                return new ApiResponse<CategoriaProduto>
                {
                    Success = true,
                    Type = "success",
                    Message = "Categoria do produto editada com sucesso.",
                    Data = categoriaProduto
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<CategoriaProduto>
                {
                    Success = false,
                    Type = "error",
                    Message = "Ocorreu um erro inesperado ao editar a categoria. Por favor, tente novamente. Se o problema persistir, entre em contato com nosso suporte."
                };
            }
        }

        public async Task<ApiResponse<IEnumerable<CategoriaProduto>>> GetItens()
        {
            try
            {
                IEnumerable<CategoriaProduto> categorias = await _categoriaProdutoRepository.GetItens();

                return new ApiResponse<IEnumerable<CategoriaProduto>>
                {
                    Success = true,
                    Data = categorias
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<IEnumerable<CategoriaProduto>>
                {
                    Success = false,
                    Type = "error",
                    Message = "Ocorreu um erro inesperado ao buscar as categorias. Por favor, tente novamente. Se o problema persistir, entre em contato com nosso suporte."
                };
            }
        }

        //        public async Task<IEnumerable<CategoriaProduto>> GetItens()
        //        {
        //            try
        //            {
        //                return await _categoriaProdutoRepository.GetItens();
        //            }
        //            catch (Exception ex)
        //            {
        //                _logger.LogError(ex, "Erro ao buscar todas as categorias");
        //                throw new Exception("Erro ao buscar categorias.");
        //            }
        //        }

        public async Task<ApiResponse<CategoriaProduto>> SaveOrUpdate(CategoriaProdutoDTO model)
        {
            try
            {
                CategoriaProduto categoriaProduto;

                if (model.CategoriaProduto_ID > 0)
                {
                    categoriaProduto = await _categoriaProdutoRepository.GetById((int)model.CategoriaProduto_ID);

                    if (categoriaProduto == null)
                    {
                        return new ApiResponse<CategoriaProduto>
                        {
                            Success = false,
                            Type = "warning",
                            Message = "Categoria não encontrada para atualização."
                        };
                    }

                    categoriaProduto.Nome = model.Nome ?? categoriaProduto.Nome;
                    categoriaProduto.Icone = model.Icone ?? categoriaProduto.Icone;
                    categoriaProduto.Habilitado = (bool)model.Habilitado;
                    categoriaProduto.DataUpdate = DateTime.Now;

                    await _categoriaProdutoRepository.SaveOrUpdate(categoriaProduto);
                }
                else
                {
                    categoriaProduto = new CategoriaProduto
                    {
                        Nome = model.Nome,
                        Icone = model.Icone,
                        Excluido = false,
                        Habilitado = (bool)model.Habilitado,
                        DataCadastro = DateTime.Now
                    };

                    await _categoriaProdutoRepository.SaveOrUpdate(categoriaProduto);
                }

                return new ApiResponse<CategoriaProduto>
                {
                    Success = true,
                    Type = "success",
                    Message = (model.CategoriaProduto_ID == 0)
                        ? "Categoria do Produto cadastrada com sucesso."
                        : "Categoria do Produto atualizada com sucesso.",
                    Data = categoriaProduto,
                };
            }
            catch (Exception ex)
            {
                // Aqui você pode logar o erro com algum logger
                return new ApiResponse<CategoriaProduto>
                {
                    Success = false,
                    Type = "error",
                    Message = "Ocorreu um erro inesperado ao salvar ou atualizar a categoria do produto. Por favor, tente novamente. Se o problema persistir, entre em contato com nosso suporte."
                };
            }
        }


        public async Task<ApiResponse<object>> Delete(int id)
        {
            try
            {
                var categoriaProduto = await _categoriaProdutoRepository.GetById(id);

                if (categoriaProduto != null)
                {
                    categoriaProduto.Habilitado = false;
                    categoriaProduto.Excluido = true;
                    categoriaProduto.DataUpdate = DateTime.Now;


                    await _categoriaProdutoRepository.Delete(categoriaProduto); 

                    return new ApiResponse<object>
                    {
                        Success = true,
                        Type = "success",
                        Message = "Categoria excluída com sucesso."
                    };
                }
                else
                {
                    // Resource not found scenario
                    return new ApiResponse<object>
                    {
                        Success = false, 
                        Type = "error",
                        Message = "Categoria do produto não encontrada. Por favor, verifique o ID e tente novamente."
                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>
                {
                    Success = false,
                    Type = "error",
                    Message = "Ocorreu um erro inesperado ao excluir a categoria do produto. Se o problema persistir, entre em contato com nosso suporte."
                };
            }
        }
    }
}
