using AjudeiMais.Data.Models.ChatModel;
using AjudeiMais.Data.Models.InstituicaoModel;

namespace AjudeiMais.API.Interfaces
{
    public interface IChatRepository : IGenericRepository<Chat>
    {
        Task<IEnumerable<MensagemChat>> GetMensagensByChatId(int chatId);
    }
}
