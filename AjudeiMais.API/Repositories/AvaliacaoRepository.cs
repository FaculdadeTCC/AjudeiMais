using AjudeiMais.Data.Context;
using AjudeiMais.Data.Models.AvaliacaoModel;

namespace AjudeiMais.API.Repositories
{
    public class AvaliacaoRepository
    {
        private readonly ApplicationDbContext _context;

        public AvaliacaoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SaveOrUpdate(Avaliacao entity)
        {
            if (entity.Avaliacao_ID > 0)
            {
                _context.Avaliacao.Update(entity);
            }
            else
            {
                _context.Avaliacao.AddAsync(entity);
            }

            await _context.SaveChangesAsync();
        }
    }
}
