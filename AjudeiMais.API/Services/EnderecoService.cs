using AjudeiMais.API.Repositories;
using AjudeiMais.Data.Models.InstituicaoModel;

namespace AjudeiMais.API.Services
{
    public class EnderecoService
    {
        private readonly EnderecoRepository _enderecoRepository;
        private readonly ILogger<EnderecoService> _logger;

        public EnderecoService(EnderecoRepository enderecoRepository, ILogger<EnderecoService> logger)
        {
            _enderecoRepository = enderecoRepository;
            _logger = logger;
        }

        public async Task<Endereco> GetById(int id)
        {
            try
            {
                return await _enderecoRepository.GetById(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar endereço");
                throw new Exception("Erro ao buscar endereço: " + ex.Message);
            }
        }

        public async Task<IEnumerable<Endereco>> GetAll()
        {
            try
            {
                return await _enderecoRepository.GetAll();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos os endereços");
                throw new Exception("Erro ao buscar todos os endereços: " + ex.Message);
            }
        }

        public async Task<IEnumerable<Endereco>> GetItens()
        {
            try
            {
                return await _enderecoRepository.GetItens();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar endereços ativos");
                throw new Exception("Erro ao buscar endereços ativos: " + ex.Message);
            }
        }

        public async Task SaveOrUpdate(Endereco model)
        {
            try
            {
                await _enderecoRepository.SaveOrUpdate(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar ou atualizar endereço");
                throw new Exception("Erro ao salvar ou atualizar endereço: " + ex.Message);
            }
        }

        public async Task Delete(int id)
        {
            try
            {
                await _enderecoRepository.Delete(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar endereço");
                throw new Exception("Erro ao deletar endereço: " + ex.Message);
            }
        }
    }
}
