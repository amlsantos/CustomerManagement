using CustomerManagement.Logic.Common;
using Microsoft.EntityFrameworkCore;

namespace CustomerManagement.Logic.Model;

public class Name : ValueObject<Name>
{
    public string Value { get; }
    private Name(string value) => Value = value;
    public static Result<Name> Create(Maybe<string> name)
    {
        if (name.HasNoValue)
            return Result.Fail<Name>("Customer name should not be empty");
        
        var customerName = name.Value.Trim();
        if (string.IsNullOrEmpty(customerName))
            return Result.Fail<Name>("Customer name should not be empty");
        
        if (string.IsNullOrWhiteSpace(customerName))
            return Result.Fail<Name>("Customer name should not be empty");

        if (customerName.Length > 200)
            return Result.Fail<Name>("Customer name is too long");
        
        return Result.Ok(new Name(customerName));
    }

    protected override bool EqualsCore(Name other)
    {
        return Value == other.Value;
    }

    protected override int GetHashCodeCore()
    {
        return Value.GetHashCode();
    }

    public static explicit operator Name(string value)
    {
        return Create(value).Value;
    }

    public static implicit operator string(Name name)
    {
        return name.Value;
    }
}