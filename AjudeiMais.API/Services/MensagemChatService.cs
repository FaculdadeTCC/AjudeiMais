//using AjudeiMais.API.Repositories;
//using AjudeiMais.Data.Models.ChatModel;
//using Microsoft.Extensions.Logging;

//namespace AjudeiMais.API.Services
//{
//    public class MensagemChatService
//    {
//        private readonly MensagemChatRepository _mensagemChatRepository;
//        private readonly ILogger<MensagemChatService> _logger;

//        public MensagemChatService(MensagemChatRepository mensagemChatRepository, ILogger<MensagemChatService> logger)
//        {
//            _mensagemChatRepository = mensagemChatRepository;
//            _logger = logger;
//        }

//        public async Task<MensagemChat> GetById(int id)
//        {
//            try
//            {
//                return await _mensagemChatRepository.GetById(id);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Erro ao buscar mensagem por ID");
//                throw new Exception("Erro ao buscar mensagem por ID: " + ex.Message);
//            }
//        }

//        public async Task<IEnumerable<MensagemChat>> GetAll()
//        {
//            try
//            {
//                return await _mensagemChatRepository.GetAll();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Erro ao buscar todas as mensagens");
//                throw new Exception("Erro ao buscar todas as mensagens: " + ex.Message);
//            }
//        }

//        public async Task<IEnumerable<MensagemChat>> GetMensagensPorChatId(int chatId)
//        {
//            try
//            {
//                return await _mensagemChatRepository.GetMensagensPorChatId(chatId);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Erro ao buscar mensagens por Chat_ID");
//                throw new Exception("Erro ao buscar mensagens por Chat_ID: " + ex.Message);
//            }
//        }

//        public async Task<IEnumerable<MensagemChat>> GetItens()
//        {
//            try
//            {
//                return await _mensagemChatRepository.GetItens();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Erro ao buscar mensagens ativas");
//                throw new Exception("Erro ao buscar mensagens ativas: " + ex.Message);
//            }
//        }

//        public async Task SaveOrUpdate(MensagemChat model)
//        {
//            try
//            {
//                await _mensagemChatRepository.SaveOrUpdate(model);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Erro ao salvar ou atualizar mensagem");
//                throw new Exception("Erro ao salvar ou atualizar mensagem: " + ex.Message);
//            }
//        }

//        public async Task Delete(int id)
//        {
//            try
//            {
//                await _mensagemChatRepository.Delete(id);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Erro ao deletar mensagem");
//                throw new Exception("Erro ao deletar mensagem: " + ex.Message);
//            }
//        }
//    }
//}
