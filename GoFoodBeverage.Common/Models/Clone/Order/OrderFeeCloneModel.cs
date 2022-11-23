using GoFoodBeverage.Common.Models.Base;
using System;

namespace GoFoodBeverage.Common.Models.Clone.Order
{
    public class OrderFeeCloneModel : BaseAuditModel
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public Guid FeeId { get; set; }

        public bool IsPercentage { get; set; }

        public decimal FeeValue { get; set; }

        public string FeeName { get; set; }
    }
}
