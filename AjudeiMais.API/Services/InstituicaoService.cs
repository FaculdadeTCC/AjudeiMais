using AjudeiMais.API.Repositories;
using AjudeiMais.API.Tools;
using AjudeiMais.Data.Models.InstituicaoModel;
using AjudeiMais.Data.Models.UsuarioModel;
using Microsoft.AspNetCore.Identity;

namespace AjudeiMais.API.Services
{
    public class InstituicaoService
    {
        private readonly InstituicaoRepository _instituicaoRepository;
        private readonly PasswordHasher<Instituicao> _passwordHasher;
        private readonly ILogger<InstituicaoService> _logger;

        public InstituicaoService(InstituicaoRepository instituicaoRepository, ILogger<InstituicaoService> logger)
        {
            _instituicaoRepository = instituicaoRepository;
            _passwordHasher = new PasswordHasher<Instituicao>();
            _logger = logger;
        }

        public async Task<Instituicao> GetById(int id)
        {
            try
            {
                return await _instituicaoRepository.GetById(id);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar instituição");
                throw new Exception("Erro ao buscar instituição" + ex.Message);
            }
        }

        public async Task<IEnumerable<Instituicao>> GetAll()
        {
            try
            {
                return await _instituicaoRepository.GetAll();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todas as instituições");
                throw new Exception("Erro ao buscar todas as instituições" + ex.Message);
            }
        }

        public async Task<IEnumerable<Instituicao>> GetItens()
        {
            try
            {
                return await _instituicaoRepository.GetItens();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todas as instituições ativas");
                throw new Exception("Erro ao buscar todas as instituições ativas" + ex.Message);
            }
        }

        public async Task Delete(int Id)
        {
            try
            {
                await _instituicaoRepository.Delete(Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar institiuição");
                throw new Exception("Erro deletar institiuição" + ex.Message);
            }
        }

        public async Task<Instituicao?> Login(string email, string senha)
        {
            try
            {
                var usuario = await _instituicaoRepository.GetByEmail(email);
                if (usuario == null)
                    return null;

                var result = _passwordHasher.VerifyHashedPassword(usuario, usuario.Senha, senha);
                return result == PasswordVerificationResult.Success ? usuario : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao fazer login da instituição");
                throw new Exception("Erro ao fazer login da instituição");
            }
        }
        public async Task SaveOrUpdate(InstituicaoDTO model)
        {
            try
            {
                var existente = await _instituicaoRepository.GetByEmail(model.Email);

                if (existente != null && existente.Instituicao_ID != model.Instituicao_ID)
                    throw new Exception("Este e-mail já está sendo utilizado por outra instituição.");

                model.GUID ??= Guid.NewGuid().ToString();

                var pasta = new[] { "images", "profile", "instituicao", model.GUID };

                var listaImagens = new List<InstituicaoImagem>();

                // Salva cada imagem
                if (model.Imagens != null && model.Imagens.Length > 0)
                {
                    foreach (var imagem in model.Imagens)
                    {
                        if (imagem.Length > 0 && imagem.Length <= 5 * 1024 * 1024)
                        {
                            var path = await Helpers.SalvarImagemComoWebpAsync(imagem, pasta, qualidade: 75);

                            if (path != null)
                            {
                                listaImagens.Add(new InstituicaoImagem
                                {
                                    CaminhoImagem = path
                                });
                            }
                        }
                    }
                }

                var instituicao = new Instituicao
                {
                    Instituicao_ID = model.Instituicao_ID,
                    Nome = model.Nome,
                    Documento = model.CNPJ,
                    Email = model.Email,
                    Senha = model.Senha,
                    Telefone = model.Telefone,
                    GUID = model.GUID,
                    DataCriacao = model.Instituicao_ID == 0 ? DateTime.Now : existente.DataCriacao,
                    DataUpdate = DateTime.Now,
                    Enderecos = new List<Endereco>
                    {
                        new Endereco
                        {
                            CEP = model.Enderecos[0].CEP,
                            Rua = model.Enderecos[0].Rua,
                            Numero = model.Enderecos[0].Numero,
                            Complemento = model.Enderecos[0].Complemento,
                            Bairro = model.Enderecos[0].Bairro,
                            Cidade = model.Enderecos[0].Cidade,
                            Estado = model.Enderecos[0].Estado
                        }
                    },
                   instituicaoImagems = listaImagens
                };

                // Hash da senha
                if (string.IsNullOrWhiteSpace(existente?.Senha) || existente.Senha != model.Senha)
                    instituicao.Senha = _passwordHasher.HashPassword(instituicao, model.Senha);

                await _instituicaoRepository.SaveOrUpdate(instituicao);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar ou atualizar a instituição");
                throw new Exception("Erro ao salvar ou atualizar a instituição.");
            }
        }




    }
}
