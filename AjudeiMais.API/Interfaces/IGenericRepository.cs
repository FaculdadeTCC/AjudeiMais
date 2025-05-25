using AjudeiMais.Data.Models.UsuarioModel;

namespace AjudeiMais.API.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<IEnumerable<T>> GetItens();
        Task<T> GetById(int id);
        Task SaveOrUpdate(T model);
        Task Delete(T model);
    }
}
