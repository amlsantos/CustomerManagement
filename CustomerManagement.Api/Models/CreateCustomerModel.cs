namespace CustomerManagement.Api.Models;

public class CreateCustomerModel
{
    public string? Name { get; init; }
    public string? PrimaryEmail { get; init; }
    public string? SecondaryEmail { get; init; }
    public string? Industry { get; init; }
}