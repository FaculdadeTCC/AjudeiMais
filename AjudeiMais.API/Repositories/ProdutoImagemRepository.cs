using AjudeiMais.API.Interfaces;
using AjudeiMais.Data.Context;
using AjudeiMais.Data.Models.ProdutoModel;
using AjudeiMais.Data.Models.UsuarioModel;
using Microsoft.EntityFrameworkCore;

namespace AjudeiMais.API.Repositories
{
    public class ProdutoImagemRepository : IGenericRepository<ProdutoImagem>
    {
        private readonly ApplicationDbContext _context;

        public ProdutoImagemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Delete(int id)
        {
            var imagem = await _context.ProdutoImagem
                .FirstOrDefaultAsync(p => p.ProdutoImagem_ID == id);

            if (imagem != null)
            {
                imagem.Excluido = true;
                imagem.Habilitado = false;

                _context.ProdutoImagem.Update(imagem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ProdutoImagem>> GetImagensPorProduto(int produtoId)
        {
            return await _context.ProdutoImagem
                .Where(p => p.Produto_ID == produtoId && p.Habilitado && !p.Excluido)
                .ToListAsync();
        }

        public async Task<ProdutoImagem> GetById(int id)
        {
            return await _context.ProdutoImagem.FindAsync(id);
        }

        public async Task<IEnumerable<ProdutoImagem>> GetAll()
        {
            return await _context.ProdutoImagem
                .ToListAsync();
        }

        public async Task<IEnumerable<ProdutoImagem>> GetItens()
        {
            return await _context.ProdutoImagem
                .Where(u => u.Habilitado == true && u.Excluido != true)
                .ToListAsync();
        }

        public async Task SaveOrUpdate(ProdutoImagem model)
        {
            if (model.ProdutoImagem_ID > 0)
            {
                _context.ProdutoImagem.Update(model);
            }
            else
            {
                model.Habilitado = true;
                model.Excluido = false;

                await _context.ProdutoImagem.AddAsync(model);
            }

            await _context.SaveChangesAsync();
        }
    }
}
