using CustomerManagement.Logic.Common;
using CustomerManagement.Logic.Model;
using Microsoft.EntityFrameworkCore;

namespace CustomerManagement.Api.DAL;

public class CustomerRepository : IRepository<Customer>
{
    private readonly DataContext _context;
    public CustomerRepository(DataContext context) => _context = context;
    
    public async Task<Maybe<Customer>> GetByIdAsync(long id)
    {
        var value = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);
        return new Maybe<Customer>(value);
    }

    public async Task<Maybe<Customer>> GetByNameAsync(string name)
    {
        var value = await _context.Customers.FirstOrDefaultAsync(c => c.Name.Value.Contains(name));
        return new Maybe<Customer>(value);
    }

    public async Task AdAsync(Customer entity)
    {
        await _context.Customers.AddAsync(entity);
    }

    public async Task CommitAsync()
    {
        await _context.SaveChangesAsync();
    }
}
