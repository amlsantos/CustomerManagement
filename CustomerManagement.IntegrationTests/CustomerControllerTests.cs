using System.Net;
using System.Net.Http.Json;
using CustomerManagement.Api;
using CustomerManagement.Api.Models;
using CustomerManagement.Logic.Model;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace CustomerManagement.IntegrationTests;

public class CustomerControllerTests : IntegrationTests
{
    [Fact]
    public async Task Create_ValidCustomer_ReturnsOK()
    {
        // arrange
        var model = new CreateCustomerModel
        {
            Name = "name",
            PrimaryEmail = "name@email.com",
            SecondaryEmail = "name@email2.com",
            Industry = "Cars"
        };
        
        // act
        using var response = await Client.PostAsJsonAsync("customer", model);

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Create_InvalidCustomerName_ReturnsBadRequest()
    {
        // arrange
        var model = new CreateCustomerModel
        {
            Name = "name",
            PrimaryEmail = "",
            SecondaryEmail = "",
            Industry = "Cars"
        };
        
        // act
        using var response = await Client.PostAsJsonAsync("customer", model);
        
        // assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
    [Fact]
    public async Task Get_UserIsPresent_ReturnsOk()
    {
        // act
        var response = await Client.GetAsync($"Customer/{1}");

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Get_UserIsNotPresent_ReturnsBadRequest()
    {
        // act
        var response = await Client.GetAsync($"Customer/{-1}");

        // assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}