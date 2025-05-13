using AjudeiMais.API.Repositories;
using AjudeiMais.Data.Models.PedidoModel;

namespace AjudeiMais.API.Services
{
    public class PedidoService
    {
        private readonly PedidoRepository _pedidoRepository;
        private readonly ILogger<PedidoService> _logger;

        public PedidoService(PedidoRepository pedidoRepository, ILogger<PedidoService> logger)
        {
            _pedidoRepository = pedidoRepository;
            _logger = logger;
        }

        public async Task<Pedido> GetById(int id)
        {
            try
            {
                return await _pedidoRepository.GetById(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedido");
                throw new Exception("Erro ao buscar pedido: " + ex.Message);
            }
        }

        public async Task<IEnumerable<Pedido>> GetAll()
        {
            try
            {
                return await _pedidoRepository.GetAll();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos os pedidos");
                throw new Exception("Erro ao buscar todos os pedidos: " + ex.Message);
            }
        }

        public async Task<IEnumerable<Pedido>> GetByInstituicaoId(int Id)
        {
            try
            {
                return await _pedidoRepository.GetByInstituicaoId(Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos os pedidos dessa instituição");
                throw new Exception("Erro ao buscar todos os pedidos dessa instituição: " + ex.Message);
            }
        }


        public async Task<IEnumerable<Pedido>> GetItens()
        {
            try
            {
                return await _pedidoRepository.GetItens();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedidos ativos");
                throw new Exception("Erro ao buscar pedidos ativos: " + ex.Message);
            }
        }

        public async Task SaveOrUpdate(Pedido model)
        {
            try
            {
                model.DataUpdate = DateTime.Now;
                await _pedidoRepository.SaveOrUpdate(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar ou atualizar pedido");
                throw new Exception("Erro ao salvar ou atualizar pedido: " + ex.Message);
            }
        }

        public async Task Delete(int id)
        {
            try
            {
                await _pedidoRepository.Delete(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar pedido");
                throw new Exception("Erro ao deletar pedido: " + ex.Message);
            }
        }
    }
}
