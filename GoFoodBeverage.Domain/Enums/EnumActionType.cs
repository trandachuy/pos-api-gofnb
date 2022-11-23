using System.ComponentModel;

namespace GoFoodBeverage.Domain.Enums
{
    public enum EnumActionType
    {
        [Description("actionType.stopped")]
        Stopped = -4,

        [Description("actionType.inActivated")]
        Inactivated = -3,

        [Description("actionType.cancelled")]
        Cancelled = -2,

        [Description("actionType.deleted")]
        Deleted = -1,

        [Description("actionType.created")]
        Created = 0,

        [Description("actionType.edited")]
        Edited = 1,

        [Description("actionType.updateStatus")]
        UpdateStatus = 2,

        [Description("actionType.approved")]
        Approved = 3,

        [Description("actionType.completed")]
        Completed = 4,

        [Description("actionType.activated")]
        Activated = 5,

        [Description("actionType.paymentStatus")]
        PaymentStatus = 6,
    }
}
