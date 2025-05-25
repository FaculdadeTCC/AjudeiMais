//using AjudeiMais.API.Repositories;
//using AjudeiMais.Data.Models.ChatModel;

//namespace AjudeiMais.API.Services
//{
//    public class ChatService
//    {
//        private readonly ChatRepository _chatRepository;
//        private readonly ILogger<ChatService> _logger;

//        public ChatService(ChatRepository chatRepository, ILogger<ChatService> logger)
//        {
//            _chatRepository = chatRepository;
//            _logger = logger;
//        }

//        public async Task<IEnumerable<Chat>> GetAll()
//        {
//            try
//            {
//                return await _chatRepository.GetAll();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Erro ao buscar todos os chats.");
//                throw new Exception("Erro ao buscar todos os chats. " + ex.Message);
//            }
//        }

//        public async Task<Chat> GetById(int id)
//        {
//            try
//            {
//                return await _chatRepository.GetById(id);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, $"Erro ao buscar chat com ID {id}.");
//                throw new Exception("Erro ao buscar chat. " + ex.Message);
//            }
//        }

//        public async Task<IEnumerable<Chat>> GetItens()
//        {
//            try
//            {
//                return await _chatRepository.GetItens();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Erro ao buscar chats habilitados.");
//                throw new Exception("Erro ao buscar chats habilitados. " + ex.Message);
//            }
//        }

//        public async Task SaveOrUpdate(Chat model)
//        {
//            try
//            {
//                await _chatRepository.SaveOrUpdate(model);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Erro ao salvar ou atualizar chat.");
//                throw new Exception("Erro ao salvar ou atualizar chat. " + ex.Message);
//            }
//        }

//        public async Task Delete(int id)
//        {
//            try
//            {
//                await _chatRepository.Delete(id);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Erro ao deletar chat.");
//                throw new Exception("Erro ao deletar chat. " + ex.Message);
//            }
//        }

//        public async Task<IEnumerable<MensagemChat>> GetMensagensByChatId(int chatId)
//        {
//            try
//            {
//                return await _chatRepository.GetMensagensByChatId(chatId);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, $"Erro ao buscar mensagens do chat ID {chatId}.");
//                throw new Exception("Erro ao buscar mensagens. " + ex.Message);
//            }
//        }
//    }
//}
