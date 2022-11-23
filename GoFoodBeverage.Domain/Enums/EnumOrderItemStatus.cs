using System.ComponentModel;

namespace GoFoodBeverage.Domain.Enums
{
    public enum EnumOrderItemStatus
    {
        /// <summary>
        /// OrderItem new
        /// </summary>
        New = 1,

        /// <summary>
        /// OrderItem completed
        /// </summary>
        Completed = 2,

        /// <summary>
        /// OrderItem canceled
        /// </summary>
        Canceled = 3,
    }

    public static class EnumOrderItemStatusExtensions
    {
        public static string GetName(this EnumOrderItemStatus enums) => enums switch
        {
            EnumOrderItemStatus.Canceled => "Canceled",
            EnumOrderItemStatus.New => "New",
            EnumOrderItemStatus.Completed => "Completed",
            _ => string.Empty
        };
    }
}
