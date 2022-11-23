using GoFoodBeverage.Domain.Enums;
using System;
using System.Collections.Generic;

namespace GoFoodBeverage.Models.Product
{
    public class UpdateComboRequestModel
    {
        public Guid ComboId { get; set; }

        public string ComboName { get; set; }

        public string Thumbnail { get; set; }

        public string Description { get; set; }

        public bool IsShowAllBranches { get; set; }

        public IEnumerable<Guid> BranchIds { get; set; }

        public EnumComboType ComboTypeId { get; set; }

        public EnumComboPriceType ComboPriceTypeId { get; set; }

        public decimal? SellingPrice { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public IEnumerable<Guid> ProductPriceIds { get; set; }

        public IEnumerable<ProductGroupDto> ProductGroups { get; set; }

        public class ProductGroupDto
        {
            public Guid Id { get; set; }

            public Guid ProductCategoryId { get; set; }

            public int Quantity { get; set; }

            public IEnumerable<Guid> ProductPriceIds { get; set; }
        }

        public IEnumerable<ComboPricingDto> ComboPricings { get; set; }

        public class ComboPricingDto
        {
            public Guid Id { get; set; }

            public string ComboProductName { get; set; }

            public decimal? SellingPrice { get; set; }

            public IEnumerable<ComboPricingProductDto> ComboPricingProducts { get; set; }
        }

        public class ComboPricingProductDto
        {
            public Guid Id { get; set; }

            public Guid ProductPriceId { get; set; }

            public decimal? ProductPrice { get; set; }
        }
    }
}
