//using AjudeiMais.API.Interfaces;
//using AjudeiMais.Data.Context;
//using AjudeiMais.Data.Models.ProdutoModel;
//using Microsoft.EntityFrameworkCore;

//namespace AjudeiMais.API.Repositories
//{
//    public class ProdutoRepository : IGenericRepository<Produto>
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly ProdutoImagemRepository _produtoImagemRepository; // Injetado, mas não usado diretamente aqui.  Pode ser removido se não for usar.

//        public ProdutoRepository(ApplicationDbContext context, ProdutoImagemRepository produtoImagemRepository)
//        {
//            _context = context;
//            _produtoImagemRepository = produtoImagemRepository; // Injetado, mas não usado aqui.
//        }

//        /// <summary>
//        /// Obtém um produto pelo seu ID.
//        /// </summary>
//        /// <param name="id">O ID do produto a ser obtido.</param>
//        /// <returns>O produto encontrado, ou nulo se não encontrado.</returns>
//        public async Task<Produto> GetById(int id)
//        {
//            // Busca o produto no contexto do banco de dados usando o ID fornecido.
//            // FirstOrDefaultAsync retorna o primeiro elemento que satisfaz a condição,
//            // ou o valor padrão (null para tipos de referência) se nenhum elemento for encontrado.
//            var produto = await _context.Produto
//                .FirstOrDefaultAsync(p => p.Produto_ID == id);

//            return produto;
//        }

//        /// <summary>
//        /// Obtém todos os produtos.
//        /// </summary>
//        /// <returns>Uma coleção de todos os produtos no banco de dados.</returns>
//        public async Task<IEnumerable<Produto>> GetAll()
//        {
//            // Retorna todos os produtos do contexto do banco de dados como uma lista assíncrona.
//            return await _context.Produto.ToListAsync();
//        }

//        /// <summary>
//        /// Obtém todos os produtos ativos.
//        /// </summary>
//        /// <returns>Uma coleção de produtos que estão habilitados e não excluídos.</returns>
//        public async Task<IEnumerable<Produto>> GetItens()
//        {
//            // Retorna uma lista de produtos que atendem aos critérios de serem habilitados e não excluídos.
//            return await _context.Produto
//                .Where(u => u.Habilitado == true && u.Excluido != true)
//                .ToListAsync();
//        }

//        /// <summary>
//        /// Salva ou atualiza um produto.
//        /// </summary>
//        /// <param name="model">O produto a ser salvo ou atualizado.</param>
//        /// <remarks>
//        /// Se o ID do produto for maior que 0, o produto é atualizado.
//        /// Caso contrário, o produto é adicionado como um novo registro.
//        /// </remarks>
//        public async Task SaveOrUpdate(Produto model)
//        {
//            if (model.Produto_ID > 0)
//            {
//                // Atualiza o produto existente no contexto do banco de dados.
//                _context.Produto.Update(model);
//            }
//            else
//            {
//                // Adiciona um novo produto ao contexto do banco de dados.
//                _context.Produto.Add(model);
//            }
//            // Salva as mudanças feitas no contexto do banco de dados de forma assíncrona.
//            await _context.SaveChangesAsync();
//        }

//        /// <summary>
//        /// Obtém todos os produtos de um usuário específico.
//        /// </summary>
//        /// <param name="id">O ID do usuário.</param>
//        /// <returns>Uma coleção de produtos pertencentes ao usuário especificado.</returns>
//        public async Task<IEnumerable<Produto>> GetByUsuarioId(int id)
//        {
//            // Retorna os produtos filtrados pelo ID do usuário.
//            var produtos = _context.Produto.Where(p => p.Usuario_ID == id)
//                .AsQueryable(); // Usa AsQueryable() para melhor performance em alguns casos

//            return await produtos.ToListAsync();
//        }

//        /// <summary>
//        /// Exclui logicamente um produto pelo seu ID.
//        /// </summary>
//        /// <remarks>
//        /// A exclusão é lógica: o produto é marcado como não habilitado e excluído,
//        /// e a data de atualização é definida.
//        /// </remarks>
//        public async Task Delete(Produto produto)
//        {
//            _context.Produto.Update(produto);

//            // Salva as mudanças no banco de dados de forma assíncrona.
//            await _context.SaveChangesAsync();
//        }
//    }
//}
