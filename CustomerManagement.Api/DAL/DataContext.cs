using CustomerManagement.Logic.Model;
using Microsoft.EntityFrameworkCore;

namespace CustomerManagement.Api.DAL;

public class DataContext : DbContext
{
    protected IConfiguration _configuration;
    
    public DataContext(IConfiguration configuration) => _configuration = configuration;
    
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        var connectionString = _configuration.GetConnectionString("SQLConnection");
        if (!string.IsNullOrEmpty(connectionString))
            builder.UseSqlServer(connectionString);
        else
            builder.UseInMemoryDatabase("CustomerManagement");
    }
    
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Industry> Industries { get; set; }
}