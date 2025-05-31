using AjudeiMais.API.Interfaces;
using AjudeiMais.Data.Context;
using AjudeiMais.Data.Models.InstituicaoModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AjudeiMais.API.Repositories
{
    public class CategoriaInstituicaoRepository : IGenericRepository<Categoria>
    {
        private readonly ApplicationDbContext _context;

        public CategoriaInstituicaoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Delete(Categoria model)
        {
            try
            {
                _context.Categoria.Update(model); 
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<Categoria>> GetAll()
        {
            try
            {
                return await _context.Categoria.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Categoria> GetById(int id)
        {
            try
            {
                return await _context.Categoria.FirstOrDefaultAsync(x => x.Categoria_ID == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Categoria> GetByName(string nome)
        {
            try
            {
                return await _context.Categoria.FirstOrDefaultAsync(x => x.Nome == nome);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<Categoria>> GetItens()
        {
            try
            {
                return await _context.Categoria
                    .Where(x => x.Habilitado == true && x.Excluido != true)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task SaveOrUpdate(Categoria model)
        {
            if (model.Categoria_ID > 0)
            {
                _context.Categoria.Update(model);
            }
            else
            {
                model.Habilitado = true;

                await _context.Categoria.AddAsync(model);
            }

            await _context.SaveChangesAsync();
        }
    }
}
