using AjudeiMais.API.Repositories;
using AjudeiMais.Data.Models.UsuarioModel;
using Microsoft.AspNetCore.Identity;
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

        public async Task SaveOrUpdate(Usuario model)
        {
            try
            {
                model.DataUpdate = DateTime.Now;

                // Geolocalização via Nominatim
                var coordenadas = await _nominatimService.ObterCoordenadasPorCepAsync(model.CEP, model.Cidade);
                model.Latitude = coordenadas.Latitude;
                model.Longitude = coordenadas.Longitude;

                // Hash da senha (caso seja novo ou senha alterada)
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
