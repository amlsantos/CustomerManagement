using CustomerManagement.Logic.Model;
using Microsoft.EntityFrameworkCore;

namespace CustomerManagement.Api.DAL;

public class IndustryRepository : IRepository<Industry>
{
    private readonly DataContext _context;

    public IndustryRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<Industry?> GetByIdAsync(long id)
    {
        return await _context.Industries.FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task AddAsync(Industry entity)
    {
        await _context.Industries.AddAsync(entity);
    }

    public async Task CommitAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<Industry?> GetByNameAsync(string name)
    {
        return await _context.Industries.FirstOrDefaultAsync(i => i.Name.ToLower().Contains(name.ToLower()));
    }
}