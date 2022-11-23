using System.ComponentModel;

namespace GoFoodBeverage.Domain.Enums
{
    public enum EnumTransactionType
    {
        [Description("Payment")]
        Payment,

        [Description("Refund")]
        Refund,
    }
}
