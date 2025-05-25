//using AjudeiMais.API.Repositories;
//using AjudeiMais.Data.Models.InstituicaoModel;

//namespace AjudeiMais.API.Services
//{
//    public class InstituicaoCategoriaService
//    {
//        private readonly InstituicaoCategoriaRepository _repository;
//        private readonly ILogger<InstituicaoCategoriaService> _logger;

//        public InstituicaoCategoriaService(InstituicaoCategoriaRepository repository, ILogger<InstituicaoCategoriaService> logger)
//        {
//            _repository = repository;
//            _logger = logger;
//        }

//        public async Task<IEnumerable<InstituicaoCategoria>> GetAll()
//        {
//            try
//            {
//                return await _repository.GetAll();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Erro ao buscar todas as relações categoria-instituição");
//                throw;
//            }
//        }

//        public async Task<IEnumerable<InstituicaoCategoria>> GetItens()
//        {
//            try
//            {
//                return await _repository.GetItens();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Erro ao buscar relações ativas categoria-instituição");
//                throw;
//            }
//        }

//        public async Task<InstituicaoCategoria> GetById(int id)
//        {
//            try
//            {
//                return await _repository.GetById(id);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Erro ao buscar relação categoria-instituição por ID");
//                throw;
//            }
//        }

//        public async Task SaveOrUpdate(InstituicaoCategoria model)
//        {
//            try
//            {
//                await _repository.SaveOrUpdate(model);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Erro ao salvar/atualizar relação categoria-instituição");
//                throw;
//            }
//        }

//        public async Task Delete(int id)
//        {
//            try
//            {
//                await _repository.Delete(id);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Erro ao deletar relação categoria-instituição");
//                throw;
//            }
//        }
//    }
//}
