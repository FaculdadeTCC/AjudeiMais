using AjudeiMais.API.Interfaces;
using AjudeiMais.Data.Context;
using AjudeiMais.Data.Models.UsuarioModel;
using Microsoft.EntityFrameworkCore;

namespace AjudeiMais.API.Repositories
{
    public class UsuarioRepository : IGenericRepository<Usuario>
    {
        private readonly ApplicationDbContext _context;

        public UsuarioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario> GetById(int id)
        {
            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(u => u.Usuario_ID == id);
            return usuario;
        }

        public async Task<Usuario> GetByGUID(string GUID)
        {
            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(u => u.GUID == GUID);
            return usuario;
        }

        public async Task<Usuario> GetByEmail(string email)
        {
            var usuario = await _context.Usuario.FirstOrDefaultAsync(u => u.Email == email && u.Excluido != true);

            return usuario;
        }

        public async Task<IEnumerable<Usuario>> GetAll()
        {
            return await _context.Usuario.ToListAsync();
        }

        public async Task<IEnumerable<Usuario>> GetItens()
        {
            return await _context.Usuario
                .Where(u => u.Habilitado == true && u.Excluido != true)
                .ToListAsync();
        }

        public async Task SaveOrUpdate(Usuario model)
        {
            if (model.Usuario_ID > 0)
            {
                _context.Usuario.Update(model);
            }
            else
            {
                _context.Usuario.Add(model);

                await _context.SaveChangesAsync();
            }

        }

        public async Task Delete(Usuario usuario)
        {
            _context.Usuario.Update(usuario);

            await _context.SaveChangesAsync();
        }
    }
}
