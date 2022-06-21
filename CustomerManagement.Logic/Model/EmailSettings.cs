using System.ComponentModel;
using CustomerManagement.Logic.Common;

namespace CustomerManagement.Logic.Model;

public class EmailSettings : Entity
{
    public virtual bool IsDisabled { get; protected set; }
    public virtual Industry Industry { get; protected set; }
    public virtual EmailType EmailType => GetEmailCampaign(Industry);

    private EmailSettings() { }
    public EmailSettings(Industry industry, bool isDisabled) : this()
    {
        Industry = industry;
        IsDisabled = isDisabled;
    }

    private EmailType GetEmailCampaign(Industry industry)
    {
        if (IsDisabled)
            return EmailType.None;
        
        if (industry == Industry.Cars)
            return EmailType.LatestCarModels;

        if (industry == Industry.Pharmacy)
            return EmailType.PharmacyNews;

        if (industry == Industry.Other)
            return EmailType.Generic;

        throw new InvalidEnumArgumentException();
    }
    
    public virtual void DisableEmailing()
    {
        IsDisabled = true;
    }

    public virtual void UpdateIndustry(Industry industry)
    {
        Industry = industry;
    }
}