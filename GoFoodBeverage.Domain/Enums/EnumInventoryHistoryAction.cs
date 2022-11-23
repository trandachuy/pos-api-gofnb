namespace GoFoodBeverage.Domain.Enums
{
    public enum EnumInventoryHistoryAction
    {
        /// <summary>
        /// Create Order
        /// </summary>
        CreateOrder = 0,

        /// <summary>
        /// Edit Order
        /// </summary>
        EditOrder = 1,

        /// <summary>
        /// Cancel Order
        /// </summary>
        CancelOrder = 2,

        /// <summary>
        /// Update Stock
        /// </summary>
        UpdateStock = 3,

        /// <summary>
        /// Import Goods
        /// </summary>
        ImportGoods = 4,

        /// <summary>
        /// Transfer Goods
        /// </summary>
        TransferGoods = 5,

        /// <summary>
        /// Start Shift
        /// </summary>
        StartShift = 6,

        /// <summary>
        /// End Shift
        /// </summary>
        EndShift = 7
    }

    public static class EnumActionInventoryHistoryExtensions
    {
        public static string GetName(this EnumInventoryHistoryAction enums) => enums switch
        {
            EnumInventoryHistoryAction.CreateOrder => "Create order",
            EnumInventoryHistoryAction.EditOrder => "Edit order",
            EnumInventoryHistoryAction.CancelOrder => "Cancel order",
            EnumInventoryHistoryAction.UpdateStock => "Update stock",
            EnumInventoryHistoryAction.ImportGoods => "Import goods",
            EnumInventoryHistoryAction.TransferGoods => "Transfer goods",
            EnumInventoryHistoryAction.StartShift => "Start shift",
            EnumInventoryHistoryAction.EndShift => "End shift",
            _ => string.Empty
        };

        public static string GetColor(this EnumInventoryHistoryAction enums) => enums switch
        {
            EnumInventoryHistoryAction.CreateOrder => "#50429B",
            EnumInventoryHistoryAction.EditOrder => "#FF8C21",
            EnumInventoryHistoryAction.CancelOrder => "#FC0D1B",
            EnumInventoryHistoryAction.UpdateStock => "#428BC1",
            EnumInventoryHistoryAction.ImportGoods => "#817CBA",
            EnumInventoryHistoryAction.TransferGoods => "#F96E43",
            EnumInventoryHistoryAction.StartShift => "#2B2162",
            EnumInventoryHistoryAction.EndShift => "#858585",
            _ => string.Empty
        };

        public static string GetBackgroundColor(this EnumInventoryHistoryAction enums) => enums switch
        {
            EnumInventoryHistoryAction.CreateOrder => "rgba(80, 66, 155, 0.1)",
            EnumInventoryHistoryAction.EditOrder => "rgba(255, 140, 33, 0.1)",
            EnumInventoryHistoryAction.CancelOrder => "rgba(252, 13, 27, 0.1)",
            EnumInventoryHistoryAction.UpdateStock => "rgba(66, 139, 193, 0.1)",
            EnumInventoryHistoryAction.ImportGoods => "rgba(129, 124, 186, 0.1)",
            EnumInventoryHistoryAction.TransferGoods => "rgba(249, 110, 67, 0.1)",
            EnumInventoryHistoryAction.StartShift => "rgba(43, 33, 98, 0.1)",
            EnumInventoryHistoryAction.EndShift => "rgba(133, 133, 133, 0.1)",
            _ => string.Empty
        };

        public static string GetNote(this EnumInventoryHistoryAction enums) => enums switch
        {
            EnumInventoryHistoryAction.CreateOrder => "[System] Create order",
            EnumInventoryHistoryAction.EditOrder => "[System] Edit order",
            EnumInventoryHistoryAction.CancelOrder => "[System] Cancel order",
            EnumInventoryHistoryAction.UpdateStock => "[System] Update material quantity",
            EnumInventoryHistoryAction.ImportGoods => "[System] Import goods",
            EnumInventoryHistoryAction.TransferGoods => "[System] Transfer goods",
            EnumInventoryHistoryAction.StartShift => "-",
            EnumInventoryHistoryAction.EndShift => "-",
            _ => string.Empty
        };
    }
}
