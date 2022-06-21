using System.Text.Json.Serialization;
using CustomerManagement.Api.DAL;
using CustomerManagement.Api.Utils;
using CustomerManagement.Logic.Model;
using Microsoft.EntityFrameworkCore;
using NullGuard;

[assembly: NullGuard(ValidationFlags.All)]
namespace CustomerManagement.Api;

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        var services = builder.Services;
        ConfigureServices(services);
        ConfigureDI(services);
        
        var app = builder.Build();
        RunMigrations(app);
        ConfigureApp(app);

        app.Run();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<DataContext>();
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAllHeaders", builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                }
            );
        });
        services.AddControllers()
            .AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    private static void ConfigureDI(IServiceCollection services)
    {
        services.AddScoped<IEmailGateway, EmailGateway>();
        services.AddScoped<IRepository<Customer>, CustomerRepository>();
    }

    private static void RunMigrations(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();

        context.Database.Migrate();
    }

    private static void ConfigureApp(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseMiddleware<ExceptionMiddleware>();
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
    }
}