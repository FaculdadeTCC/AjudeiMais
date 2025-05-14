using AjudeiMais.API.Interfaces;
using AjudeiMais.Data.Context;
using AjudeiMais.Data.Models.ChatModel;
using AjudeiMais.Data.Models.InstituicaoModel;
using Microsoft.EntityFrameworkCore;

namespace AjudeiMais.API.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly ApplicationDbContext _context;

        public ChatRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Delete(int id)
        {
            var chat = await _context.Chat.FirstOrDefaultAsync(x => x.Chat_ID == id);
            if (chat != null)
            {
                chat.Habilitado = false;
                chat.Excluido = true;
                _context.Chat.Update(chat);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Chat>> GetAll()
        {
            return await _context.Chat
                .ToListAsync();
        }

        public async Task<Chat> GetById(int id)
        {
            return await _context.Chat
                .Include(c => c.MensagemChats)
                .FirstOrDefaultAsync(x => x.Chat_ID == id);
        }

        public async Task SaveOrUpdate(Chat model)
        {
            if (model.Chat_ID > 0)
            {
                _context.Chat.Update(model);
            }
            else
            {
                model.Habilitado = true;
                model.Excluido = false;
                _context.Chat.Add(model);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<MensagemChat>> GetMensagensByChatId(int chatId)
        {
            return await _context.MensagemChat
                .Where(m => m.Chat_ID == chatId && m.Habilitado && !m.Excluido)
                .OrderBy(m => m.MensagemChat_ID)
                .ToListAsync();
        }

        public async Task<IEnumerable<Chat>> GetItens()
        {
            return await _context.Chat
                .Where(x => x.Habilitado && !x.Excluido)
                .ToListAsync();
        }
    }
}
