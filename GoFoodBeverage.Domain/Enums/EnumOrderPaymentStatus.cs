
namespace GoFoodBeverage.Domain.Enums
{
    public enum EnumOrderPaymentStatus
    {
        /// <summary>
        /// The order payment status is Unpaid
        /// </summary>
        Unpaid,

        /// <summary>
        /// The order payment status is Paid
        /// </summary>
        Paid,

        /// <summary>
        /// The order payment status is Refunded
        /// </summary>
        Refunded,
    }

    public static class EnumOrderPaymentStatusExtensions
    {
        public static string GetName(this EnumOrderPaymentStatus enums) => enums switch
        {
            EnumOrderPaymentStatus.Unpaid => "Unpaid",
            EnumOrderPaymentStatus.Paid => "Paid",
            EnumOrderPaymentStatus.Refunded => "Refunded",
            _ => string.Empty
        };
    }
}
