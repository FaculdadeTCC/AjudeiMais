using AjudeiMais.API.Interfaces;
using AjudeiMais.Data.Context;
using AjudeiMais.Data.Models.InstituicaoModel;
using Microsoft.EntityFrameworkCore;

namespace AjudeiMais.API.Repositories
{
    public class InstituicaoCategoriaRepository : IGenericRepository<InstituicaoCategoria>
    {
        private readonly ApplicationDbContext _context;

        public InstituicaoCategoriaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InstituicaoCategoria>> GetAll()
        {
            return await _context.InstituicaoCategoria
                .Include(ci => ci.Categoria)
                .Include(ci => ci.Instituicao)
                .ToListAsync();
        }

        public async Task<IEnumerable<InstituicaoCategoria>> GetItens()
        {
            return await _context.InstituicaoCategoria
                .Include(ci => ci.Categoria)
                .Include(ci => ci.Instituicao)
                .ToListAsync();
        }

        public async Task<InstituicaoCategoria> GetById(int id)
        {
            return await _context.InstituicaoCategoria.FirstOrDefaultAsync(ci => ci.Categoria_ID == id);
        }

        public async Task SaveOrUpdate(InstituicaoCategoria model)
        {
            if (_context.InstituicaoCategoria.Any(x =>
                x.Categoria_ID == model.Categoria_ID &&
                x.Instituicao_ID == model.Instituicao_ID))
            {
                _context.InstituicaoCategoria.Update(model);
            }
            else
            {
                _context.InstituicaoCategoria.Add(model);
            }

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var item = await _context.InstituicaoCategoria
                .FirstOrDefaultAsync(x => x.Categoria_ID == id);

            if (item != null)
            {
                _context.InstituicaoCategoria.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
    }
}
