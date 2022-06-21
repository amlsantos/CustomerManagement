using CustomerManagement.Logic.Common;

namespace CustomerManagement.Logic.Model;

public class Customer : Entity
{
    private string _name;
    private string _primaryEmail;
    private string _secondaryEmail;

    public virtual Name Name
    {
        get => (Name)_name;
        protected init => _name = value;
    }

    public virtual Email PrimaryEmail
    {
        get => (Email)_primaryEmail;
        protected init => _primaryEmail = value;
    }

    public virtual Maybe<Email> SecondaryEmail
    {
        get => (Email)_secondaryEmail;
        protected init => _secondaryEmail = value.Unwrap();
    }
    public virtual EmailSettings EmailingSettings { get; protected set; }
    public virtual CustomerType Type { get; protected set; }

    private Customer() { }
    
    public Customer(Name name, Email primaryEmail, Maybe<Email> secondaryEmail, Industry industry) : this()
    {
        Name = name;
        PrimaryEmail = primaryEmail;
        SecondaryEmail = secondaryEmail;
        EmailingSettings = new EmailSettings(industry, false);
        Type = CustomerType.Regular;
    }

    public virtual void DisableEmailing()
    {
        EmailingSettings.DisableEmailing();
    }
    
    public virtual void UpdateIndustry(Industry industry)
    {
        EmailingSettings.UpdateIndustry(industry);
    }

    public virtual bool CanBePromoted()
    {
        return Type != CustomerType.Gold;
    }

    public virtual void Promote()
    {
        if (!CanBePromoted())
            throw new InvalidOperationException();

        Type = Type switch
        {
            CustomerType.Regular => CustomerType.Preferred,
            CustomerType.Preferred => CustomerType.Gold,
            CustomerType.Gold => throw new InvalidOperationException(),
            _ => throw new InvalidOperationException()
        };
    }
}