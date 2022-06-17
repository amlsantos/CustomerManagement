using System.Text.RegularExpressions;
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
        var nameError = ValidateName(model.Name);
        if (!string.IsNullOrEmpty(nameError))
            return Error(nameError);

        var emailError = ValidateEmail(model.PrimaryEmail, "Primary email");
        if (!string.IsNullOrEmpty(emailError))
            return Error(emailError);

        if (model.SecondaryEmail != null)
        {
            emailError = ValidateEmail(model.SecondaryEmail, "Secondary email");
            if (!string.IsNullOrEmpty(emailError))
                return Error(emailError);
        }

        var industry = await _industryRepository.GetByNameAsync(model.Industry);
        if (industry == null)
            return Error($"Industry name is invalid: {model.Industry}");

        var customer = new Customer(model.Name, model.PrimaryEmail, model.SecondaryEmail, industry);
        await _customerRepository.AddAsync(customer);

        return await Success(customer);
    }

    private string ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "Customer name should not be empty";
        if (name.Length > 200)
            return "Customer name is too long";

        return string.Empty;
    }

    private string ValidateEmail(string email, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(email))
            return fieldName + " should not be empty";
        if (email.Length > 256)
            return fieldName + " is too long";
        if (!Regex.IsMatch(email, @"^(.+)@(.+)$"))
            return fieldName + " is invalid";

        return string.Empty;
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> Get(long id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null)
            return Error("Customer with such Id is not found: " + id);

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
        if (!emailSent)
            return Error("Unable to sent notification email");

        return await Success(customer);
    }
}