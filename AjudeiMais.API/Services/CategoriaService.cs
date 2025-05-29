using AjudeiMais.API.Repositories;
using AjudeiMais.Data.Models.InstituicaoModel; // Assumindo que Categoria está neste namespace
using Microsoft.Extensions.Logging;
using AjudeiMais.API.DTO; // Certifique-se de que o ApiResponse esteja neste namespace
using AjudeiMais.API.Tools;
namespace AjudeiMais.API.Services
{
    public class CategoriaService
    {
        private readonly CategoriaRepository _categoriaRepository;
        private readonly ILogger<CategoriaService> _logger;

        public CategoriaService(CategoriaRepository categoriaRepository, ILogger<CategoriaService> logger)
        {
            _categoriaRepository = categoriaRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<Categoria>> GetById(int id)
        {
            try
            {
                var categoria = await _categoriaRepository.GetById(id);
                if (categoria == null)
                {
                    return new ApiResponse<Categoria>
                    {
                        Success = false,
                        Type = "error",
                        Message = "Categoria não encontrada."
                    };
                }
                return new ApiResponse<Categoria>
                {
                    Success = true,
                    Type = "success",
                    Data = categoria
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar categoria por ID: {CategoriaId}", id);
                return new ApiResponse<Categoria>
                {
                    Success = false,
                    Type = "error",
                    Message = "Ocorreu um erro ao buscar a categoria. Por favor, tente novamente."
                };
            }
        }

        public async Task<ApiResponse<IEnumerable<Categoria>>> GetAll()
        {
            try
            {
                var categorias = await _categoriaRepository.GetAll();
                return new ApiResponse<IEnumerable<Categoria>>
                {
                    Success = true,
                    Type = "success",
                    Data = categorias
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todas as categorias");
                return new ApiResponse<IEnumerable<Categoria>>
                {
                    Success = false,
                    Type = "error",
                    Message = "Ocorreu um erro ao buscar as categorias. Por favor, tente novamente."
                };
            }
        }

        public async Task<ApiResponse<IEnumerable<Categoria>>> GetItens()
        {
            try
            {
                var categoriasAtivas = await _categoriaRepository.GetItens();
                return new ApiResponse<IEnumerable<Categoria>>
                {
                    Success = true,
                    Type = "success",
                    Data = categoriasAtivas
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar categorias ativas");
                return new ApiResponse<IEnumerable<Categoria>>
                {
                    Success = false,
                    Type = "error",
                    Message = "Ocorreu um erro ao buscar as categorias ativas. Por favor, tente novamente."
                };
            }
        }

        public async Task<ApiResponse<Categoria>> SaveOrUpdate(CategoriaDTO model)
        {
            try
            {
                // Aqui você faria o mapeamento de CategoriaDTO para Categoria
                // Para manter o exemplo conciso, estou assumindo que CategoriaDTO tem as mesmas propriedades de Categoria
                // E que a lógica de "atualizar" vs "salvar" é tratada no repositório ou com uma verificação de ID.

                
                model.GUID ??= Guid.NewGuid().ToString();
                var pasta = new[] { "images", "profile", "instituicao", model.GUID };
                string pathIcone = await Helpers.SalvarImagemComoWebpAsync(model.Icone, pasta);

                var categoria = new Categoria
                {
                    Categoria_ID = model.Categoria_ID,
                    Nome = model.Nome,
                    Icone = pathIcone,
                    Habilitado = model.Habilitado,
                    Excluido = false
                    // Mapeie outras propriedades conforme necessário
                };

                // Exemplo de lógica para verificar se a categoria já existe (opcional, dependendo da sua regra de negócio)
                // Se sua regra permite categorias com o mesmo nome, pode remover esta verificação.
                var existingCategory = await _categoriaRepository.GetByName(model.Nome);
                if (existingCategory != null && existingCategory.Categoria_ID != model.Categoria_ID)
                {
                    return new ApiResponse<Categoria>
                    {
                        Success = false,
                        Type = "error",
                        Message = "Já existe uma categoria com este nome."
                    };
                }


                await _categoriaRepository.SaveOrUpdate(categoria);

                string message = model.Categoria_ID > 0 ? "Categoria atualizada com sucesso." : "Categoria cadastrada com sucesso.";

                return new ApiResponse<Categoria>
                {
                    Success = true,
                    Type = "success",
                    Message = message,
                    Data = categoria
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar ou atualizar categoria");
                return new ApiResponse<Categoria>
                {
                    Success = false,
                    Type = "error",
                    Message = "Ocorreu um erro inesperado ao salvar ou atualizar a categoria. Por favor, tente novamente."
                };
            }
        }

        public async Task<ApiResponse<Categoria>> Delete(int id)
        {
            try
            {
                var categoria = await _categoriaRepository.GetById(id);
                if (categoria == null)
                {
                    return new ApiResponse<Categoria>
                    {
                        Success = false,
                        Type = "error",
                        Message = "Categoria não encontrada."
                    };
                }

                // Assumindo que a exclusão é lógica (habilitado = false, excluido = true)
                categoria.Habilitado = false; // Exemplo de desabilitação
                categoria.Excluido = true;    // Exemplo de marcação como excluída
                // categoria.DataUpdate = DateTime.Now; // Se houver um campo DataUpdate

                await _categoriaRepository.Delete(categoria); // Ou um método específico para exclusão lógica

                return new ApiResponse<Categoria>
                {
                    Success = true,
                    Type = "success",
                    Message = "Categoria excluída com sucesso."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir categoria com ID: {CategoriaId}", id);
                return new ApiResponse<Categoria>
                {
                    Success = false,
                    Type = "error",
                    Message = "Não foi possível excluir a categoria. Por favor, tente novamente. Se o problema persistir, entre em contato com nosso suporte."
                };
            }
        }
    }
}