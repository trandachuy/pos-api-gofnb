using System;
using GoFoodBeverage.Domain.Enums;

namespace GoFoodBeverage.Models.Staff
{
    public class StaffActivityModel
    {
        public Guid StaffId { get; set; }

        public Guid StoreId { get; set; }

        public EnumActionGroup ActionGroup { get; set; }

        public EnumActionType ActionType { get; set; }

        public DateTime ExecutedTime { get; set; }

        public Guid ObjectId { get; set; }

        public string ObjectName { get; set; }

        public string ObjectThumbnail { get; set; }

        public string StaffName { get; set; }

        public string ActionGroupDescribe { get; set; }

        public string ActionTypeDescribe { get; set; }
    }
}
