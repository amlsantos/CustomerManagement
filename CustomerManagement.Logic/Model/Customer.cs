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
    public virtual CustomerStatus Status { get; protected set; }

    private Customer() { }
    
    public Customer(Name name, Email primaryEmail, Maybe<Email> secondaryEmail, Industry industry) : this()
    {
        Name = name;
        PrimaryEmail = primaryEmail;
        SecondaryEmail = secondaryEmail;
        EmailingSettings = new EmailSettings(industry, true);
        Status = CustomerStatus.Regular;
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
        return Status != CustomerStatus.Gold;
    }

    public virtual void Promote()
    {
        if (!CanBePromoted())
            throw new InvalidOperationException();

        Status = Status switch
        {
            CustomerStatus.Regular => CustomerStatus.Preferred,
            CustomerStatus.Preferred => CustomerStatus.Gold,
            CustomerStatus.Gold => throw new InvalidOperationException(),
            _ => throw new InvalidOperationException()
        };
    }
}