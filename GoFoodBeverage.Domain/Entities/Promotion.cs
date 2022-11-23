using System;
using GoFoodBeverage.Domain.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(Promotion))]
    public class Promotion : BaseEntity
    {
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

        public virtual ICollection<PromotionProduct> PromotionProducts { get; set; }

        public virtual ICollection<PromotionProductCategory> PromotionProductCategories { get; set; }

        public virtual ICollection<PromotionBranch> PromotionBranches { get; set; }
    }
}
