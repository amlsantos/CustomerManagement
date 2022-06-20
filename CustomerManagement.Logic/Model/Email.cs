using System.Text.RegularExpressions;
using CustomerManagement.Logic.Common;
using Microsoft.EntityFrameworkCore;

namespace CustomerManagement.Logic.Model;

public class Email : ValueObject<Email>
{
    public string Value { get; }
    private Email(string value) => Value = value;
    public static Result<Email> Create(Maybe<string> value)
    {
        if (value.HasNoValue)
            return Result.Fail<Email>("Email should not be empty");
        
        var email = value.Value.Trim();
        if (string.IsNullOrEmpty(email))
            return Result.Fail<Email>("Email should not be empty");
        
        if (email.Length > 256)
            return Result.Fail<Email>("Email is too long");
        
        if (!Regex.IsMatch(email, @"^(.+)@(.+)$"))
            return Result.Fail<Email>("Email is invalid");

        return Result.Ok(new Email(email));
    }

    protected override bool EqualsCore(Email other)
    {
        return Value == other.Value;
    }

    protected override int GetHashCodeCore()
    {
        return Value.GetHashCode();
    }

    public static explicit operator Email(string value)
    {
        return Create(value).Value;
    }

    public static implicit operator string(Email customerName)
    {
        return customerName.Value;
    }
}