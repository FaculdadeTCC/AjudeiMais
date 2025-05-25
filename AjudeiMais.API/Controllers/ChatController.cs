//using AjudeiMais.API.Services;
//using AjudeiMais.Data.Models.ChatModel;
//using Microsoft.AspNetCore.Mvc;

//namespace AjudeiMais.API.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class ChatController : ControllerBase
//    {
//        private readonly ChatService _chatService;
//        private readonly ILogger<ChatController> _logger;

//        public ChatController(ChatService chatService, ILogger<ChatController> logger)
//        {
//            _chatService = chatService;
//            _logger = logger;
//        }

//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<Chat>>> GetAll()
//        {
//            try
//            {
//                var chats = await _chatService.GetAll();
//                return Ok(chats);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Erro ao listar todos os chats.");
//                return StatusCode(500, ex.Message);
//            }
//        }

//        [HttpGet("{id}")]
//        public async Task<ActionResult<Chat>> GetById(int id)
//        {
//            try
//            {
//                var chat = await _chatService.GetById(id);
//                if (chat == null)
//                    return NotFound();

//                return Ok(chat);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, $"Erro ao buscar chat com ID {id}.");
//                return StatusCode(500, ex.Message);
//            }
//        }

//        [HttpGet("ativos")]
//        public async Task<ActionResult<IEnumerable<Chat>>> GetItens()
//        {
//            try
//            {
//                var chats = await _chatService.GetItens();
//                return Ok(chats);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Erro ao buscar chats ativos.");
//                return StatusCode(500, ex.Message);
//            }
//        }

//        [HttpPost]
//        public async Task<ActionResult> Save([FromBody] Chat model)
//        {
//            try
//            {
//                await _chatService.SaveOrUpdate(model);
//                return Ok();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Erro ao salvar ou atualizar chat.");
//                return StatusCode(500, ex.Message);
//            }
//        }

//        [HttpDelete("{id}")]
//        public async Task<ActionResult> Delete(int id)
//        {
//            try
//            {
//                await _chatService.Delete(id);
//                return Ok();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, $"Erro ao deletar chat com ID {id}.");
//                return StatusCode(500, ex.Message);
//            }
//        }

//        [HttpGet("{chatId}/mensagens")]
//        public async Task<ActionResult<IEnumerable<MensagemChat>>> GetMensagensByChatId(int chatId)
//        {
//            try
//            {
//                var mensagens = await _chatService.GetMensagensByChatId(chatId);
//                return Ok(mensagens);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, $"Erro ao buscar mensagens do chat ID {chatId}.");
//                return StatusCode(500, ex.Message);
//            }
//        }
//    }
//}
