using System.ComponentModel;

namespace GoFoodBeverage.Domain.Enums
{
    public enum EnumOrderStatus
    {
        /// <summary>
        /// New order
        /// </summary>
        New = 0,

        /// <summary>
        /// Order returned
        /// </summary>
        Returned = 1,

        /// <summary>
        /// Order canceled
        /// </summary>
        Canceled = 2,

        /// <summary>
        /// Order confirmed
        /// </summary>
        ToConfirm = 3,

        /// <summary>
        /// Order on processing
        /// </summary>
        Processing = 4,

        /// <summary>
        /// Order on delivering
        /// </summary>
        Delivering = 5,

        /// <summary>
        /// Order completed
        /// </summary>
        Completed = 6,

        /// <summary>
        /// Draft
        /// </summary>
        Draft = 7,
    }

    public static class EnumOrderStatusExtensions
    {
        public static string GetName(this EnumOrderStatus enums) => enums switch
        {
            EnumOrderStatus.New => "New",
            EnumOrderStatus.Returned => "Returned",
            EnumOrderStatus.Canceled => "Canceled",
            EnumOrderStatus.ToConfirm => "To Confirm",
            EnumOrderStatus.Processing => "Processing",
            EnumOrderStatus.Delivering => "Delivering",
            EnumOrderStatus.Completed => "Completed",
            EnumOrderStatus.Draft => "Draft",
            _ => string.Empty
        };

        public static string GetColor(this EnumOrderStatus enums) => enums switch
        {
            EnumOrderStatus.New => "#C3BDBD",
            EnumOrderStatus.Returned => "#858585",
            EnumOrderStatus.Canceled => "#FC0D1B",
            EnumOrderStatus.ToConfirm => "#428BC1",
            EnumOrderStatus.Processing => "#FF8C21",
            EnumOrderStatus.Delivering => "#50429B",
            EnumOrderStatus.Completed => "#33B530",
            EnumOrderStatus.Draft => "#B3B3B3",
            _ => string.Empty
        };

        public static string GetBackgroundColor(this EnumOrderStatus enums) => enums switch
        {
            EnumOrderStatus.New => "rgba(195, 189, 189, 0.1)",
            EnumOrderStatus.Returned => "rgba(195, 189, 189, 0.1)",
            EnumOrderStatus.Canceled => "rgba(252, 13, 27, 0.1)",
            EnumOrderStatus.ToConfirm => "rgba(66, 139, 193, 0.1)",
            EnumOrderStatus.Processing => "rgba(255, 140, 33, 0.1)",
            EnumOrderStatus.Delivering => "rgba(80, 66, 155, 0.1)",
            EnumOrderStatus.Completed => "rgba(51, 181, 48, 0.1)",
            EnumOrderStatus.Draft => "rgba(178, 178, 178, 0.1)",
            _ => string.Empty
        };
    }
}
