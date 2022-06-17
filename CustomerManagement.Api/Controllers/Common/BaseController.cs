using CustomerManagement.Api.DAL;
using CustomerManagement.Logic.Model;
using Microsoft.AspNetCore.Mvc;

namespace CustomerManagement.Api.Controllers.Common;

public class BaseController : ControllerBase
{
    private readonly IRepository<Customer> _customerRepository;
    private readonly IRepository<Industry> _industryRepository;

    public BaseController(IRepository<Customer> customerRepository, IRepository<Industry> industryRepository)
    {
        _customerRepository = customerRepository;
        _industryRepository = industryRepository;
    }

    protected async Task<IActionResult> Success(object? o)
    {
        await _customerRepository.CommitAsync();
        await _industryRepository.CommitAsync();
        
        return Ok(o);
    }
    
    protected IActionResult Error(string error)
    {
        return BadRequest(error);
    }
}