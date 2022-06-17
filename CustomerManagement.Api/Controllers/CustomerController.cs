using CustomerManagement.Api.Controllers.Common;
using CustomerManagement.Api.DAL;
using CustomerManagement.Api.Models;
using CustomerManagement.Logic.Model;
using Microsoft.AspNetCore.Mvc;

namespace CustomerManagement.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CustomerController : BaseController
{
    private readonly IEmailGateway _emailGateway;
    private readonly IRepository<Customer> _customerRepository;
    private readonly IRepository<Industry> _industryRepository;

    public CustomerController(
        IEmailGateway emailGateway,
        IRepository<Customer> customerRepository,
        IRepository<Industry> industryRepository) : base(customerRepository, industryRepository)
    {
        _emailGateway = emailGateway;
        _customerRepository = customerRepository;
        _industryRepository = industryRepository;
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

        if (model.SecondaryEmail != null)
        {
            var secondaryEmail = Email.Create(model.SecondaryEmail);
            if (secondaryEmail.IsFailure)
                return Error(secondaryEmail.Error);
        }

        var industry = await _industryRepository.GetByNameAsync(model.Industry);
        if (industry == null)
            return Error($"Industry name is invalid: {model.Industry}");

        var customer = new Customer(customerName.Value, primaryEmail.Value, Email.Create(model.SecondaryEmail).Value, industry);
        await _customerRepository.AddAsync(customer);

        return await Success(customer);
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> Get(long id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null)
            return Error("Customer with such Id is not found: " + id);
        
        var industry = await _industryRepository.GetByIdAsync(customer.IndustryId);
        customer.UpdateIndustry(industry);

        return await Success(customer);
    }

    [HttpPut]
    [Route("{id}")]
    public async Task<IActionResult> Update(int id, UpdateCustomerModel model)
    {
        var customer = await _customerRepository.GetByIdAsync(model.Id);
        if (customer == null)
            return Error("Customer with such Id is not found: " + model.Id);

        var industry = await _industryRepository.GetByNameAsync(model.Industry);
        if (industry == null)
            return Error("Industry name is invalid: " + model.Industry);

        customer.UpdateIndustry(industry);
        
        return await Success(customer);
    }

    [HttpDelete]
    [Route("{id}/emailing")]
    public async Task<IActionResult> DisableEmailing(long id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null)
            return Error("Customer with such Id is not found: " + id);

        var industry = await _industryRepository.GetByIdAsync(customer.IndustryId);
        if (industry != null)
            customer.Industry = industry;
            
        customer.DisableEmailing();
        
        return await Success(customer);
    }

    [HttpPost]
    [Route("{id}/promotion")]
    public async Task<IActionResult> Promote(long id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null)
            return Error("Customer with such Id is not found: " + id);

        if (!customer.CanBePromoted())
            return Error("The customer has the highest status possible");

        customer.Promote();
        
        var emailSent = _emailGateway.SendPromotionNotification(customer.PrimaryEmail, customer.Status);
        if (emailSent.IsFailure)
            return Error("Unable to sent notification email");

        return await Success(customer);
    }
}