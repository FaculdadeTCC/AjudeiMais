//using AjudeiMais.API.Interfaces;
//using AjudeiMais.Data.Context;
//using AjudeiMais.Data.Models.ChatModel;
//using AjudeiMais.Data.Models.InstituicaoModel;
//using Microsoft.EntityFrameworkCore;
//using System;

//namespace AjudeiMais.API.Repositories
//{
//	public class ChatRepository : IChatRepository
//	{
//		private readonly ApplicationDbContext _context;

//		public ChatRepository(ApplicationDbContext context)
//		{
//			_context = context;
//		}

//		public async Task<Chat> ObterOuCriarChatAsync(int usuarioId, int instituicaoId)
//		{
//			var chat = await _context.Chat
//				.FirstOrDefaultAsync(c => c.Usuario_ID == usuarioId && c.Instituicao_ID == instituicaoId && !c.Excluido);

//			if (chat != null) return chat;

//			chat = new Chat
//			{
//				Usuario_ID = usuarioId,
//				Instituicao_ID = instituicaoId,
//				DataAbertura = DateTime.Now,
//				Habilitado = true,
//				Excluido = false,
//				GUID = Guid.NewGuid().ToString()
//			};

//			_context.Chat.Add(chat);
//			await _context.SaveChangesAsync();

//			return chat;
//		}

//		public async Task<List<MensagemChat>> ObterMensagensPorChatAsync(int chatId)
//		{
//			return await _context.MensagemChat
//				.Where(m => m.Chat_ID == chatId && !m.Excluido)
//				.OrderBy(m => m.MensagemChat_ID)
//				.ToListAsync();
//		}

//		public async Task<MensagemChat> EnviarMensagemAsync(MensagemChat mensagem)
//		{
//			_context.MensagemChat.Add(mensagem);
//			await _context.SaveChangesAsync();
//			return mensagem;
//		}

//		Task<Chat> IChatRepository.ObterOuCriarChatAsync(int usuarioId, int instituicaoId)
//		{
//			throw new NotImplementedException();
//		}

//		Task<List<MensagemChat>> IChatRepository.ObterMensagensPorChatAsync(int chatId)
//		{
//			throw new NotImplementedException();
//		}

//		Task<MensagemChat> IChatRepository.EnviarMensagemAsync(MensagemChat mensagem)
//		{
//			throw new NotImplementedException();
//		}

//		Task<IEnumerable<Chat>> IGenericRepository<Chat>.GetAll()
//		{
//			throw new NotImplementedException();
//		}

//		Task<IEnumerable<Chat>> IGenericRepository<Chat>.GetItens()
//		{
//			throw new NotImplementedException();
//		}

//		Task<Chat> IGenericRepository<Chat>.GetById(int id)
//		{
//			throw new NotImplementedException();
//		}

//		Task IGenericRepository<Chat>.SaveOrUpdate(Chat model)
//		{
//			throw new NotImplementedException();
//		}

//		Task IGenericRepository<Chat>.Delete(Chat model)
//		{
//			throw new NotImplementedException();
//		}
//	}

//}
