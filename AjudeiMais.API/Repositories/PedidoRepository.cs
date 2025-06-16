using AjudeiMais.Data;
using AjudeiMais.Data.Context;
using AjudeiMais.Data.Models.PedidoModel;
using Microsoft.EntityFrameworkCore;

namespace AjudeiMais.API.Repositories
{
	public class PedidoRepository
	{
		private readonly ApplicationDbContext _context;

		public PedidoRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<List<Pedido>> GetAllAsync()
		{
			return await _context.Pedido
                .Include(p => p.Produto)
                    .ThenInclude(prod => prod.CategoriaProduto) // Certifique-se de incluir a categoria
                .Include(p => p.Instituicao)
                .Include(p => p.Produto)
                    .ThenInclude(prod => prod.Usuario)
                .ToListAsync();
		}

		public async Task<List<Pedido>> GetItensAsync()
		{
			return await _context.Pedido.Where(p => p.Habilitado == true && p.Excluido != true)
                .Include(p => p.Produto)
                    .ThenInclude(prod => prod.CategoriaProduto) // Certifique-se de incluir a categoria
                .Include(p => p.Instituicao)
                .Include(p => p.Produto)
                    .ThenInclude(prod => prod.Usuario)
                .ToListAsync();
		}

        public async Task<List<Pedido>> GetPedidosInstituicaoAsync(string GUID)
        {
            return await _context.Pedido.Where(p => p.Habilitado == true && p.Excluido != true && p.Instituicao.GUID == GUID)
				.Include(p => p.Produto)
					.ThenInclude(prod => prod.CategoriaProduto) // Certifique-se de incluir a categoria
				.Include(p => p.Instituicao)
				.Include(p => p.Produto)
					.ThenInclude(prod => prod.Usuario)
				.ToListAsync();
        }

        public async Task<List<Pedido>> GetPedidosUsuarioAsync(string GUID)
        {
			try {
				var response = await _context.Pedido.Where(p => p.Habilitado == true && p.Excluido != true && p.Usuario.GUID == GUID)
					.Include(p => p.Produto)
						.ThenInclude(prod => prod.CategoriaProduto) // Certifique-se de incluir a categoria
					.Include(p => p.Instituicao)
					.Include(p => p.Produto)
						.ThenInclude(prod => prod.Usuario)
					.ToListAsync();

				return response;
			} catch (DbUpdateException ex)
			{
				throw new Exception(ex.Message);
			}
        }

        public async Task<Pedido?> GetByIdAsync(int id)
		{
			return await _context.Pedido.Where(p => p.Habilitado == true && p.Excluido != true)
                .Include(p => p.Produto)
                    .ThenInclude(prod => prod.CategoriaProduto) // Certifique-se de incluir a categoria
                .Include(p => p.Instituicao)
                .Include(p => p.Produto)
                    .ThenInclude(prod => prod.Usuario)
                .FirstOrDefaultAsync(p => p.Pedido_ID == id);
		}

		public async Task<Pedido?> GetByGUIDAsync(string GUID)
		{
			return await _context.Pedido.Where(p => p.Habilitado == true && p.Excluido != true)
                .Include(p => p.Produto)
                    .ThenInclude(prod => prod.CategoriaProduto) // Certifique-se de incluir a categoria
                .Include(p => p.Instituicao)
                .Include(p => p.Produto)
                    .ThenInclude(prod => prod.Usuario)
                .FirstOrDefaultAsync(p => p.GUID == GUID);
		}

		public async Task AddAsync(Pedido pedido)
		{
			_context.Pedido.Add(pedido);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateAsync(Pedido pedido)
		{
			_context.Pedido.Update(pedido);
			await _context.SaveChangesAsync();
		}

		// Delete logico
		public async Task DeleteAsync(Pedido pedido)
		{
			_context.Pedido.Update(pedido);
			await _context.SaveChangesAsync();
		}
	}
}
