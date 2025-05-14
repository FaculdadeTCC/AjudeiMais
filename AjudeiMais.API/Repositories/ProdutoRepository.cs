using AjudeiMais.API.Interfaces;
using AjudeiMais.Data.Context;
using AjudeiMais.Data.Models.ProdutoModel;
using Microsoft.EntityFrameworkCore;

namespace AjudeiMais.API.Repositories
{
    public class ProdutoRepository : IGenericRepository<Produto>
    {
        private readonly ApplicationDbContext _context;
        private readonly ProdutoImagemRepository _produtoImagemRepository;

        public ProdutoRepository(ApplicationDbContext context, ProdutoImagemRepository produtoImagemRepository)
        {
            _context = context;
            _produtoImagemRepository = produtoImagemRepository;
        }

        public async Task<Produto> GetById(int id)
        {
            var produto = await _context.Produto
                .FirstOrDefaultAsync(p => p.Produto_ID == id);

            return produto;
        }

        public async Task<IEnumerable<Produto>> GetAll()
        {
            return await _context.Produto.ToListAsync();
        }

        public async Task<IEnumerable<Produto>> GetItens()
        {
            return await _context.Produto
                .Where(u => u.Habilitado == true && u.Excluido != true)
                .ToListAsync();
        }

        public async Task SaveOrUpdate(Produto model)
        {
            if (model.Produto_ID > 0)
            {
                // Atualiza o produto
                _context.Produto.Update(model);
            }
            else
            {
                // Criação do novo produto
                model.DataCriacao = DateTime.Now;
                model.Habilitado = true;
                model.Excluido = false;

                foreach (var item in model.ProdutoImagens)
                {
                    item.Habilitado = true;
                    item.Excluido = false;
                }

                _context.Produto.Add(model);

                await _context.SaveChangesAsync(); 
            }
        }


        public async Task Delete(int id)
        {
            var produto = await _context.Produto
                .FirstOrDefaultAsync(p => p.Produto_ID == id);

            if (produto != null)
            {
                produto.Habilitado = false;
                produto.Excluido = true;
                produto.DataUpdate = DateTime.Now;

                _context.Produto.Update(produto);

                await _context.SaveChangesAsync();
            }
        }
    }
}
