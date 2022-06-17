using CustomerManagement.Logic.Common;

namespace CustomerManagement.Logic.Model;

public class Industry : Entity
{
    public const string CarsIndustry = "Cars";
    public const string PharmacyIndustry = "Pharmacy";
    public const string OtherIndustry = "Other";

    public Industry(long id, string name)
    {
        Id = id;
        Name = name;
    }

    public virtual string Name { get; set; }
}