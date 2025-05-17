    using AjudeiMais.API.Repositories;
    using AjudeiMais.Data.Models.UsuarioModel;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Logging;

    namespace AjudeiMais.API.Services
    {
        public class UsuarioService
        {
            private readonly UsuarioRepository _usuarioRepository;
            private readonly ILogger<UsuarioService> _logger;
            private readonly PasswordHasher<Usuario> _passwordHasher;

            public UsuarioService(UsuarioRepository usuarioRepository, ILogger<UsuarioService> logger)
            {
                _usuarioRepository = usuarioRepository;
                _logger = logger;
                _passwordHasher = new PasswordHasher<Usuario>();
            }

            public async Task<Usuario> GetById(int id)
            {
                try
                {
                    var usuario = await _usuarioRepository.GetById(id);

                    return usuario;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao buscar usuários por ID");
                    throw new Exception("Erro ao buscar usuários.");
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
                    _logger.LogError(ex, "Erro ao buscar todos os usuários");
                    throw new Exception("Erro ao buscar usuários.");
                }
            }

            public async Task SaveOrUpdate(Usuario model)
            {
                try
                {
                    model.DataUpdate = DateTime.Now;

                    // Criptografando senha 
                    //Se for novo ou se a senha foi alterada
                    if (model.Usuario_ID == 0 || !string.IsNullOrWhiteSpace(model.Senha))
                    {
                        model.Senha = _passwordHasher.HashPassword(model, model.Senha);
                    }

                await _usuarioRepository.SaveOrUpdate(model);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao salvar ou atualizar o usuário");
                    throw new Exception("Erro ao salvar ou atualizar o usuário.");
                }
            }

            public async Task Delete(int id)
            {
                try
                {
                    await _usuarioRepository.Delete(id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao excluir o usuário com ID {UsuarioId}", id);
                    throw new Exception("Erro ao excluir o usuário.");
                }
            }

            public async Task<Usuario> Login(string email, string senha)
            {
                try 
                {
                    var usuario = await _usuarioRepository.GetByEmail(email);
                    if (usuario == null)
                    {
                        return null;
                    }

                    var resultado = _passwordHasher.VerifyHashedPassword(usuario, usuario.Senha, senha);
                    
                    if(resultado == PasswordVerificationResult.Success)
                    {
                        return usuario;
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao fazer login do usuario", ex.Message);
                    throw new Exception("Erro ao fazer login do usuario");
            }   
            }

        }
    }
