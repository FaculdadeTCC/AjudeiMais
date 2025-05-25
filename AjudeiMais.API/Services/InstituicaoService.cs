    //using AjudeiMais.API.Repositories;
    //using AjudeiMais.Data.Models.InstituicaoModel;
    //using AjudeiMais.Data.Models.UsuarioModel;
    //using Microsoft.AspNetCore.Identity;

    //namespace AjudeiMais.API.Services
    //{
    //    public class InstituicaoService
    //    {
    //        private readonly InstituicaoRepository _instituicaoRepository;
    //        private readonly PasswordHasher<Instituicao> _passwordHasher;
    //        private readonly ILogger<InstituicaoService> _logger;

    //        public InstituicaoService(InstituicaoRepository instituicaoRepository, ILogger<InstituicaoService> logger)
    //        {
    //            _instituicaoRepository = instituicaoRepository;
    //            _passwordHasher = new PasswordHasher<Instituicao>();
    //            _logger = logger;
    //        }

    //        public async Task<Instituicao> GetById(int id)
    //        {
    //            try
    //            {
    //                return await _instituicaoRepository.GetById(id);

    //            }
    //            catch (Exception ex)
    //            {
    //                _logger.LogError(ex, "Erro ao buscar instituição");
    //                throw new Exception("Erro ao buscar instituição" + ex.Message);
    //            }
    //        }

    //        public async Task<IEnumerable<Instituicao>> GetAll()
    //        {
    //            try
    //            {
    //                return await _instituicaoRepository.GetAll();

    //            }
    //            catch (Exception ex)
    //            {
    //                _logger.LogError(ex, "Erro ao buscar todas as instituições");
    //                throw new Exception("Erro ao buscar todas as instituições" + ex.Message);
    //            }
    //        }

    //        public async Task<IEnumerable<Instituicao>> GetItens()
    //        {
    //            try
    //            {
    //                return await _instituicaoRepository.GetItens();

    //            }
    //            catch (Exception ex)
    //            {
    //                _logger.LogError(ex, "Erro ao buscar todas as instituições ativas");
    //                throw new Exception("Erro ao buscar todas as instituições ativas" + ex.Message);
    //            }
    //        }

    //        public async Task SaveOrUpdate(Instituicao model)
    //        {
    //            try
    //            {
    //                model.DataUpdate = DateTime.Now;

    //                // Hash da senha (caso seja novo ou senha alterada)
    //                if (model.Instituicao_ID == 0 || !string.IsNullOrWhiteSpace(model.Senha))
    //                {
    //                    model.Senha = _passwordHasher.HashPassword(model, model.Senha);
    //                }

    //                await _instituicaoRepository.SaveOrUpdate(model);
    //            }
    //            catch (Exception ex)
    //            {
    //                _logger.LogError(ex, "Erro ao salvar ou atuzalizar institiuição");
    //                throw new Exception("Erro ao salvar ou atuzalizar institiuição" + ex.Message);
    //            }
    //        }

    //        public async Task Delete(int Id)
    //        {
    //            try
    //            {
    //                await _instituicaoRepository.Delete(Id);
    //            }
    //            catch (Exception ex)
    //            {
    //                _logger.LogError(ex, "Erro ao deletar institiuição");
    //                throw new Exception("Erro deletar institiuição" + ex.Message);
    //            }
    //        }

    //        public async Task<Instituicao?> Login(string email, string senha)
    //        {
    //            try
    //            {
    //                var usuario = await _instituicaoRepository.GetByEmail(email);
    //                if (usuario == null)
    //                    return null;

    //                var result = _passwordHasher.VerifyHashedPassword(usuario, usuario.Senha, senha);
    //                return result == PasswordVerificationResult.Success ? usuario : null;
    //            }
    //            catch (Exception ex)
    //            {
    //                _logger.LogError(ex, "Erro ao fazer login da instituição");
    //                throw new Exception("Erro ao fazer login da instituição");
    //            }
    //        }


    //    }
    //}
