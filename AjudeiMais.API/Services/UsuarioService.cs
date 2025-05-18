using AjudeiMais.API.Repositories;
using AjudeiMais.Data.Models.UsuarioModel;
using Microsoft.Extensions.Logging;

namespace AjudeiMais.API.Services
{
    public class UsuarioService
    {
        private readonly UsuarioRepository _usuarioRepository;
        private readonly NominatimService _nominatimService;
        private readonly ILogger<UsuarioService> _logger;

        public UsuarioService(UsuarioRepository usuarioRepository,
            NominatimService nominatimService,
            ILogger<UsuarioService> logger)
        {
            _usuarioRepository = usuarioRepository;
            _nominatimService = nominatimService;
            _logger = logger;
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
                var coordenadas = await _nominatimService.ObterCoordenadasPorCepAsync(model.CEP, model.Cidade);

                model.Latitude = coordenadas.Latitude;
                model.Longitude = coordenadas.Longitude;
                model.DataUpdate = DateTime.Now;

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
    }
}
