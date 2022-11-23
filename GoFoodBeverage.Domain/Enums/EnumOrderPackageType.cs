using System.ComponentModel;

namespace GoFoodBeverage.Domain.Enums
{
    public enum EnumOrderPackageType
    {
        [Description("STORE ACTIVATE")]
        StoreActivate = 0,

        [Description("BRANCH PURCHASE")]
        BranchPurchase = 1
    }
}
