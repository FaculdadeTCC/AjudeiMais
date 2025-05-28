using AjudeiMais.API.Interfaces;
using AjudeiMais.Data.Context;
using AjudeiMais.Data.Models.ProdutoModel;
using Microsoft.EntityFrameworkCore;

namespace AjudeiMais.API.Repositories
{
    public class CategoriaProdutoRepository : IGenericRepository<CategoriaProduto>
    {
        private readonly ApplicationDbContext _context;

        public CategoriaProdutoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CategoriaProduto> GetById(int id)
        {
            var categoriaProduto = await _context.CategoriaProduto
                .FirstOrDefaultAsync(u => u.CategoriaProduto_ID == id);

            return categoriaProduto;
        }

        public async Task<IEnumerable<CategoriaProduto>> GetAll()
        {
            return await _context.CategoriaProduto.ToListAsync();
        }

        public async Task<IEnumerable<CategoriaProduto>> GetItens()
        {
            return await _context.CategoriaProduto
                .Where(u => u.Excluido != true)
                .ToListAsync();
        }

        public async Task SaveOrUpdate(CategoriaProduto model)
        {
            if (model.CategoriaProduto_ID > 0)
            {
                _context.CategoriaProduto.Update(model);
            }
            else
            {
                _context.CategoriaProduto.Add(model);
            }

            await _context.SaveChangesAsync();
        }

        public async Task Delete(CategoriaProduto model)
        {
            _context.CategoriaProduto.Update(model);

            await _context.SaveChangesAsync();
        }
    }
}
