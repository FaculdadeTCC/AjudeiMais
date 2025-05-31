//using AjudeiMais.API.DTO;
//using AjudeiMais.API.Interfaces;
//using AjudeiMais.API.Repositories;
//using AjudeiMais.Data.Models.ChatModel;

//namespace AjudeiMais.API.Services
//{
//	public class ChatService
//	{
//		private readonly IChatRepository _repository;

//		public ChatService(IChatRepository repository)
//		{
//			_repository = repository;
//		}

//		public async Task<Chat> IniciarOuObterChatAsync(IniciarChatDTO dto)
//		{
//			return await _repository.ObterOuCriarChatAsync(dto.Usuario_ID, dto.Instituicao_ID);
//		}

//		public async Task<List<MensagemChat>> ObterMensagensAsync(int chatId)
//		{
//			return await _repository.ObterMensagensPorChatAsync(chatId);
//		}

//		public async Task<MensagemChat> EnviarMensagemAsync(EnviarMensagemDTO dto)
//		{
//			var mensagem = new MensagemChat
//			{
//				Chat_ID = dto.Chat_ID,
//				Mensagem = dto.Mensagem,
//				TipoRemetente = dto.TipoRemetente,
//				Remetente_ID = dto.Remetente_ID,
//				GUID = Guid.NewGuid().ToString(),
//				Habilitado = true,
//				Excluido = false
//			};

//			return await _repository.EnviarMensagemAsync(mensagem);
//		}
//	}

//}
