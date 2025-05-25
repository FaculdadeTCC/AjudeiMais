//using AjudeiMais.API.Interfaces;
//using AjudeiMais.Data.Context;
//using AjudeiMais.Data.Models.InstituicaoModel;
//using Microsoft.EntityFrameworkCore;

//namespace AjudeiMais.API.Repositories
//{
//    public class CategoriaRepository : IGenericRepository<Categoria>
//    {
//        private readonly ApplicationDbContext _context;

//        public CategoriaRepository(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        public async Task Delete(int id)
//        {
//            try
//            {
//                var categoria = await _context.Categoria.FirstOrDefaultAsync(x => x.Categoria_ID == id);

//                if (categoria != null)
//                {
//                    categoria.Habilitado = false;
//                    categoria.Excluido = true;

//                    _context.Categoria.Update(categoria);
//                    await _context.SaveChangesAsync();
//                }
//            }
//            catch (Exception ex)
//            {
//                throw new Exception(ex.Message);
//            }
//        }

//        public async Task<IEnumerable<Categoria>> GetAll()
//        {
//            try
//            {
//                return await _context.Categoria.ToListAsync();
//            }
//            catch (Exception ex)
//            {
//                throw new Exception(ex.Message);
//            }
//        }

//        public async Task<Categoria> GetById(int id)
//        {
//            try
//            {
//                return await _context.Categoria.FirstOrDefaultAsync(x => x.Categoria_ID == id);
//            }
//            catch (Exception ex)
//            {
//                throw new Exception(ex.Message);
//            }
//        }

//        public async Task<IEnumerable<Categoria>> GetItens()
//        {
//            try
//            {
//                return await _context.Categoria
//                    .Where(x => x.Habilitado == true && x.Excluido != true)
//                    .ToListAsync();
//            }
//            catch (Exception ex)
//            {
//                throw new Exception(ex.Message);
//            }
//        }

//        public async Task SaveOrUpdate(Categoria model)
//        {
//            if (model.Categoria_ID > 0)
//            {
//                _context.Categoria.Update(model);
//            }
//            else
//            {
//                model.Habilitado = true;

//                await _context.Categoria.AddAsync(model);
//            }

//            await _context.SaveChangesAsync();
//        }
//    }
//}
