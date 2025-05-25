//using AjudeiMais.API.Repositories;
//using AjudeiMais.Data.Models.InstituicaoModel;
//using Microsoft.Extensions.Logging;

//namespace AjudeiMais.API.Services
//{
//    public class CategoriaService
//    {
//        private readonly CategoriaRepository _categoriaRepository;
//        private readonly ILogger<CategoriaService> _logger;

//        public CategoriaService(CategoriaRepository categoriaRepository, ILogger<CategoriaService> logger)
//        {
//            _categoriaRepository = categoriaRepository;
//            _logger = logger;
//        }

//        public async Task<Categoria> GetById(int id)
//        {
//            try
//            {
//                return await _categoriaRepository.GetById(id);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Erro ao buscar categoria por ID");
//                throw new Exception("Erro ao buscar categoria por ID: " + ex.Message);
//            }
//        }

//        public async Task<IEnumerable<Categoria>> GetAll()
//        {
//            try
//            {
//                return await _categoriaRepository.GetAll();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Erro ao buscar todas as categorias");
//                throw new Exception("Erro ao buscar todas as categorias: " + ex.Message);
//            }
//        }

//        public async Task<IEnumerable<Categoria>> GetItens()
//        {
//            try
//            {
//                return await _categoriaRepository.GetItens();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Erro ao buscar categorias ativas");
//                throw new Exception("Erro ao buscar categorias ativas: " + ex.Message);
//            }
//        }

//        public async Task SaveOrUpdate(Categoria model)
//        {
//            try
//            {
//                await _categoriaRepository.SaveOrUpdate(model);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Erro ao salvar ou atualizar categoria");
//                throw new Exception("Erro ao salvar ou atualizar categoria: " + ex.Message);
//            }
//        }

//        public async Task Delete(int id)
//        {
//            try
//            {
//                await _categoriaRepository.Delete(id);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Erro ao excluir categoria");
//                throw new Exception("Erro ao excluir categoria: " + ex.Message);
//            }
//        }
//    }
//}
