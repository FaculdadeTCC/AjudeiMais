using AjudeiMais.API.Repositories;
using AjudeiMais.Data.Models.InstituicaoModel;

namespace AjudeiMais.API.Services
{
    public class InstituicaoService
    {
        private readonly InstituicaoRepository _instituicaoRepository;
        private readonly ILogger<InstituicaoService> _logger;

        public InstituicaoService(InstituicaoRepository instituicaoRepository, ILogger<InstituicaoService> logger)
        {
            _instituicaoRepository = instituicaoRepository;
            _logger = logger;
        }

        public async Task<Instituicao> GetById(int id)
        {
            try
            {
                return await _instituicaoRepository.GetById(id);
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar instituição");
                throw new Exception("Erro ao buscar instituição" + ex.Message);
            }
        }

        public async Task<IEnumerable<Instituicao>> GetAll()
        {
            try
            {
                return await _instituicaoRepository.GetAll();
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todas as instituições");
                throw new Exception("Erro ao buscar todas as instituições" + ex.Message);
            }
        }

        public async Task<IEnumerable<Instituicao>> GetItens()
        {
            try
            {
                return await _instituicaoRepository.GetItens();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todas as instituições ativas");
                throw new Exception("Erro ao buscar todas as instituições ativas" + ex.Message);
            }
        }

        public async Task SaveOrUpdate(Instituicao model)
        {
            try
            {
                model.DataUpdate = DateTime.Now;
                await _instituicaoRepository.SaveOrUpdate(model);
            } 
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar ou atuzalizar institiuição");
                throw new Exception("Erro ao salvar ou atuzalizar institiuição" + ex.Message);
            }
        }

        public async Task Delete(int Id)
        {
            try
            {
                await _instituicaoRepository.Delete(Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar institiuição");
                throw new Exception("Erro deletar institiuição" + ex.Message);
            }
        }


    }
}
