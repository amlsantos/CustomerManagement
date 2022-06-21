using CustomerManagement.Api.Controllers;
using CustomerManagement.Api.DAL;
using CustomerManagement.Api.Models;
using CustomerManagement.Logic.Common;
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
    private readonly CustomerController _controller;

    public CustomerControllerTests()
    {
        _emailGatewayMock = new Mock<IEmailGateway>();
        _customerRepositoryMock = new Mock<IRepository<Customer>>();
        _controller = new CustomerController(_emailGatewayMock.Object, _customerRepositoryMock.Object);
    }
    
    [Fact]
    public async Task  Create_ValidCustomer_ReturnsOK()
    {
        // arrange
        var industry = Industry.Cars;
        var model = new CreateCustomerModel
        {
            Name = "user",
            Industry = industry.Name,
            PrimaryEmail = "user@email.com",
            SecondaryEmail = "user2@email.com"
        };
        _customerRepositoryMock.Setup(c => c.AddAsync(It.IsAny<Customer>()));
    
        // act
        var result = await _controller.Create(model);
        var actual = (result as OkObjectResult).Value;
        var expected = new
        {
            Id = 0,
            model.Name,
            model.PrimaryEmail,
            model.SecondaryEmail,
            model.Industry,
            Settings = new
            {
                IsDisabled = false,
                Industry = industry
            },
            Status = CustomerType.Regular
        };
        
        // assert
        Assert.IsType<OkObjectResult>(result);
        Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(actual));
    }

    [Fact]
    public async Task Create_InvalidCustomerName_ReturnsBadRequest()
    {
        // arrange
        var industry = Industry.Cars;
        var model = new CreateCustomerModel
        {
            Name = string.Empty,
            Industry = industry.Name,
            PrimaryEmail = string.Empty,
            SecondaryEmail = string.Empty
        };
        _customerRepositoryMock.Setup(c => c.AddAsync(It.IsAny<Customer>()));
        
        // act
        var result = await _controller.Create(model);
        
        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
    
    [Fact]
    public async Task Create_InvalidEmail_ReturnsBadRequest()
    {
        // arrange
        var industry = Industry.Cars;
        var model = new CreateCustomerModel
        {
            Name = "user",
            Industry = industry.Name,
            PrimaryEmail = string.Empty,
            SecondaryEmail = string.Empty
        };
        _customerRepositoryMock.Setup(c => c.AddAsync(It.IsAny<Customer>()));
        
        // act
        var result = await _controller.Create(model);

        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Get_UserIsPresent_ReturnsOk()
    {
        // arrange
        var customer = new Customer(
            Name.Create("name").Value, 
            Email.Create("email@email.com").Value, 
            new Maybe<Email>(Email.Create("secondaryemail@email.com").Value), 
            Industry.Cars);
        var customerOrNothing = new Maybe<Customer>(customer);
        
        _customerRepositoryMock
            .Setup(c => c.GetByIdAsync(It.IsAny<long>()))
            .ReturnsAsync(customerOrNothing);

        // act
        var result = await _controller.Get(0);
        var actual = (result as OkObjectResult).Value;
        var expected = new
        {
            Id = customer.Id,
            Name = customer.PrimaryEmail.Value,
            PrimaryEmail = customer.PrimaryEmail.Value,
            SecondaryEmail = customer.SecondaryEmail.Value.Value,
            Settings = new
            {
                IsDisabled = customer.EmailingSettings.IsDisabled,
                Industry = customer.EmailingSettings.Industry
            },
            Status = customer.Type
        };

        // assert
        Assert.IsType<OkObjectResult>(result);
        Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(actual));
    }

    [Fact]
    public async Task Get_UserIsNotPresent_ReturnsBadRequest()
    {
        // arrange
        var customerOrNothing = new Maybe<Customer>(null);
        _customerRepositoryMock
            .Setup(c => c.GetByIdAsync(It.IsAny<long>()))
            .ReturnsAsync(customerOrNothing);
        
        // act
        var result = await _controller.Get(0);
        
        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Update_UserIsPresent_ReturnsOk()
    {
        var model = new UpdateCustomerModel
        {
            Industry = Industry.Other.Name,
            IsEmailDisabled = true,
        };
        
        var customer = new Customer(
            Name.Create("name").Value, 
            Email.Create("email@email.com").Value, 
            new Maybe<Email>(Email.Create("secondaryemail@email.com").Value), 
            Industry.Cars);
        var customerOrNothing = new Maybe<Customer>(customer);
        
        _customerRepositoryMock
            .Setup(c => c.GetByIdAsync(It.IsAny<long>()))
            .ReturnsAsync(customerOrNothing);
        
        // act
        var result = await _controller.Update(0, model);
        var actual = (result as OkObjectResult).Value;
        var expected = new
        {
            Id = customer.Id,
            Name = customer.PrimaryEmail.Value,
            PrimaryEmail = customer.PrimaryEmail.Value,
            SecondaryEmail = customer.SecondaryEmail.Value.Value,
            Settings = new
            {
                IsDisabled = customer.EmailingSettings.IsDisabled,
                Industry = Industry.Other
            },
            Status = customer.Type
        };
        
        // assert
        Assert.IsType<OkObjectResult>(result);
        Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(actual));
    }

    [Fact]
    public async Task Update_UserIsNotPresent_ReturnsBadRequest()
    {
        // arrange
        var model = new UpdateCustomerModel
        {
            Industry = Industry.Other.Name,
            IsEmailDisabled = true,
        };
        var customerOrNothing = new Maybe<Customer>(null);
        
        _customerRepositoryMock
            .Setup(c => c.GetByIdAsync(It.IsAny<long>()))
            .ReturnsAsync(customerOrNothing);
     
        // act
        await _controller.Update(0, model);
        var result = await _controller.Update(0, model);
        
        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task DisableEmailing_UserIsPresent_ReturnsOk()
    {
        // arrange
        var customer = new Customer(
            Name.Create("name").Value, 
            Email.Create("email@email.com").Value, 
            new Maybe<Email>(Email.Create("secondaryemail@email.com").Value), 
            Industry.Cars);
        var customerOrNothing = new Maybe<Customer>(customer);
        
        _customerRepositoryMock
            .Setup(c => c.GetByIdAsync(It.IsAny<long>()))
            .ReturnsAsync(customerOrNothing);
        
        // act
        var result = await _controller.DisableEmailing(0);
        var actual = (result as OkObjectResult).Value;
        var expected = new
        {
            Id = customer.Id,
            Name = customer.PrimaryEmail.Value,
            PrimaryEmail = customer.PrimaryEmail.Value,
            SecondaryEmail = customer.SecondaryEmail.Value.Value,
            Settings = new
            {
                IsDisabled = true,
                Industry = customer.EmailingSettings.Industry
            },
            Status = customer.Type
        };

        // assert
        Assert.IsType<OkObjectResult>(result);
        Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(actual));
    }

    [Fact]
    public async Task DisableEmailing_UserIsNotPresent_ReturnsBadRequest()
    {
        // arrange
        var customerOrNothing = new Maybe<Customer>(null);
        
        _customerRepositoryMock
            .Setup(c => c.GetByIdAsync(It.IsAny<long>()))
            .ReturnsAsync(customerOrNothing);
        
        // act
        var result = await _controller.DisableEmailing(0);
        
        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Promote_UserIsPresentAndCanBePromoted_ReturnsOk()
    {
        var email = Email.Create("email@email.com").Value;
        var customer = new Customer(
            Name.Create("name").Value, 
            email, 
            new Maybe<Email>(Email.Create("secondaryemail@email.com").Value), 
            Industry.Cars);
        var customerOrNothing = new Maybe<Customer>(customer);
        
        _emailGatewayMock
            .Setup(eg => eg.SendPromotionNotification(email, It.IsAny<CustomerType>()))
            .Returns(Result.Ok());
        _customerRepositoryMock
            .Setup(c => c.GetByIdAsync(It.IsAny<long>()))
            .ReturnsAsync(customerOrNothing);

        // act
        var result = await _controller.Promote(0);
        var actual = (result as OkObjectResult).Value;
        var expected = new
        {
            customer.Id,
            Name = customer.PrimaryEmail.Value,
            PrimaryEmail = customer.PrimaryEmail.Value,
            SecondaryEmail = customer.SecondaryEmail.Value.Value,
            Settings = new
            {
                customer.EmailingSettings.IsDisabled, 
                customer.EmailingSettings.Industry
            },
            Status = CustomerType.Preferred
        };
        
        // assert
        Assert.IsType<OkObjectResult>(result);
        Assert.Equal(JsonConvert.SerializeObject(expected), JsonConvert.SerializeObject(actual));
    }

    [Fact]
    public async Task Promote_UserIsNotPresent_ReturnsBadRequest()
    {
        // arrange
        var email = Email.Create("email@email.com").Value;
        var customerOrNothing = new Maybe<Customer>(null);
        
        _emailGatewayMock
            .Setup(eg => eg.SendPromotionNotification(email, It.IsAny<CustomerType>()))
            .Returns(Result.Ok());
        _customerRepositoryMock
            .Setup(c => c.GetByIdAsync(It.IsAny<long>()))
            .ReturnsAsync(customerOrNothing);
        
        // act
        var result = await _controller.Promote(0);
        
        // assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
}