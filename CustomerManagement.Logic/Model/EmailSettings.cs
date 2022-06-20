using System.ComponentModel;
using CustomerManagement.Logic.Common;

namespace CustomerManagement.Logic.Model;

public class EmailSettings : Entity
{
    public virtual bool IsDisabled { get; protected set; }
    public virtual Industry Industry { get; protected set; }
    public virtual EmailCampaign EmailCampaign => GetEmailCampaign(Industry);

    private EmailSettings() { }
    public EmailSettings(Industry industry, bool isDisabled) : this()
    {
        Industry = industry;
        IsDisabled = isDisabled;
    }

    private EmailCampaign GetEmailCampaign(Industry industry)
    {
        if (IsDisabled)
            return EmailCampaign.None;
        
        if (industry == Industry.Cars)
            return EmailCampaign.LatestCarModels;

        if (industry == Industry.Pharmacy)
            return EmailCampaign.PharmacyNews;

        if (industry == Industry.Other)
            return EmailCampaign.Generic;

        throw new InvalidEnumArgumentException();
    }
    
    public virtual void DisableEmailing()
    {
        IsDisabled = false;
    }

    public virtual void UpdateIndustry(Industry industry)
    {
        Industry = industry;
    }
}