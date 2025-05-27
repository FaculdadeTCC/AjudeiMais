using AjudeiMais.API.DTO;
using AjudeiMais.API.Repositories;
using AjudeiMais.API.Tools;
using AjudeiMais.Data.Models.ProdutoModel;
using AjudeiMais.Data.Models.UsuarioModel;
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

        //        public async Task<CategoriaProduto> GetById(int id)
        //        {
        //            try
        //            {
        //                var categoriaProduto = await _categoriaProdutoRepository.GetById(id);
        //                return categoriaProduto;
        //            }
        //            catch (Exception ex)
        //            {
        //                _logger.LogError(ex, "Erro ao buscar categoria por ID");
        //                throw new Exception("Erro ao buscar categorias.");
        //            }
        //        }

        //        public async Task<IEnumerable<CategoriaProduto>> GetAll()
        //        {
        //            try
        //            {
        //                return await _categoriaProdutoRepository.GetAll();
        //            }
        //            catch (Exception ex)
        //            {
        //                _logger.LogError(ex, "Erro ao buscar todas as categorias");
        //                throw new Exception("Erro ao buscar categorias.");
        //            }
        //        }

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
                string[] folder = ["products", "categories", "icons"];
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

                    if (model.Icone != null)
                    {
                        var path = await Helpers.SalvarImagemComoWebpAsync(model.Icone, folder);
                        categoriaProduto.Icone = path;
                    }

                    categoriaProduto.Excluido = model.Excluido ?? categoriaProduto.Excluido;
                    categoriaProduto.Habilitado = model.Habilitado ?? categoriaProduto.Habilitado;
                    categoriaProduto.DataUpdate = DateTime.Now;

                    await _categoriaProdutoRepository.SaveOrUpdate(categoriaProduto);
                }
                else
                {
                    var path = await Helpers.SalvarImagemComoWebpAsync(model.Icone, folder);

                    categoriaProduto = new CategoriaProduto
                    {
                        Nome = model.Nome,
                        Icone = path,
                        Excluido = false,
                        Habilitado = true,
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
                    Data = categoriaProduto
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


        //        public async Task Delete(int id)
        //        {
        //            try
        //            {
        //                await _categoriaProdutoRepository.Delete(id);
        //            }
        //            catch (Exception ex)
        //            {
        //                _logger.LogError(ex, "Erro ao excluir o categoria com ID {CategoriaProduto_Id}", id);
        //                throw new Exception("Erro ao excluir a categoria.");
        //            }
        //        }
    }
}
