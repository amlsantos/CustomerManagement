using CustomerManagement.Logic.Common;

namespace CustomerManagement.Logic.Model;

public class Customer : Entity
{
    public virtual string Name { get; protected set; }
    public virtual string PrimaryEmail { get; protected set; }
    public virtual string SecondaryEmail { get; protected set; }
    public virtual Industry Industry { get; set; }
    public virtual EmailCampaignType EmailCampaignType { get; protected set; }
    public virtual CustomerStatus Status { get; protected set; }
    
    protected Customer() { }

    public Customer(string name, string primaryEmail, string secondaryEmail, Industry industry) : this()
    {
        Name = name;
        PrimaryEmail = primaryEmail;
        SecondaryEmail = secondaryEmail;
        Industry = industry;
        EmailCampaignType = GetEmailCampaign(industry);
        Status = CustomerStatus.Regular;
    }

    private EmailCampaignType GetEmailCampaign(Industry industry)
    {
        if (industry.Name == Industry.CarsIndustry)
            return EmailCampaignType.LatestCarModels;

        if (industry.Name == Industry.PharmacyIndustry)
            return EmailCampaignType.PharmacyNews;

        return EmailCampaignType.Generic;
    }

    public virtual void DisableEmailing()
    {
        EmailCampaignType = EmailCampaignType.None;
    }

    public virtual void UpdateIndustry(Industry industry)
    {
        if (EmailCampaignType == EmailCampaignType.None)
            return;

        EmailCampaignType = GetEmailCampaign(industry);
        Industry = industry;
    }

    public virtual bool CanBePromoted()
    {
        return Status != CustomerStatus.Gold;
    }

    public virtual void Promote()
    {
        if (Status == CustomerStatus.Regular)
        {
            Status = CustomerStatus.Preferred;
        }
        else
        {
            Status = CustomerStatus.Gold;
        }
    }

    public override string ToString()
    {
        return $"Id:{Id}, Name:{Name}, PrimaryEmail:{PrimaryEmail}, Industry:{Industry}";
    }
}