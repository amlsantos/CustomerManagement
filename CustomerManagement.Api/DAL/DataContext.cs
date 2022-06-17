using CustomerManagement.Logic.Model;
using Microsoft.EntityFrameworkCore;

namespace CustomerManagement.Api.DAL;

public class DataContext : DbContext
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Industry> Industries { get; set; }
    
    private readonly IConfiguration _configuration;

    public DataContext(IConfiguration configuration) => _configuration = configuration;
    
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        var connectionString = _configuration.GetConnectionString("SQLConnection");
        
        if (!string.IsNullOrEmpty(connectionString))
            builder.UseSqlServer(connectionString);
        else
            builder.UseInMemoryDatabase("CustomerManagement");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>().OwnsOne(x => x.Settings);
        modelBuilder.Entity<Customer>().Property(c => c.IndustryId).HasColumnName("IndustryId");
        modelBuilder.Entity<Customer>()
            .Property(c => c.Name)
            .HasConversion(n => n.Value, s => Name.Create(s).Value);
        modelBuilder.Entity<Customer>()
            .Property(c => c.PrimaryEmail)
            .HasConversion(n => n.Value, s => Email.Create(s).Value);
        modelBuilder.Entity<Customer>()
            .Property(c => c.SecondaryEmail)
            .HasConversion(n => n.Value, s => Email.Create(s).Value);
    }
}