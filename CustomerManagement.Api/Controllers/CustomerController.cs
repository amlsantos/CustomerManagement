using CustomerManagement.Api.Controllers.Common;
using CustomerManagement.Api.DAL;
using CustomerManagement.Api.Models;
using CustomerManagement.Logic.Common;
using CustomerManagement.Logic.Model;
using Microsoft.AspNetCore.Mvc;

namespace CustomerManagement.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CustomerController : BaseController
{
    private readonly IEmailGateway _emailGateway;
    private readonly IRepository<Customer> _customerRepository;

    public CustomerController(
        IEmailGateway emailGateway,
        IRepository<Customer> customerRepository) 
        : base(customerRepository)
    {
        _emailGateway = emailGateway;
        _customerRepository = customerRepository;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCustomerModel model)
    {
        var customerName = Name.Create(model.Name);
        if (customerName.IsFailure)
            return Error(customerName.Error);

        var primaryEmail = Email.Create(model.PrimaryEmail);
        if (primaryEmail.IsFailure)
            return Error(primaryEmail.Error);

        var secondaryEmailResult = GetSecondaryEmail(model.SecondaryEmail);
        var industryOrNothing = Industry.Get(model.Industry);
        if (industryOrNothing.IsFailure)
            return Error($"Industry name is invalid: {model.Industry}");

        var customer = new Customer(customerName.Value, primaryEmail.Value, secondaryEmailResult.Value, industryOrNothing.Value);
        
        await _customerRepository.AddAsync(customer);
        await _customerRepository.CommitAsync();
        
        var dto = new
        {
            Id = customer.Id,
            Name = customer.Name.Value,
            PrimaryEmail = customer.PrimaryEmail.Value,
            SecondaryEmail = customer.SecondaryEmail.Value.Value,
            Industry = industryOrNothing.Value.Name,
            Settings = new
            {
                IsDisabled = customer.EmailingSettings.IsDisabled,
                Industry = customer.EmailingSettings.Industry
            },
            Status = customer.Type
        };

        return Ok(dto);
    }

    private Result<Maybe<Email>> GetSecondaryEmail(string value)
    {
        if (value == null)
            return Result.Ok<Maybe<Email>>(null);

        var email = Email.Create(value);
        if (email.IsSuccess)
            return Result.Ok<Maybe<Email>>(email.Value);
        
        return Result.Fail<Maybe<Email>>(email.Error);
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> Get(long id)
    {
        var customerOrNothing = await _customerRepository.GetByIdAsync(id);
        if (customerOrNothing.HasNoValue)
            return Error("Customer with such Id is not found: " + id);

        var customer = customerOrNothing.Value;
        
        var dto = new
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
            
        return await Success(dto);
    }

    [HttpPut]
    [Route("{id}")]
    public async Task<IActionResult> Update(int id, UpdateCustomerModel model)
    {
        var customerOrNothing = await _customerRepository.GetByIdAsync(id);
        if (customerOrNothing.HasNoValue)
            return Error("Customer with such Id is not found: " + id);
    
        var industryResult = Industry.Get(model.Industry);
        if (industryResult.IsFailure)
            return Error("Industry name is invalid: " + model.Industry);

        var customer = customerOrNothing.Value;
        var industry = industryResult.Value;
        customer.UpdateIndustry(industry);

        var dto = new
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
        
        return await Success(dto);
    }
    
    [HttpDelete]
    [Route("{id}/emailing")]
    public async Task<IActionResult> DisableEmailing(long id)
    {
        var customerOrNothing = await _customerRepository.GetByIdAsync(id);
        if (customerOrNothing.HasNoValue)
            return Error("Customer with such Id is not found: " + id);

        var customer = customerOrNothing.Value;
        customer.DisableEmailing();
        
        var dto = new
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
        
        return await Success(dto);
    }
    
    [HttpPost]
    [Route("{id}/promotion")]
    public async Task<IActionResult> Promote(long id)
    {
        var customerOrNothing = await _customerRepository.GetByIdAsync(id);
        if (customerOrNothing.HasNoValue)
            return Error("Customer with such Id is not found: " + id);
    
        if (!customerOrNothing.Value.CanBePromoted())
            return Error("The customer has the highest status possible");
    
        var customer = customerOrNothing.Value;
        customer.Promote();
        
        var emailSent = _emailGateway.SendPromotionNotification(customer.PrimaryEmail, customer.Type);
        if (emailSent.IsFailure)
            return Error("Unable to sent notification email");
    
        var dto = new
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
        
        return await Success(dto);
    }
}