using AjudeiMais.API.DTO;
using AjudeiMais.API.Repositories;
using AjudeiMais.API.Tools; // Certifique-se de que Helpers e ApiResponse estejam definidos aqui.
using AjudeiMais.Data.Models.InstituicaoModel;
using Microsoft.Extensions.Logging;
using System.Collections.Generic; // Necessário para List<string>

namespace AjudeiMais.API.Services
{
    public class InstituicaoImagemService
    {
        private readonly InstituicaoImagemRepository _instituicaoImagemRepository;
        private readonly InstituicaoRepository _instituicaoRepository;
        private readonly ILogger<InstituicaoImagemService> _logger;

        public InstituicaoImagemService(InstituicaoImagemRepository instituicaoImagemRepository, InstituicaoRepository instituicaoRepository, ILogger<InstituicaoImagemService> logger)
        {
            _instituicaoImagemRepository = instituicaoImagemRepository;
            _instituicaoRepository = instituicaoRepository;
            _logger = logger;
        }

        // --- Métodos de Consulta (GET) - Lançam exceção em caso de erro, como na UsuarioService ---

        public async Task<InstituicaoImagem?> GetById(int id) // Retorno null se não encontrar, lança exceção em erro
        {
            try
            {
                var imagem = await _instituicaoImagemRepository.GetById(id);
                if (imagem == null)
                {
                    // Se não encontrou, retorna null para o controller lidar com o NotFound
                    return null;
                }
                return imagem;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar imagem da instituição por ID: {Id}", id);
                throw new Exception("Erro ao buscar imagem da instituição.");
            }
        }

        public async Task<IEnumerable<InstituicaoImagem>> GetAll()
        {
            try
            {
                return await _instituicaoImagemRepository.GetAll();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todas as imagens da instituição");
                throw new Exception("Erro ao buscar as imagens da instituição.");
            }
        }

        public async Task<IEnumerable<InstituicaoImagem>> GetItens()
        {
            try
            {
                return await _instituicaoImagemRepository.GetItens();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar imagens ativas da instituição");
                throw new Exception("Erro ao buscar imagens ativas da instituição.");
            }
        }

        public async Task<IEnumerable<InstituicaoImagem>> GetImagensPorInstituicao(int instituicaoId)
        {
            try
            {
                return await _instituicaoImagemRepository.GetImagensPorInstituicao(instituicaoId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar imagens da instituição com ID: {InstituicaoId}", instituicaoId);
                throw new Exception("Erro ao buscar imagens da instituição.");
            }
        }

        // --- Métodos de Escrita/Atualização/Exclusão (POST/DELETE) - Retornam ApiResponse ---

        public async Task<ApiResponse<InstituicaoImagem>> SaveOrUpdate(InstituicaoImagem model)
        {
            try
            {
                await _instituicaoImagemRepository.SaveOrUpdate(model);
                var mensagem = model.InsituicaoImagem_ID == 0 ? "Imagem cadastrada com sucesso." : "Imagem atualizada com sucesso.";

                return new ApiResponse<InstituicaoImagem> // Retorna o objeto InstituicaoImagem no Data
                {
                    Success = true,
                    Type = "success",
                    Message = mensagem,
                    Data = model
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar ou atualizar a imagem da instituição");
                return new ApiResponse<InstituicaoImagem> // Ajustado para ApiResponse<InstituicaoImagem> para consistência
                {
                    Success = false,
                    Type = "error",
                    Message = "Ocorreu um erro ao salvar ou atualizar a imagem da instituição. Por favor, tente novamente.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<string>> Delete(int id) // Altera para receber int id, como no Controller
        {
            try
            {
                var foto = await _instituicaoImagemRepository.GetById(id);

                if (foto == null)
                {
                    return new ApiResponse<string>
                    {
                        Success = false,
                        Type = "error",
                        Message = "Imagem não encontrada para exclusão."
                    };
                }

                foto.Habilitado = false;
                foto.Excluido = true;

                await _instituicaoImagemRepository.Delete(foto); // Passa o objeto completo para o repositório se necessário

                return new ApiResponse<string>
                {
                    Success = true,
                    Type = "success",
                    Message = "Imagem excluída com sucesso."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir a imagem da instituição com ID: {Id}", id);
                return new ApiResponse<string>
                {
                    Success = false,
                    Type = "error",
                    Message = "Ocorreu um erro ao excluir a imagem da instituição. Por favor, tente novamente.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<ApiResponse<string>> AtualizarFotosInstituicaoAsync(AtualizarFotosInstituicaoDTO model)
        {
            try
            {
                var instituicao = await _instituicaoRepository.GetByGUID(model.Instituicao_GUID);

                if (instituicao == null)
                {
                    return new ApiResponse<string>
                    {
                        Success = false,
                        Type = "warning",
                        Message = "Instituição não encontrada."
                    };
                }

                var pasta = new[] { "images", "profile", "instituicao", model.Instituicao_GUID };

                if (model.Fotos != null && model.Fotos.Length > 0)
                {
                    foreach (var foto in model.Fotos)
                    {
                        if (foto.Length > 0 && foto.Length <= 5 * 1024 * 1024) // 5 MB
                        {
                            var path = await Helpers.SalvarImagemComoWebpAsync(foto, pasta, qualidade: 75);

                            if (!string.IsNullOrWhiteSpace(path))
                            {
                                var novaImagem = new InstituicaoImagem
                                {
                                    Instituicao_ID = instituicao.Instituicao_ID,
                                    CaminhoImagem = path
                                };
                                await _instituicaoImagemRepository.SaveOrUpdate(novaImagem);
                            }
                        }
                        else
                        {
                            // Opcional: Adicionar lógica para lidar com arquivos grandes/vazios
                            _logger.LogWarning("Uma das fotos enviadas para a instituição {GUID} excedeu o limite de tamanho ou estava vazia.", model.Instituicao_GUID);
                            // Você pode optar por retornar um erro aqui ou continuar processando as outras fotos
                        }
                    }
                }

                return new ApiResponse<string>
                {
                    Success = true,
                    Type = "success",
                    Message = "Fotos atualizadas com sucesso."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar fotos da instituição com GUID: {InstituicaoGUID}", model.Instituicao_GUID);
                return new ApiResponse<string>
                {
                    Success = false,
                    Type = "error",
                    Message = "Ocorreu um erro ao atualizar as fotos da instituição. Por favor, tente novamente.",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
    }
}