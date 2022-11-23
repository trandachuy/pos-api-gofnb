namespace GoFoodBeverage.Payment.VNPay.Enums
{
    public enum EnumVNPayTransactionType
    {
        FullRefund,

        PartiallyRefund
    }

    public static class EnumVNPayTransactionTypeExtension
    {
        public static string GetCode(this EnumVNPayTransactionType enums) => enums switch
        {
            EnumVNPayTransactionType.FullRefund => "02",
            EnumVNPayTransactionType.PartiallyRefund => "03",
            _ => string.Empty
        };
    }
}
