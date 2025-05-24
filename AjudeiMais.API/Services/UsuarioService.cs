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

        public async Task SaveOrUpdate(UsuarioDTO model)
        {
            try
            {
                var currentUsuario = await _usuarioRepository.GetByEmail(model.Email);

                if (currentUsuario != null)
                {
                    throw new Exception("Este e-mail já está sendo utilizado por outro usuário.");
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
                _logger.LogError(ex, "Erro ao fazer login do usuário");
                throw new Exception("Erro ao fazer login do usuário.");
            }
        }
    }
}
