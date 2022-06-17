using System.Text.RegularExpressions;
using CustomerManagement.Api.DAL;
using CustomerManagement.Api.Models;
using CustomerManagement.Logic.Common;
using CustomerManagement.Logic.Model;
using Microsoft.AspNetCore.Mvc;

namespace CustomerManagement.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CustomerController : ControllerBase
{
    private readonly IEmailGateway _emailGateway;
    private readonly IRepository<Customer> _customerRepository;
    private readonly IRepository<Industry> _industryRepository;

    public CustomerController(
        IEmailGateway emailGateway,
        IRepository<Customer> customerRepository,
        IRepository<Industry> industryRepository)
    {
        _emailGateway = emailGateway;
        _customerRepository = customerRepository;
        _industryRepository = industryRepository;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCustomerModel model)
    {
        try
        {
            ValidateName(model.Name);
            ValidateEmail(model.PrimaryEmail, "Primary email");
            
            if (model.SecondaryEmail != null)
                ValidateEmail(model.SecondaryEmail, "Secondary email");
            
            var industry = await _industryRepository.GetByNameAsync(model.Industry);
            if (industry == null)
                throw new BusinessException("Industry name is invalid: " + model.Industry);
            
            var customer = new Customer(model.Name, model.PrimaryEmail, model.SecondaryEmail, industry);
            await _customerRepository.AddAsync(customer);
            
            return Ok(customer);
        }
        catch (BusinessException e)
        {
            return BadRequest();
        }
        catch (Exception e)
        {
            return StatusCode(500);
        }
    }

    private void ValidateEmail(string email, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new BusinessException(fieldName + " should not be empty");
        if (email.Length > 256)
            throw new BusinessException(fieldName + " is too long");
        if (!Regex.IsMatch(email, @"^(.+)@(.+)$"))
            throw new BusinessException(fieldName + " is invalid");
    }

    private void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessException("Customer name should not be empty");
        if (name.Length > 200)
            throw new BusinessException("Customer name is too long");
    }

    [HttpPut]
    [Route("{id}")]
    public async Task<IActionResult> Update(int id, UpdateCustomerModel model)
    {
        try
        {
            var customer = await _customerRepository.GetByIdAsync(model.Id);
            if (customer == null)
                throw new BusinessException("Customer with such Id is not found: " + model.Id);

            var industry = await _industryRepository.GetByNameAsync(model.Industry);
            if (industry == null)
                throw new BusinessException("Industry name is invalid: " + model.Industry);

            customer.UpdateIndustry(industry);
            await _customerRepository.CommitAsync();

            return Ok(customer);
        }
        catch (BusinessException e)
        {
            return BadRequest();
        }
        catch (Exception e)
        {
            return StatusCode(500);
        }
    }

    [HttpDelete]
    [Route("{id}/emailing")]
    public async Task<IActionResult> DisableEmailing(long id)
    {
        try
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
                throw new BusinessException("Customer with such Id is not found: " + id);

            customer.DisableEmailing();
            await _customerRepository.CommitAsync();

            return Ok(customer);
        }
        catch (BusinessException e)
        {
            return BadRequest();
        }
        catch (Exception e)
        {
            return StatusCode(500);
        }
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> Get(long id)
    {
        try
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
                throw new BusinessException("Customer with such Id is not found: " + id);

            return Ok(customer);
        }
        catch (BusinessException e)
        {
            return BadRequest();
        }
        catch (Exception e)
        {
            return StatusCode(500);
        }
    }

    [HttpPost]
    [Route("{id}/promotion")]
    public async Task<IActionResult> Promote(long id)
    {
        try
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
                throw new BusinessException("Customer with such Id is not found: " + id);

            if (!customer.CanBePromoted())
                throw new BusinessException("The customer has the highest status possible");

            customer.Promote();
            _emailGateway.SendPromotionNotification(customer.PrimaryEmail, customer.Status);

            await _customerRepository.CommitAsync();
            return Ok(customer);
        }
        catch (BusinessException e)
        {
            return BadRequest();
        }
        catch (Exception e)
        {
            return StatusCode(500);
        }
    }
}