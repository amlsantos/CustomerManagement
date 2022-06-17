using System.ComponentModel;
using CustomerManagement.Logic.Common;
using System;

namespace CustomerManagement.Logic.Model;

public class EmailSettings : ValueObject<EmailSettings>
{
    public virtual Industry Industry { get; }
    public virtual bool EmailingIsDisabled { get; }
    public virtual EmailCampaign EmailCampaign => GetEmailCampaign(Industry);

    private EmailSettings() { }
    
    public EmailSettings(Industry industry, bool emailingIsDisabled) : this()
    {
        Industry = industry;
        EmailingIsDisabled = emailingIsDisabled;
    }
    
    protected override bool EqualsCore(EmailSettings other)
    {
        return Industry == other.Industry && EmailingIsDisabled == other.EmailingIsDisabled;
    }

    protected override int GetHashCodeCore()
    {
        unchecked
        {
            var hashCode = Industry.GetHashCode();
            hashCode = (hashCode * 397) ^ EmailingIsDisabled.GetHashCode();
            return hashCode;
        }
    }
    
    private EmailCampaign GetEmailCampaign(Industry? industry)
    {
        if (industry ==null || EmailingIsDisabled)
            return EmailCampaign.None;
        
        if (industry.Name == Industry.CarsIndustry)
            return EmailCampaign.LatestCarModels;

        if (industry.Name == Industry.PharmacyIndustry)
            return EmailCampaign.PharmacyNews;

        if (industry.Name == Industry.OtherIndustry)
            return EmailCampaign.Generic;

        throw new InvalidEnumArgumentException();
    }
}