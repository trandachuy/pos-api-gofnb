
namespace GoFoodBeverage.Domain.Enums
{
    public enum EnumPackagePaymentMethod
    {
        /// <summary>
        /// Visa
        /// </summary>
        Visa = 0,

        /// <summary>
        /// ATM
        /// </summary>
        ATM = 1,

        /// <summary>
        /// Bank Transfer
        /// </summary>
        BankTransfer = 2,
    }

    public static class PackagePaymentExtensions
    {
        public static string GetName(this EnumPackagePaymentMethod enums) => enums switch
        {
            EnumPackagePaymentMethod.Visa => "Visa",
            EnumPackagePaymentMethod.ATM => "ATM",
            EnumPackagePaymentMethod.BankTransfer => "Bank Transfer",
            _ => string.Empty
        };

        public static string GetBankCode(this EnumPackagePaymentMethod _enum) => _enum switch {
            EnumPackagePaymentMethod.Visa => "INTCARD",
            EnumPackagePaymentMethod.ATM => "VNBANK",
            _ => string.Empty
        };
    }
}

