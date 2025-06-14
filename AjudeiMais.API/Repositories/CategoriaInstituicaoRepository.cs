using AjudeiMais.API.Interfaces;
using AjudeiMais.Data.Context;
using AjudeiMais.Data.Models.InstituicaoModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AjudeiMais.API.Repositories
{
    public class CategoriaInstituicaoRepository : IGenericRepository<CategoriaInstituicao>
    {
        private readonly ApplicationDbContext _context;

        public CategoriaInstituicaoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Delete(CategoriaInstituicao model)
        {
            try
            {
                _context.CategoriaInstituicao.Update(model); 
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<CategoriaInstituicao>> GetAll()
        {
            try
            {
                return await _context.CategoriaInstituicao.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<CategoriaInstituicao> GetById(int id)
        {
            try
            {
                return await _context.CategoriaInstituicao.FirstOrDefaultAsync(x => x.Categoria_ID == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<CategoriaInstituicao> GetByName(string nome)
        {
            try
            {
                return await _context.CategoriaInstituicao.FirstOrDefaultAsync(x => x.Nome == nome);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

		public async Task<IEnumerable<CategoriaInstituicao>> GetItens()
		{
			try
			{
				return await _context.CategoriaInstituicao
					.Where(x => x.Habilitado == true && x.Excluido != true)
					.OrderByDescending(x => x.DataCriacao) // Ordena do mais novo pro mais antigo
					.ToListAsync();
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}


		public async Task SaveOrUpdate(CategoriaInstituicao model)
        {
            if (model.Categoria_ID > 0)
            {
                _context.CategoriaInstituicao.Update(model);
            }
            else
            {
                model.Habilitado = true;

                await _context.CategoriaInstituicao.AddAsync(model);
            }

            await _context.SaveChangesAsync();
        }
    }
}
