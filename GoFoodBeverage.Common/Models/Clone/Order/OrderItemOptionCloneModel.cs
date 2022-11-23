using GoFoodBeverage.Common.Models.Base;
using System;

namespace GoFoodBeverage.Common.Models.Clone.Order
{
    public class OrderItemOptionCloneModel: BaseAuditModel
    {
        public Guid Id { get; set; }

        public Guid? OptionId { get; set; }

        public Guid? OptionLevelId { get; set; }

        public Guid OrderItemId { get; set; }

        public string OptionName { get; set; }

        public string OptionLevelName { get; set; }
    }
}
