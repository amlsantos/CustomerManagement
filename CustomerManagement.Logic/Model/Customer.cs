using CustomerManagement.Logic.Common;

namespace CustomerManagement.Logic.Model;

public class Customer : Entity
{
    public virtual Name Name { get; protected set; }
    public virtual Email PrimaryEmail { get; protected set; }
    public virtual Email SecondaryEmail { get; protected set; }
    
    public long IndustryId { get; }
    public virtual Industry Industry { get; set; }
    
    public virtual EmailSettings Settings { get; protected set; }
    public virtual CustomerStatus Status { get; protected set; }

    
    private Customer() { }

    public Customer(Name name, Email primaryEmail, Email secondaryEmail, Industry industry) : this()
    {
        if (name == null)
            throw new ArgumentNullException(nameof(name));
        if (primaryEmail == null)
            throw new ArgumentNullException(nameof(primaryEmail));
        if (secondaryEmail == null)
            throw new ArgumentNullException(nameof(secondaryEmail));
        
        Name = name;
        PrimaryEmail = primaryEmail;
        SecondaryEmail = secondaryEmail;
        Industry = industry;
        Settings = new EmailSettings(industry, false);
        Status = CustomerStatus.Regular;
    }

    public virtual void DisableEmailing()
    {
        Settings = new EmailSettings(Settings.Industry, true);
    }

    public virtual void UpdateIndustry(Industry industry)
    {
        Settings = new EmailSettings(industry, Settings.EmailingIsDisabled);
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

    public override string ToString()
    {
        return $"Id:{Id}, Name:{Name}, PrimaryEmail:{PrimaryEmail}, Industry:{Industry}";
    }
}