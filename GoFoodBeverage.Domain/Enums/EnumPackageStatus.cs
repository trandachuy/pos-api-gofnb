namespace GoFoodBeverage.Domain.Enums
{
    public enum EnumPackageStatus
    {
        Inactive = 0,
        Active = 1
    }

    public static class EnumPackageStatusExtensions
    {
        public static string GetName(this EnumPackageStatus enums) => enums switch
        {
            EnumPackageStatus.Inactive => "packageStatus.inactive",
            EnumPackageStatus.Active => "packageStatus.active",
            _ => string.Empty
        };
    }
}
