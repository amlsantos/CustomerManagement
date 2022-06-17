using CustomerManagement.Api.Controllers;
using CustomerManagement.Api.DAL;
using CustomerManagement.Api.Models;
using CustomerManagement.Logic.Model;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace CustomerManagement.UnitTests;

public class CustomerControllerTests
{
    private readonly Mock<IEmailGateway> _emailGatewayMock;
    private readonly Mock<IRepository<Customer>> _customerRepositoryMock;
    private readonly Mock<IRepository<Industry>> _industryRepositoryMock;
    private readonly CustomerController _controller;

    public CustomerControllerTests()
    {
        _emailGatewayMock = new Mock<IEmailGateway>();
        _customerRepositoryMock = new Mock<IRepository<Customer>>();
        _industryRepositoryMock = new Mock<IRepository<Industry>>();

        _controller = new CustomerController(
            _emailGatewayMock.Object, 
            _customerRepositoryMock.Object,
            _industryRepositoryMock.Object);
    }
    
    [Fact]
    public async Task  Create_ValidCustomer_ReturnsOK()
    {
        // arrange
        var model = new CreateCustomerModel
        {
            Name = "user",
            Industry = Industry.CarsIndustry,
            PrimaryEmail = "user@email.com",
            SecondaryEmail = "user2@email.com"
        };
    
        var industry = new Industry
        {
            Name = Industry.CarsIndustry
        };
    
        _industryRepositoryMock
            .Setup(i => i.GetByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(industry);
        _customerRepositoryMock.Setup(c => c.AddAsync(It.IsAny<Customer>()));
    
        // act
        var result = await _controller.Create(model);
        var okResult = result as OkObjectResult;
    
        // assert
        Assert.IsType<OkObjectResult>(result);
        Assert.Equal(JsonConvert.SerializeObject(okResult.Value), JsonConvert.SerializeObject(new Customer("user", "user@email.com", "user2@email.com", industry)));
    }

    [Fact]
    public async Task Create_InvalidCustomerName_ThrowBusinessException()
    {
        // arrange
        var model = new CreateCustomerModel
        {
            Name = string.Empty,
            Industry = Industry.CarsIndustry,
            PrimaryEmail = string.Empty,
            SecondaryEmail = string.Empty
        };
    
        var industry = new Industry
        {
            Name = Industry.CarsIndustry
        };
    
        _industryRepositoryMock
            .Setup(i => i.GetByNameAsync(It.IsAny<string>())).ReturnsAsync(industry);
        _customerRepositoryMock.Setup(c => c.AddAsync(It.IsAny<Customer>()));
        
        // act
        var result = await _controller.Create(model);
        
        // assert
        Assert.IsType<BadRequestResult>(result);
    }
    
    [Fact]
    public async Task Create_InvalidEmail_ThrowBusinessException()
    {
        // arrange
        var model = new CreateCustomerModel
        {
            Name = "user",
            Industry = Industry.CarsIndustry,
            PrimaryEmail = string.Empty,
            SecondaryEmail = string.Empty
        };
    
        var industry = new Industry
        {
            Name = Industry.CarsIndustry
        };
    
        _industryRepositoryMock
            .Setup(i => i.GetByNameAsync(It.IsAny<string>())).ReturnsAsync(industry);
        _customerRepositoryMock.Setup(c => c.AddAsync(It.IsAny<Customer>()));
        
        // act
        var result = await _controller.Create(model);
        
        // assert
        Assert.IsType<BadRequestResult>(result);
    }
    
    [Fact]
    public async Task Create_ValidCustomer_ThrowException()
    {
        // arrange
        var model = new CreateCustomerModel
        {
            Name = "user",
            Industry = Industry.CarsIndustry,
            PrimaryEmail = "user@email.com",
            SecondaryEmail = "user2@email.com"
        };
    
        var industry = new Industry
        {
            Name = Industry.CarsIndustry
        };
    
        _industryRepositoryMock
            .Setup(i => i.GetByNameAsync(It.IsAny<string>())).ReturnsAsync(industry);
        _customerRepositoryMock.Setup(c => c.AddAsync(It.IsAny<Customer>())).Throws<Exception>();
        
        // act
        var result = await _controller.Create(model);
        
        // assert
        Assert.IsType<StatusCodeResult>(result);
    }
}