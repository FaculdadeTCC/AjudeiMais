using AjudeiMais.API.Repositories;
using AjudeiMais.Data.Models.InstituicaoModel;
using Microsoft.Extensions.Logging;

namespace AjudeiMais.API.Services
{
    public class InstituicaoImagemService
    {
        private readonly InstituicaoImagemRepository _instituicaoImagemRepository;
        private readonly ILogger<InstituicaoImagemService> _logger;

        public InstituicaoImagemService(InstituicaoImagemRepository instituicaoImagemRepository, ILogger<InstituicaoImagemService> logger)
        {
            _instituicaoImagemRepository = instituicaoImagemRepository;
            _logger = logger;
        }

        public async Task<InstituicaoImagem> GetById(int id)
        {
            try
            {
                var instituicaoImagem = await _instituicaoImagemRepository.GetById(id);
                return instituicaoImagem;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar imagem da instituição por ID");
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
                _logger.LogError(ex, "Erro ao buscar as imagens da instituição.");
                throw new Exception("Erro ao buscar as imagens da instituição.");
            }
        }

        public async Task SaveOrUpdate(InstituicaoImagem model)
        {
            try
            {
                await _instituicaoImagemRepository.SaveOrUpdate(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar ou atualizar a imagem da instituição");
                throw new Exception("Erro ao salvar ou atualizar a imagem da instituição.");
            }
        }

        public async Task Delete(int id)
        {
            try
            {
                await _instituicaoImagemRepository.Delete(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir a imagem da instituição com ID {InstituicaoImagemId}", id);
                throw new Exception("Erro ao excluir a imagem da instituição.");
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
                _logger.LogError(ex, "Erro ao buscar imagens da instituição");
                throw new Exception("Erro ao buscar imagens da instituição.");
            }
        }
    }
}
