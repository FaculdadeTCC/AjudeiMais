using AjudeiMais.API.DTO;
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
        public async Task<InstituicaoGetDTO> GetByGUID(string GUID)
        {
            try
            {
                var instituicao = await _instituicaoRepository.GetByGUID(GUID);

                var instituicaoDTO = new InstituicaoGetDTO
                {
                    GUID = instituicao.GUID,
                    Nome = instituicao.Nome,
                    Descricao = instituicao.Descricao,
                    Telefone = instituicao.Telefone,
                    Email = instituicao.Email,
                    FotoPerfil = instituicao.FotoPerfil,
                    Documento = instituicao.Documento,
                    Role = instituicao.Role,                    
                    Enderecos = instituicao.Enderecos.Select(e => new EnderecoDTO
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

                    FotosUrl = instituicao.instituicaoImagems.Select(img => new InstituicaoImagemDTO
                    {
                        InsituicaoImagem_ID = img.InsituicaoImagem_ID,
                        CaminhoImagem = img.CaminhoImagem
                    }).ToList()
                };

                return instituicaoDTO;

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

        public async Task<ApiResponse<bool>> SaveOrUpdate(InstituicaoPostDTO model)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var existente = await _instituicaoRepository.GetByEmail(model.Email);
                if (existente != null && existente.Instituicao_ID != model.Instituicao_ID)
                {
                    response.Success = false;
                    response.Type = "Conflict";
                    response.Message = "Este e-mail já está sendo utilizado por outra instituição.";
                    return response;
                }

                model.GUID ??= Guid.NewGuid().ToString();
                var pasta = new[] { "images", "profile", "instituicao", model.GUID };
                string pathPerfil = await Helpers.SalvarImagemComoWebpAsync(model.FotoPerfil, pasta);

                var listaImagens = new List<InstituicaoImagem>();
                if (model.Fotos != null)
                {
                    foreach (var foto in model.Fotos)
                    {
                        if (foto.Length > 0 && foto.Length <= 5 * 1024 * 1024)
                        {
                            var path = await Helpers.SalvarImagemComoWebpAsync(foto, pasta, 75);
                            if (path != null)
                                listaImagens.Add(new InstituicaoImagem { CaminhoImagem = path });
                        }
                    }
                }

                var endereco = model.Enderecos?.FirstOrDefault();
                if (endereco == null)
                {
                    response.Success = false;
                    response.Type = "Validation";
                    response.Message = "Endereço obrigatório.";
                    return response;
                }

                var instituicao = new Instituicao
                {
                    Instituicao_ID = model.Instituicao_ID,
                    Nome = model.Nome,
                    Documento = model.Documento,
                    FotoPerfil = pathPerfil,
                    Descricao = model.Descricao,
                    Email = model.Email,
                    Senha = model.Senha,
                    Telefone = model.Telefone,
                    GUID = model.GUID,
                    Role = model.Role,
                    DataCriacao = model.Instituicao_ID == 0 ? DateTime.Now : existente?.DataCriacao ?? DateTime.Now,
                    DataUpdate = DateTime.Now,
                    Enderecos = new List<Endereco> { new Endereco
                    {
                        CEP = endereco.CEP,
                        Rua = endereco.Rua,
                        Numero = endereco.Numero,
                        Complemento = endereco.Complemento,
                        Bairro = endereco.Bairro,
                        Cidade = endereco.Cidade,
                        Estado = endereco.Estado
                    }},
                    instituicaoImagems = listaImagens
                };

                if (string.IsNullOrWhiteSpace(existente?.Senha) || existente.Senha != model.Senha)
                    instituicao.Senha = _passwordHasher.HashPassword(instituicao, model.Senha);

                await _instituicaoRepository.SaveOrUpdate(instituicao);

                response.Success = true;
                response.Type = "Success";
                response.Message = "Instituição salva/atualizada com sucesso.";
                response.Data = true;
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar ou atualizar a instituição");
                response.Success = false;
                response.Type = "Exception";
                response.Message = "Erro ao salvar ou atualizar a instituição.";
                response.Errors.Add(ex.Message);
                return response;
            }
        }

        public async Task<ApiResponse<bool>> AtualizarDadosInstituicaoAsync(AtualizarDadosDTO model)
        {
            var response = new ApiResponse<bool>();
            try
            {
                var existente = await _instituicaoRepository.GetByGUID(model.GUID);
                if (existente == null)
                {
                    response.Success = false;
                    response.Type = "NotFound";
                    response.Message = "Instituição não encontrada.";
                    return response;
                }

                var emailRepetido = await _instituicaoRepository.GetByEmail(model.Email);
                if (emailRepetido != null && emailRepetido.GUID != model.GUID)
                {
                    response.Success = false;
                    response.Type = "Conflict";
                    response.Message = "Este e-mail já está sendo utilizado por outra instituição.";
                    return response;
                }

                var pasta = new[] { "images", "profile", "instituicao", existente.GUID };
                if (model.FotoPerfil != null)
                    existente.FotoPerfil = await Helpers.SalvarImagemComoWebpAsync(model.FotoPerfil, pasta);

                existente.Nome = model.Nome;
                existente.Descricao = model.Descricao;
                existente.Email = model.Email;
                existente.Telefone = model.Telefone;
                existente.Documento = model.Documento;
                existente.Role = model.Role;

                await _instituicaoRepository.SaveOrUpdate(existente);

                response.Success = true;
                response.Type = "Success";
                response.Message = "Dados atualizados com sucesso.";
                response.Data = true;
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar dados da instituição");
                response.Success = false;
                response.Type = "Exception";
                response.Message = "Erro ao atualizar dados da instituição.";
                response.Errors.Add(ex.Message);
                return response;
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

        public async Task<ApiResponse<Instituicao>> Delete(string guid)
        {
            try
            {
                var instituicao = await _instituicaoRepository.GetByGUID(guid);

                if (instituicao == null)
                {
                    return new ApiResponse<Instituicao>
                    {
                        Success = false,
                        Type = "error",
                        Message = "Usuário não encontrado."
                    };
                }

                // Desabilita e exclui instituição
                instituicao.Habilitado = false;
                instituicao.Excluido = true;
                instituicao.DataUpdate = DateTime.Now;

                // Desabilita e exclui endereços dessa instituição

                foreach( var endereco in instituicao.Enderecos)
                {
                    endereco.Habilitado = false;
                    endereco.Excluido = true;
                }

                // Desabilita e exclui Fotos dessa instituição

                foreach (var foto in instituicao.instituicaoImagems )
                {
                    foto.Habilitado=false;
                    foto.Excluido = true;
                }

                await _instituicaoRepository.Delete(instituicao);

                return new ApiResponse<Instituicao>
                {
                    Success = true,
                    Type = "success",
                    Message = "Sua conta foi excluída com sucesso."
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<Instituicao>
                {
                    Success = false,
                    Type = "error",
                    Message = "Não foi possível excluir o usuário. Por favor, tente novamente. Se o problema persistir, entre em contato com nosso suporte."
                };
            }
        }

        public async Task<ApiResponse<Instituicao>> AtualizarSenha(InstituicaoSenhaDTO instituicaoSenha)
        {
            try
            {
                var instituicao = await _instituicaoRepository.GetByGUID(instituicaoSenha.GUID);

                if (instituicao == null)
                {
                    return new ApiResponse<Instituicao>
                    {
                        Success = false,
                        Type = "error",
                        Message = "Usuário não encontrado."
                    };
                }

                var result = _passwordHasher.VerifyHashedPassword(instituicao, instituicao.Senha, instituicaoSenha.SenhaAtual);

                if (result != PasswordVerificationResult.Success)
                {
                    return new ApiResponse<Instituicao>
                    {
                        Success = false,
                        Type = "error",
                        Message = "Senha atual inválida."
                    };
                }

                instituicao.Senha = _passwordHasher.HashPassword(instituicao, instituicaoSenha.NovaSenha);

                await _instituicaoRepository.SaveOrUpdate(instituicao);

                return new ApiResponse<Instituicao>
                {
                    Success = true,
                    Type = "success",
                    Message = "Senha alterada com sucesso.",
                    Data = instituicao
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar senha do usuário");
                return new ApiResponse<Instituicao>
                {
                    Success = false,
                    Type = "error",
                    Message = "Ocorreu um erro ao atualizar sua senha. Tente novamente."
                };
            }
        }

        public async Task<ApiResponse<Instituicao>> ValidarSenha(InstituicaoValidarSenhaDTO instituicaoSenha)
        {
            try
            {
                var instituicao = await _instituicaoRepository.GetByGUID(instituicaoSenha.GUID);

                if (instituicao == null)
                {
                    return new ApiResponse<Instituicao>
                    {
                        Success = false,
                        Type = "error",
                        Message = "Usuário não encontrado."
                    };
                }

                var result = _passwordHasher.VerifyHashedPassword(instituicao, instituicao.Senha, instituicaoSenha.Senha);

                if (result != PasswordVerificationResult.Success)
                {
                    return new ApiResponse<Instituicao>
                    {
                        Success = false,
                        Type = "error",
                        Message = "Senha atual inválida."
                    };
                }

                return new ApiResponse<Instituicao>
                {
                    Success = true,
                    Type = "success",
                    Message = "Senha valida com sucesso.",
                    Data = instituicao
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar senha do usuário");
                return new ApiResponse<Instituicao>
                {
                    Success = false,
                    Type = "error",
                    Message = "Ocorreu um erro ao atualizar sua senha. Tente novamente."
                };
            }
        }
    }
}
