//using AjudeiMais.API.Interfaces;
//using AjudeiMais.Data.Context;
//using AjudeiMais.Data.Models.InstituicaoModel;
//using AjudeiMais.Data.Models.UsuarioModel;
//using Microsoft.EntityFrameworkCore;

//namespace AjudeiMais.API.Repositories
//{
//    public class EnderecoRepository : IGenericRepository<Endereco>
//    {
//        private readonly ApplicationDbContext _context;

//        public EnderecoRepository(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        public async Task Delete(int id)
//        {
//            try
//            {
//                var endereco = await _context.Endereco.FirstOrDefaultAsync(x => x.Endereco_ID == id);

//                if (endereco != null)
//                {
//                    endereco.Habilitado = false;
//                    endereco.Excluido = true;

//                    _context.Endereco.Update(endereco);
//                    await _context.SaveChangesAsync();
//                }
//            }
//            catch (Exception ex)
//            {
//                throw new Exception(ex.Message);
//            }
//        }

//        public async Task<IEnumerable<Endereco>> GetAll()
//        {
//            try
//            {
//                return await _context.Endereco.ToListAsync();
//            }
//            catch (Exception ex)
//            {
//                throw new Exception(ex.Message);
//            }
//        }

//        public async Task<Endereco> GetById(int id)
//        {
//            try
//            {
//                return await _context.Endereco.FirstOrDefaultAsync(x => x.Endereco_ID == id);
//            }
//            catch (Exception ex)
//            {
//                throw new Exception(ex.Message);
//            }
//        }

//        public async Task<IEnumerable<Endereco>> GetItens()
//        {
//            try
//            {
//                return await _context.Endereco
//                    .Where(x => x.Habilitado == true && x.Excluido != true)
//                    .ToListAsync();
//            }
//            catch (Exception ex)
//            {
//                throw new Exception(ex.Message);
//            }
//        }

//        public async Task SaveOrUpdate(Endereco model)
//        {
//            if (model.Endereco_ID > 0)
//            {
//                _context.Endereco.Update(model);
//            }
//            else
//            {
//                model.Habilitado = true;
//                _context.Endereco.Add(model);
//            }

//            await _context.SaveChangesAsync();
//        }
//    }
//}
