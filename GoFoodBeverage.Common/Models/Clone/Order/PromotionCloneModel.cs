using GoFoodBeverage.Common.Models.Base;
using System;

namespace GoFoodBeverage.Common.Models.Clone
{
    public class PromotionCloneModel : BaseAuditModel
    {
        public Guid Id { get; set; }

        public Guid StoreId { get; set; }

        public string Name { get; set; }

        public int PromotionTypeId { get; set; }

        public bool IsPercentDiscount { get; set; }

        public decimal PercentNumber { get; set; }

        public decimal MaximumDiscountAmount { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string TermsAndCondition { get; set; }

        public bool? IsMinimumPurchaseAmount { get; set; }

        public decimal? MinimumPurchaseAmount { get; set; }

        public bool IsSpecificBranch { get; set; }

        public bool IsIncludedTopping { get; set; }

        public bool? IsStopped { get; set; }
    }
}
