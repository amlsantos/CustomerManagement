using CustomerManagement.Logic.Common;
using CustomerManagement.Logic.Model;

namespace CustomerManagement.Api.DAL;

public interface IRepository<T> where T : class
{
    Task<Maybe<T>> GetByIdAsync(long id);
    Task<Maybe<T>> GetByNameAsync(string name);
    Task AdAsync(T entity);
    Task CommitAsync();
}