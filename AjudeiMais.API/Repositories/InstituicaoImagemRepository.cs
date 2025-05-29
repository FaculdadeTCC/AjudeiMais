using AjudeiMais.API.Interfaces;
using AjudeiMais.Data.Context;
using AjudeiMais.Data.Models.InstituicaoModel;
using Microsoft.EntityFrameworkCore;

namespace AjudeiMais.API.Repositories
{
    public class InstituicaoImagemRepository : IGenericRepository<InstituicaoImagem>
    {
        private readonly ApplicationDbContext _context;

        public InstituicaoImagemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Delete(InstituicaoImagem model)
        {
            try
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao excluir a imagem da instituição: " + ex.Message);
            }
        }

        public async Task<IEnumerable<InstituicaoImagem>> GetImagensPorInstituicao(int instituicaoId)
        {
            try
            {
                return await _context.InstituicaoImagem
                    .Where(p => p.Instituicao_ID == instituicaoId && p.Habilitado && !p.Excluido)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar imagens da instituição: " + ex.Message);
            }
        }

        public async Task<InstituicaoImagem> GetById(int id)
        {
            try
            {
                return await _context.InstituicaoImagem.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar imagem por ID: " + ex.Message);
            }
        }

        public async Task<IEnumerable<InstituicaoImagem>> GetAll()
        {
            try
            {
                return await _context.InstituicaoImagem
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar todas as imagens: " + ex.Message);
            }
        }

        public async Task<IEnumerable<InstituicaoImagem>> GetItens()
        {
            try
            {
                return await _context.InstituicaoImagem
                    .Where(u => u.Habilitado == true && u.Excluido != true)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar imagens ativas: " + ex.Message);
            }
        }

        public async Task SaveOrUpdate(InstituicaoImagem model)
        {
            try
            {
                if (model.InsituicaoImagem_ID > 0)
                {
                    _context.InstituicaoImagem.Update(model);

                    await _context.SaveChangesAsync();
                }
                else
                {
                    model.Habilitado = true;
                    model.Excluido = false;

                    await _context.InstituicaoImagem.AddAsync(model);

                    await _context.SaveChangesAsync();
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao salvar ou atualizar imagem da instituição: " + ex.Message);
            }
        }
    }
}
