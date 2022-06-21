using CustomerManagement.Api;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CustomerManagement.IntegrationTests;

public class IntegrationTests
{
    protected HttpClient Client { get; }

    protected IntegrationTests()
    {
        var factory = new WebApplicationFactory<Program>();
        
        Client = factory.CreateDefaultClient();
    }
}