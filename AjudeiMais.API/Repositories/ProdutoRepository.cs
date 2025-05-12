using AjudeiMais.API.Interfaces;
using AjudeiMais.Data.Context;
using AjudeiMais.Data.Models.ProdutoModel;
using Microsoft.EntityFrameworkCore;

namespace AjudeiMais.API.Repositories
{
    public class ProdutoRepository : IGenericRepository<Produto>
    {
        private readonly ApplicationDbContext _context;

        public ProdutoRepository(ApplicationDbContext context)
        {
            _context = context;
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

                // Desanexa as imagens do produto para evitar salvar antes
                var imagens = model.ProdutoImagens?.ToList();
                model.ProdutoImagens = null; // Desvincula as imagens temporariamente

                // Adiciona o produto ao banco
                _context.Produto.Add(model);
                await _context.SaveChangesAsync(); // O ID do produto será gerado aqui após o SaveChangesAsync()

                // Agora que o produto tem um ID, associa as imagens ao produto
                if (imagens != null && imagens.Any())
                {
                    foreach (var img in imagens)
                    {
                        img.Produto_ID = model.Produto_ID; // Agora o ID do produto é conhecido
                        img.Habilitado = true;
                        img.Excluido = false;
                    }

                    // Salva as imagens associadas ao produto
                    _context.ProdutoImagem.AddRange(imagens);
                    await _context.SaveChangesAsync(); // Salva as imagens no banco
                }
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
