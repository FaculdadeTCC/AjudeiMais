//using AjudeiMais.API.DTO;
//using AjudeiMais.API.Services;
//using AjudeiMais.Data.Models.ChatModel;
//using Microsoft.AspNetCore.Mvc;

//namespace AjudeiMais.API.Controllers
//{
//[ApiController]
//[Route("api/[controller]")]
//public class ChatController : ControllerBase
//{
//	private readonly ChatService _service;

//	public ChatController(ChatService service)
//	{
//		_service = service;
//	}

//	[HttpPost("iniciar")]
//	public async Task<IActionResult> IniciarChat([FromBody] IniciarChatDTO dto)
//	{
//		try
//		{
//			var chat = await _service.IniciarOuObterChatAsync(dto);
//			return Ok(new ApiResponse<Chat>
//			{
//				Success = true,
//				Type = "success",
//				Message = "Chat iniciado com sucesso",
//				Data = chat
//			});
//		}
//		catch (Exception ex)
//		{
//			return BadRequest(new ApiResponse<Chat>
//			{
//				Success = false,
//				Type = "error",
//				Message = "Erro ao iniciar o chat",
//				Errors = new List<string> { ex.Message }
//			});
//		}
//	}

//	[HttpPost("mensagem")]
//	public async Task<IActionResult> EnviarMensagem([FromBody] EnviarMensagemDTO dto)
//	{
//		try
//		{
//			var mensagem = await _service.EnviarMensagemAsync(dto);
//			return Ok(new ApiResponse<MensagemChat>
//			{
//				Success = true,
//				Type = "success",
//				Message = "Mensagem enviada com sucesso",
//				Data = mensagem
//			});
//		}
//		catch (Exception ex)
//		{
//			return BadRequest(new ApiResponse<MensagemChat>
//			{
//				Success = false,
//				Type = "error",
//				Message = "Erro ao enviar a mensagem",
//				Errors = new List<string> { ex.Message }
//			});
//		}
//	}

//	[HttpGet("{chatId}/mensagens")]
//	public async Task<IActionResult> ObterMensagens(int chatId)
//	{
//		try
//		{
//			var mensagens = await _service.ObterMensagensAsync(chatId);
//			return Ok(new ApiResponse<List<MensagemChat>>
//			{
//				Success = true,
//				Type = "success",
//				Message = "Mensagens obtidas com sucesso",
//				Data = mensagens
//			});
//		}
//		catch (Exception ex)
//		{
//			return BadRequest(new ApiResponse<List<MensagemChat>>
//			{
//				Success = false,
//				Type = "error",
//				Message = "Erro ao obter as mensagens",
//				Errors = new List<string> { ex.Message }
//			});
//		}
//	}
//}
	

//}
