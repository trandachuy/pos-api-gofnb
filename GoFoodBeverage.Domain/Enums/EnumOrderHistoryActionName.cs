using System.ComponentModel;

namespace GoFoodBeverage.Domain.Enums
{
    public enum EnumOrderHistoryActionName
    {
        /// <summary>
        /// Add new order
        /// </summary>
        New = 0,

        /// <summary>
        ///Edit order
        /// </summary>
        Edit = 1,

        /// <summary>
        /// Cancel order
        /// </summary>
        Cancel = 2,

        /// <summary>
        ///  Update other statuses of order
        /// </summary>
        UpdateOrderStatus = 3,

        /// <summary>
        /// Update Payment status of order
        /// </summary>
        UpdatePaymentStatus = 4,

        /// <summary>
        /// Update order Ahamove status
        /// </summary>
        UpdateOrderAhamoveStatus = 5,
    }

    public static class EnumOrderHistoryActionNameExtensions
    {
        public static string GetName(this EnumOrderHistoryActionName enums) => enums switch
        {
            EnumOrderHistoryActionName.New => "Order is created",
            EnumOrderHistoryActionName.Edit => "Order is edited successful",
            EnumOrderHistoryActionName.Cancel => "Order is canceled",
            EnumOrderHistoryActionName.UpdatePaymentStatus => "Paid successful by {0}",
            EnumOrderHistoryActionName.UpdateOrderAhamoveStatus => "Update order status from Ahamove",
            _ => string.Empty
        };

        public static string GetNote(this EnumOrderHistoryActionName enums) => enums switch
        {
            EnumOrderHistoryActionName.New => "Order was created successfully",
            EnumOrderHistoryActionName.UpdateOrderAhamoveStatus => "Order ahamove is updated successfully",
            _ => string.Empty
        };
    }
}
