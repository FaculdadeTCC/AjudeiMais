using AjudeiMais.API.DTO;
using AjudeiMais.API.Repositories;
using AjudeiMais.API.Tools;
using AjudeiMais.Data.Models.UsuarioModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AjudeiMais.API.Services
{
    public class UsuarioService
    {
        private readonly UsuarioRepository _usuarioRepository;
        private readonly NominatimService _nominatimService;
        private readonly ILogger<UsuarioService> _logger;
        private readonly PasswordHasher<Usuario> _passwordHasher;

        public UsuarioService(
            UsuarioRepository usuarioRepository,
            NominatimService nominatimService,
            ILogger<UsuarioService> logger)
        {
            _usuarioRepository = usuarioRepository;
            _nominatimService = nominatimService;
            _logger = logger;
            _passwordHasher = new PasswordHasher<Usuario>();
        }

        public async Task<Usuario> GetById(int id)
        {
            try
            {
                return await _usuarioRepository.GetById(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuário por ID");
                throw new Exception("Erro ao buscar usuário.");
            }
        }

        public async Task<Usuario> GetByEmail(string email)
        {
            try
            {
                return await _usuarioRepository.GetByEmail(email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuário por e-mail");
                throw new Exception("Erro ao buscar usuário por e-mail.");
            }
        }

        public async Task<Usuario> GetByGUID(string GUID)
        {
            try
            {
                return await _usuarioRepository.GetByGUID(GUID);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuário por GUID");
                throw new Exception("Erro ao buscar usuário.");
            }
        }

        public async Task<IEnumerable<Usuario>> GetAll()
        {
            try
            {
                return await _usuarioRepository.GetAll();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos os usuários");
                throw new Exception("Erro ao buscar usuários.");
            }
        }

        public async Task<IEnumerable<Usuario>> GetItens()
        {
            try
            {
                return await _usuarioRepository.GetItens();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar itens dos usuários");
                throw new Exception("Erro ao buscar itens.");
            }
        }

        public async Task<ApiResponse<Usuario>> SaveOrUpdate(UsuarioDTO model)
        {
            try
            {
                var currentUsuario = await _usuarioRepository.GetByEmail(model.Email);

                if (currentUsuario != null)
                {
                    return new ApiResponse<Usuario>
                    {
                        Success = false,
                        Type = "error",
                        Message = "Este e-mail já está sendo utilizado por outro usuário.",
                    };
                }

                model.GUID = Guid.NewGuid().ToString();

                var pasta = new[] { "images", "profile", "user", model.GUID.ToString() };

                string path = await Helpers.SalvarImagemComoWebpAsync(model.FotoDePerfil, pasta);

                Usuario Usuario = new Usuario
                {
                    Usuario_ID = model.Usuario_ID,
                    NomeCompleto = model.NomeCompleto,
                    Documento = model.Documento,
                    Email = model.Email,
                    Senha = model.Senha,
                    GUID = model.GUID,
                    Role = model.Role,
                    CEP = model.CEP,
                    Rua = model.Rua,
                    Numero = model.Numero,
                    Complemento = model.Complemento ?? "",
                    Bairro = model.Bairro,
                    Cidade = model.Cidade,
                    Estado = model.Estado,
                    FotoDePerfil = path,
                    Telefone = model.Telefone ?? "",
                    TelefoneFixo = model.TelefoneFixo ?? "",
                    Latitude = model.Latitude ?? "",
                    Longitude = model.Longitude ?? "",
                    Habilitado = model.Habilitado,
                    Excluido = model.Excluido,
                    DataCriacao = model.DataCriacao,
                    DataUpdate = model.DataUpdate,
                    Produtos = model.Produtos,
                };

                if (model.Usuario_ID > 0)
                {
                    var coordenadas = await _nominatimService.ObterCoordenadasPorCepAsync(Usuario.CEP, Usuario.Cidade);
                    Usuario.Latitude = coordenadas.Latitude;
                    Usuario.Longitude = coordenadas.Longitude;

                    Usuario.Senha = _passwordHasher.HashPassword(Usuario, Usuario.Senha);
                    Usuario.DataUpdate = DateTime.Now;

                    await _usuarioRepository.SaveOrUpdate(Usuario);

                    return new ApiResponse<Usuario>
                    {
                        Success = true,
                        Type = "success",
                        Message = "Dados atualizados com sucesso.",
                        Data = Usuario
                    };
                }
                else
                {
                    Usuario.Role = "usuario";
                    Usuario.DataCriacao = DateTime.Now;
                    Usuario.Habilitado = true;

                    // Geolocalização via Nominatim
                    var coordenadas = await _nominatimService.ObterCoordenadasPorCepAsync(Usuario.CEP, Usuario.Cidade);
                    Usuario.Latitude = coordenadas.Latitude;
                    Usuario.Longitude = coordenadas.Longitude;

                    // Hash da senha (caso seja novo ou senha alterada)
                    Usuario.Senha = _passwordHasher.HashPassword(Usuario, Usuario.Senha);

                    await _usuarioRepository.SaveOrUpdate(Usuario);

                }

                return new ApiResponse<Usuario>
                {
                    Success = true,
                    Type = "success",
                    Message = "Boas-vindas! Seu cadastro foi concluído. Acesse sua conta agora mesmo.",
                    Data = Usuario
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar ou atualizar o usuário");
                return new ApiResponse<Usuario>
                {
                    Success = false,
                    Type = "error",
                    Message = "Ocorreu um erro inesperado ao salvar ou atualizar o usuário. Por favor, tente novamente. Se o problema persistir, entre em contato com nosso suporte."
                };
            }
        }

        public async Task<ApiResponse<Usuario>> Delete(string guid)
        {
            try
            {
                var usuario = await GetByGUID(guid);

                if (usuario == null)
                {
                    return new ApiResponse<Usuario>
                    {
                        Success = false,
                        Type = "error",
                        Message = "Usuário não encontrado."
                    };
                }

                usuario.Habilitado = false;
                usuario.Excluido = true;
                usuario.DataUpdate = DateTime.Now;

                await _usuarioRepository.Delete(usuario);

                return new ApiResponse<Usuario>
                {
                    Success = true,
                    Type = "success",
                    Message = "Sua conta foi excluída com sucesso."
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<Usuario>
                {
                    Success = false,
                    Type = "error",
                    Message = "Não foi possível excluir o usuário. Por favor, tente novamente. Se o problema persistir, entre em contato com nosso suporte."
                };
            }
        }

        public async Task<Usuario?> Login(string email, string senha)
        {
            try
            {
                var usuario = await _usuarioRepository.GetByEmail(email);
                if (usuario == null)
                    return null;

                var result = _passwordHasher.VerifyHashedPassword(usuario, usuario.Senha, senha);
                return result == PasswordVerificationResult.Success ? usuario : null;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao fazer login do usuário.");
            }
        }

        public async Task<ApiResponse<Usuario>> AtualizarDadosPessoais(UsuarioDadosPessoaisDTO dadosAtualizados)
        {
            try
            {
                var usuarioExistente = await _usuarioRepository.GetById(dadosAtualizados.Usuario_ID);

                if (usuarioExistente == null)
                {
                    return new ApiResponse<Usuario>
                    {
                        Success = false,
                        Type = "error",
                        Message = "Usuário não encontrado."
                    };
                }

                var emailCadastrado = await _usuarioRepository.GetByEmail(dadosAtualizados.Email);

                if (emailCadastrado != null && dadosAtualizados.Email != usuarioExistente.Email)
                {
                    return new ApiResponse<Usuario>
                    {
                        Success = false,
                        Type = "error",
                        Message = "Este e-mail já está sendo utilizado por outro usuário.",
                    };
                }

                var pasta = new[] { "images", "profile", "user", dadosAtualizados.GUID.ToString() };

                if (dadosAtualizados.FotoDePerfil != null)
                {
                    string path = await Helpers.SalvarImagemComoWebpAsync(dadosAtualizados.FotoDePerfil, pasta);

                    usuarioExistente.FotoDePerfil = path;
                }

                // Atualiza somente os campos pessoais que são permitidos mudar
                usuarioExistente.NomeCompleto = dadosAtualizados.NomeCompleto ?? usuarioExistente.NomeCompleto;
                usuarioExistente.Telefone = dadosAtualizados.Telefone ?? usuarioExistente.Telefone;
                usuarioExistente.TelefoneFixo = dadosAtualizados.TelefoneFixo ?? usuarioExistente.TelefoneFixo;
                usuarioExistente.Email = dadosAtualizados.Email ?? usuarioExistente.Email;
                usuarioExistente.DataUpdate = DateTime.Now;

                await _usuarioRepository.SaveOrUpdate(usuarioExistente);

                return new ApiResponse<Usuario>
                {
                    Success = true,
                    Type = "success",
                    Message = "Dados pessoais atualizados com sucesso.",
                    Data = usuarioExistente
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar dados pessoais do usuário");
                return new ApiResponse<Usuario>
                {
                    Success = false,
                    Type = "error",
                    Message = "Ocorreu um erro ao atualizar seus dados pessoais. Tente novamente."
                };
            }
        }

        public async Task<ApiResponse<Usuario>> AtualizarEndereco(UsuarioEnderecoDTO dadosAtualizados)
        {
            try
            {
                var usuarioExistente = await _usuarioRepository.GetById(dadosAtualizados.Usuario_ID);

                if (usuarioExistente == null)
                {
                    return new ApiResponse<Usuario>
                    {
                        Success = false,
                        Type = "error",
                        Message = "Usuário não encontrado."
                    };
                }

                var coordenadas = await _nominatimService.ObterCoordenadasPorCepAsync(dadosAtualizados.CEP, dadosAtualizados.Cidade);

                dadosAtualizados.Latitude = coordenadas.Latitude;
                dadosAtualizados.Longitude = coordenadas.Longitude;

                // Atualiza somente os campos pessoais que são permitidos mudar
                usuarioExistente.CEP = dadosAtualizados.CEP ?? usuarioExistente.CEP;
                usuarioExistente.Rua = dadosAtualizados.Rua ?? usuarioExistente.Rua;
                usuarioExistente.Numero = dadosAtualizados.Numero > 0 ? dadosAtualizados.Numero : usuarioExistente.Numero;
                usuarioExistente.Complemento = dadosAtualizados.Complemento ?? usuarioExistente.Complemento;
                usuarioExistente.Bairro = dadosAtualizados.Bairro ?? usuarioExistente.Bairro;
                usuarioExistente.Cidade = dadosAtualizados.Cidade ?? usuarioExistente.Cidade;
                usuarioExistente.Estado = dadosAtualizados.Estado ?? usuarioExistente.Estado;
                usuarioExistente.Latitude = dadosAtualizados.Latitude ?? usuarioExistente.Latitude;
                usuarioExistente.Longitude = dadosAtualizados.Longitude ?? usuarioExistente.Longitude;
                usuarioExistente.DataUpdate = DateTime.Now;

                await _usuarioRepository.SaveOrUpdate(usuarioExistente);

                return new ApiResponse<Usuario>
                {
                    Success = true,
                    Type = "success",
                    Message = "Endereço atualizados com sucesso.",
                    Data = usuarioExistente
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar endereço do usuário");
                return new ApiResponse<Usuario>
                {
                    Success = false,
                    Type = "error",
                    Message = "Ocorreu um erro ao atualizar seu endereço. Tente novamente."
                };
            }
        }
        
        public async Task<ApiResponse<Usuario>> AtualizarSenha(UsuarioSenhaDTO dadosAtualizados)
        {
            try
            {
                var usuarioExistente = await _usuarioRepository.GetById(dadosAtualizados.Usuario_ID);

                if (usuarioExistente == null)
                {
                    return new ApiResponse<Usuario>
                    {
                        Success = false,
                        Type = "error",
                        Message = "Usuário não encontrado."
                    };
                }

                var result = _passwordHasher.VerifyHashedPassword(usuarioExistente, usuarioExistente.Senha, dadosAtualizados.SenhaAtual);

                if(result != PasswordVerificationResult.Success)
                {
                    return new ApiResponse<Usuario>
                    {
                        Success = false,
                        Type = "error",
                        Message = "Senha atual inválida."
                    };
                }

                usuarioExistente.Senha = _passwordHasher.HashPassword(usuarioExistente, dadosAtualizados.NovaSenha);

                await _usuarioRepository.SaveOrUpdate(usuarioExistente);

                return new ApiResponse<Usuario>
                {
                    Success = true,
                    Type = "success",
                    Message = "Senha alterada com sucesso.",
                    Data = usuarioExistente
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar senha do usuário");
                return new ApiResponse<Usuario>
                {
                    Success = false,
                    Type = "error",
                    Message = "Ocorreu um erro ao atualizar sua senha. Tente novamente."
                };
            }
        }

        public async Task<ApiResponse<Usuario>> VerificarSenha(UsuarioExcluirContaDTO usuario)
        {
            var usuarioExistente = await GetById(usuario.Usuario_ID);

            if (usuarioExistente == null)
            {
                return new ApiResponse<Usuario>
                {
                    Success = false,
                    Type = "error",
                    Message = "Usuário não encontrado."
                };
            }

            var result = _passwordHasher.VerifyHashedPassword(usuarioExistente, usuarioExistente.Senha, usuario.SenhaAtual);

            if (result != PasswordVerificationResult.Success)
            {
                return new ApiResponse<Usuario>
                {
                    Success = false,
                    Type = "error",
                    Message = "Senha atual inválida."
                };
            } else
            {
                return new ApiResponse<Usuario>
                {
                    Success = true,
                };
            }
        }

    }
}
