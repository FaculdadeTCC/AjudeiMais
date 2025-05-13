using AjudeiMais.API.Interfaces;
using AjudeiMais.Data.Context;
using AjudeiMais.Data.Models.PedidoModel;
using Microsoft.EntityFrameworkCore;

namespace AjudeiMais.API.Repositories
{
    public class PedidoRepository : IGenericRepository<Pedido>
    {
        private readonly ApplicationDbContext _context;

        public PedidoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Delete(int id)
        {
            try
            {
                var pedido = _context.Pedido.FirstOrDefault(x => x.Pedido_ID == id);

                if (pedido != null)
                {
                    pedido.Habilitado = false;
                    pedido.Excluido = true;
                    pedido.DataUpdate = DateTime.Now;

                    _context.Pedido.Update(pedido);

                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<Pedido>> GetAll()
        {
            try
            {
                var pedidos = await _context.Pedido.ToListAsync();
                return pedidos;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Pedido> GetById(int id)
        {
            try
            {
                var pedido = await _context.Pedido.FirstOrDefaultAsync(x => x.Pedido_ID == id);
                return pedido;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<Pedido>> GetByInstituicaoId(int Id)
        {
            try
            {
                var pedidos = await _context.Pedido.Where(x => x.Instituicao_ID == Id).ToListAsync();
                return pedidos;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<Pedido>> GetItens()
        {
            try
            {
                var pedidos = await _context.Pedido
                    .Where(x => x.Habilitado == true && x.Excluido != true)
                    .ToListAsync();

                return pedidos;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task SaveOrUpdate(Pedido model)
        {
            if (model.Pedido_ID > 0)
            {
                _context.Pedido.Update(model);
            }
            else
            {
                model.DataCriacao = DateTime.Now;
                model.Habilitado = true;

                _context.Pedido.Add(model);
            }

            await _context.SaveChangesAsync();
        }
    }
}
