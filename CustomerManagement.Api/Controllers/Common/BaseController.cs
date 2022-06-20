using CustomerManagement.Api.DAL;
using CustomerManagement.Logic.Model;
using Microsoft.AspNetCore.Mvc;

namespace CustomerManagement.Api.Controllers.Common;

public class BaseController : ControllerBase
{
    private readonly IRepository<Customer> _customerRepository;

    public BaseController(IRepository<Customer> customerRepository)
    {
        _customerRepository = customerRepository;
    }

    protected async Task<IActionResult> Success(object? o)
    {
        await _customerRepository.CommitAsync();
        
        return Ok(o);
    }
    
    protected IActionResult Error(string error)
    {
        return BadRequest(error);
    }
}