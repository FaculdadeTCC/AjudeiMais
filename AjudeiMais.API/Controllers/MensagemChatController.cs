using AjudeiMais.API.Services;
using AjudeiMais.Data.Models.ChatModel;
using Microsoft.AspNetCore.Mvc;

namespace AjudeiMais.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MensagemChatController : ControllerBase
    {
        private readonly MensagemChatService _mensagemChatService;
        private readonly ILogger<MensagemChatController> _logger;

        public MensagemChatController(MensagemChatService mensagemChatService, ILogger<MensagemChatController> logger)
        {
            _mensagemChatService = mensagemChatService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MensagemChat>>> GetAll()
        {
            try
            {
                var mensagens = await _mensagemChatService.GetAll();
                return Ok(mensagens);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter todas as mensagens");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MensagemChat>> GetById(int id)
        {
            try
            {
                var mensagem = await _mensagemChatService.GetById(id);
                if (mensagem == null)
                    return NotFound();

                return Ok(mensagem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao obter mensagem com ID {id}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("chat/{chatId}")]
        public async Task<ActionResult<IEnumerable<MensagemChat>>> GetMensagensPorChatId(int chatId)
        {
            try
            {
                var mensagens = await _mensagemChatService.GetMensagensPorChatId(chatId);
                return Ok(mensagens);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao obter mensagens do chat {chatId}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Save([FromBody] MensagemChat model)
        {
            try
            {
                await _mensagemChatService.SaveOrUpdate(model);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar ou atualizar mensagem");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _mensagemChatService.Delete(id);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao deletar mensagem com ID {id}");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
