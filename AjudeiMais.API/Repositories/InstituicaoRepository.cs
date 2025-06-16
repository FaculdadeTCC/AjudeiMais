using AjudeiMais.API.DTO;
using AjudeiMais.API.Interfaces;
using AjudeiMais.Data.Context;
using AjudeiMais.Data.Models.InstituicaoModel;
using AjudeiMais.Data.Models.ProdutoModel;
using Microsoft.EntityFrameworkCore;

namespace AjudeiMais.API.Repositories
{
    public class InstituicaoRepository : IGenericRepository<Instituicao>
    {
        private readonly ApplicationDbContext _context;

        public InstituicaoRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task Delete(Instituicao model)
        {
            try
            {
                _context.Update(model);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<Instituicao>> GetAll()
        {
            try
            {
                var instituicoes = await _context.Instituicao.ToListAsync();
                return instituicoes;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Instituicao> GetById(int id)
        {
            try
            {
                var instituicao = await _context.Instituicao.FirstOrDefaultAsync(x => x.Instituicao_ID == id);

                return instituicao;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Instituicao> GetByGUID(string Guid)
        {
            try
            {
				var instituicao = await _context.Instituicao
					.Where(x => x.GUID == Guid)
					.Select(x => new Instituicao
					{
						GUID = x.GUID,
						Nome = x.Nome,
						Descricao = x.Descricao,
						Telefone = x.Telefone,
						Email = x.Email,
						FotoPerfil = x.FotoPerfil,
						Documento = x.Documento,
						Role = x.Role,
						Latitude = x.Latitude,
						Longitude = x.Longitude,

						Enderecos = x.Enderecos.Select(e => new Endereco
						{
							Endereco_ID = e.Endereco_ID,
							CEP = e.CEP,
							Rua = e.Rua,
							Numero = e.Numero,
							Complemento = e.Complemento,
							Bairro = e.Bairro,
							Cidade = e.Cidade,
							Estado = e.Estado
						}).ToList(),

						instituicaoImagems = x.instituicaoImagems
							.Where(img => img.Habilitado && !img.Excluido)
							.Select(img => new InstituicaoImagem
							{
								InsituicaoImagem_ID = img.InsituicaoImagem_ID,
								CaminhoImagem = img.CaminhoImagem
							}).ToList()
					})
					.FirstOrDefaultAsync();


				return instituicao;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Instituicao> GetByEmail(string email)
        {
            try
            {
                var instituicao = await _context.Instituicao.FirstOrDefaultAsync(x => x.Email == email);

                return instituicao;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<Instituicao>> GetItens()
        {
            try
            {
                var instituicoes = await _context.Instituicao.Where(x => x.Habilitado == true && x.Excluido != true).ToListAsync();
                return instituicoes;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task SaveOrUpdate(Instituicao model)
        {
            if (model.Instituicao_ID > 0)
            {
                model.DataUpdate = DateTime.Now;

                _context.Instituicao.Update(model);
                await _context.SaveChangesAsync();
            }
            else
            {

                model.DataCriacao = DateTime.Now;
                model.Habilitado = true;
                model.Excluido = false;

                foreach (var item in model.instituicaoImagems)
                {
                    item.Habilitado = true;
                    item.Excluido = false;
                }

                _context.Instituicao.Add(model);

                await _context.SaveChangesAsync();
            }
        }
    }
}
