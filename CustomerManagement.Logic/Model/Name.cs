using CustomerManagement.Logic.Common;
using Microsoft.EntityFrameworkCore;

namespace CustomerManagement.Logic.Model;

public class Name : ValueObject<Name>
{
    public string Value { get; }
    private Name(string value) => Value = value;
    public static Result<Name> Create(string value)
    {
        value = value.Trim();
        
        if (string.IsNullOrEmpty(value))
            return Result.Fail<Name>("Customer name should not be empty");
        
        if (string.IsNullOrWhiteSpace(value))
            return Result.Fail<Name>("Customer name should not be empty");

        if (value.Length > 200)
            return Result.Fail<Name>("Customer name is too long");
        
        return Result.Ok(new Name(value));
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