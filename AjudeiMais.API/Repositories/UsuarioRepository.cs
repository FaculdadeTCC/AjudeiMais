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
                model.DataCriacao = DateTime.Now;
                model.Habilitado = true;
                model.GUID = Guid.NewGuid().ToString();

                _context.Usuario.Add(model);
                await _context.SaveChangesAsync();
            }

        }

        public async Task Delete(int id)
        {
            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(u => u.Usuario_ID == id);

            if (usuario != null)
            {
                usuario.Habilitado = false;
                usuario.Excluido = true;
                usuario.DataUpdate = DateTime.Now;

                _context.Usuario.Update(usuario);
                await _context.SaveChangesAsync();
            }
        }
    }
}
