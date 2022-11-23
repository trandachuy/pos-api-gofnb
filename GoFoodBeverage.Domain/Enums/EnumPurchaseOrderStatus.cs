using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Domain.Enums
{
    public enum EnumPurchaseOrderStatus
    {
        /// <summary>
        /// Canceled
        /// </summary>
        Canceled = 0,

        /// <summary>
        /// Draft
        /// </summary>
        Draft = 1,

        /// <summary>
        /// Approved
        /// </summary>
        Approved = 2,

        /// <summary>
        /// Completed
        /// </summary>
        Completed = 3,
    }

    public enum EnumPurchaseOrderAction
    {
        /// <summary>
        /// Approve
        /// </summary>
        Approve = 0,

        /// <summary>
        /// Cancel
        /// </summary>
        Cancel = 1,

        /// <summary>
        /// Complete
        /// </summary>
        Complete = 2
    }

    public static class EnumPurchaseOrderStatusExtensions
    {
        public static string GetStatusName(this EnumPurchaseOrderStatus enums) => enums switch
        {
            EnumPurchaseOrderStatus.Draft => "status.draft",
            EnumPurchaseOrderStatus.Approved => "status.inProgress",
            EnumPurchaseOrderStatus.Completed => "status.complete",
            EnumPurchaseOrderStatus.Canceled => "status.cancel",
            _ => "Error"
        };

        public static string GetColor(this EnumPurchaseOrderStatus enums) => enums switch
        {
            EnumPurchaseOrderStatus.Draft => "#9F9F9F",
            EnumPurchaseOrderStatus.Approved => "#FF8C21",
            EnumPurchaseOrderStatus.Completed => "#33B530",
            EnumPurchaseOrderStatus.Canceled => "#FC0D1B",
            _ => "#1A132F"
        };

        public static string GetBackGroundColor(this EnumPurchaseOrderStatus enums) => enums switch
        {
            EnumPurchaseOrderStatus.Draft => "rgba(159, 159, 159, 0.1)",
            EnumPurchaseOrderStatus.Approved => "rgba(255, 140, 33, 0.1)",
            EnumPurchaseOrderStatus.Completed => "#rgba(51, 181, 48, 0.1)",
            EnumPurchaseOrderStatus.Canceled => "rgba(252, 13, 27, 0.1)",
            _ => "#1A132F"
        };
    }
}
