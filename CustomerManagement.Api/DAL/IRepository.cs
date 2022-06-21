using CustomerManagement.Logic.Common;

namespace CustomerManagement.Api.DAL;

public interface IRepository<T> where T : class
{
    Task<Maybe<T>> GetByIdAsync(long id);
    Task<Maybe<T>> GetByNameAsync(string name);
    Task AddAsync(T entity);
    Task CommitAsync();
}