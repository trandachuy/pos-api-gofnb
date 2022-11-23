using System;
using System.Collections.Generic;

namespace GoFoodBeverage.Models.Promotion
{
    public class CreateNewPromotionModel
    {
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

        public List<Guid> ProductIds { get; set; }

        public List<Guid> ProductCategoryIds { get; set; }

        public List<Guid> BranchIds { get; set; }

    }
}
