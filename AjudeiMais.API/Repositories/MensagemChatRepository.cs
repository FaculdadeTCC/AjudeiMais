//using AjudeiMais.API.Interfaces;
//using AjudeiMais.Data.Context;
//using AjudeiMais.Data.Models.ChatModel;
//using Microsoft.EntityFrameworkCore;

//namespace AjudeiMais.API.Repositories
//{
//    public class MensagemChatRepository : IGenericRepository<MensagemChat>
//    {
//        private readonly ApplicationDbContext _context;

//        public MensagemChatRepository(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        public async Task Delete(int id)
//        {
//            var mensagem = await _context.MensagemChat.FirstOrDefaultAsync(x => x.MensagemChat_ID == id);

//            if (mensagem != null)
//            {
//                mensagem.Habilitado = false;
//                mensagem.Excluido = true;

//                _context.MensagemChat.Update(mensagem);

//                await _context.SaveChangesAsync();
//            }
//        }

//        public async Task<IEnumerable<MensagemChat>> GetAll()
//        {
//            return await _context.MensagemChat
//                .OrderBy(x => x.MensagemChat_ID)
//                .ToListAsync();
//        }

//        public async Task<MensagemChat> GetById(int id)
//        {
//            return await _context.MensagemChat
//                .FirstOrDefaultAsync(x => x.MensagemChat_ID == id && x.Habilitado && !x.Excluido);
//        }

//        public async Task SaveOrUpdate(MensagemChat model)
//        {
//            if (model.MensagemChat_ID > 0)
//            {
//                _context.MensagemChat.Update(model);
//            }
//            else
//            {
//                model.Habilitado = true;
//                model.Excluido = false;
//                _context.MensagemChat.Add(model);
//            }

//            await _context.SaveChangesAsync();
//        }

//        public async Task<IEnumerable<MensagemChat>> GetMensagensPorChatId(int chatId)
//        {
//            return await _context.MensagemChat
//                .Where(x => x.Chat_ID == chatId && x.Habilitado && !x.Excluido)
//                .OrderBy(x => x.MensagemChat_ID)
//                .ToListAsync();
//        }

//        public async Task<IEnumerable<MensagemChat>> GetItens()
//        {
//            return await _context.MensagemChat
//                .Where(x => x.Habilitado && !x.Excluido)
//                .OrderBy(x => x.MensagemChat_ID)
//                .ToListAsync();
//        }
//    }
//}
