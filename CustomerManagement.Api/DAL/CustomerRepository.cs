using CustomerManagement.Logic.Model;
using Microsoft.EntityFrameworkCore;

namespace CustomerManagement.Api.DAL;

public class CustomerRepository : IRepository<Customer>
{
    private readonly DataContext _context;

    public CustomerRepository(DataContext context) => _context = context;

    public async Task<Customer?> GetByIdAsync(long id)
    {
        return await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task AddAsync(Customer customer)
    {
        await _context.Customers.AddAsync(customer);
    }

    public async Task CommitAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<Customer?> GetByNameAsync(string name)
    {
        return await _context.Customers.FirstOrDefaultAsync(c => c.Name.Value.Contains(name));
    }
}
