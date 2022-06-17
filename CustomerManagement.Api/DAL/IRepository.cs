using CustomerManagement.Logic.Model;

namespace CustomerManagement.Api.DAL;

public interface IRepository<T>
{
    Task<T?> GetByIdAsync(long id);
    Task<T?> GetByNameAsync(string name);
    Task AddAsync(T entity);
    Task CommitAsync();
}