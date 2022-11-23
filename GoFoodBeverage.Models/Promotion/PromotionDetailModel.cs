using System;
using System.Collections.Generic;

namespace GoFoodBeverage.Models.Promotion
{
    public class PromotionDetailModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int PromotionTypeId { get; set; }

        public bool IsPercentDiscount { get; set; }

        public float PercentNumber { get; set; }

        public decimal MaximumDiscountAmount { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string TermsAndCondition { get; set; }

        public bool? IsMinimumPurchaseAmount { get; set; }

        public decimal? MinimumPurchaseAmount { get; set; }

        public bool IsSpecificBranch { get; set; }

        public bool IsIncludedTopping { get; set; }

        public IEnumerable<ProductDto> Products { get; set; }

        public IEnumerable<ProductCategoryDto> ProductCategories { get; set; }

        public IEnumerable<StoreBranchDto> Branches { get; set; }

        public class ProductDto
        {
            public Guid Id { get; set; }

            public string Name { get; set; }
        }

        public class ProductCategoryDto
        {
            public Guid Id { get; set; }

            public string Name { get; set; }
        }

        public class StoreBranchDto
        {
            public Guid Id { get; set; }

            public string Name { get; set; }
        }
    }
}
