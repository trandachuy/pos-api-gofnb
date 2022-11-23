using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.POS.Models.Product;
using System;
using System.Collections.Generic;

namespace GoFoodBeverage.POS.Models.Combo
{
    public class ComboDetailPosOrderDto
    {
        public Guid Id { get; set; }

        public Guid ComboId { get; set; }

        public string ComboName { get; set; }

        public decimal OriginalPrice { get; set; }

        public Guid ComboPricingId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsShowAllBranches { get; set; }

        public string Thumbnail { get; set; }

        public EnumComboType ComboTypeId { get; set; }

        public EnumComboPriceType ComboPriceTypeId { get; set; }

        public decimal? SellingPrice { get; set; }

        public List<ComboProductPriceDto> ComboProductPrices { get; set; }

        public List<ComboProductGroupDto> ComboProductGroups { get; set; }

        public List<ComboPricingDto> ComboPricings { get; set; }

        public class ComboProductPriceDto
        {
            public Guid ProductPriceId { get; set; }

            public ProductPriceDto ProductPrice { get; set; }

            public decimal PriceValue { get; set; }

            public string PriceName { get; set; }
        }

        public class ComboProductGroupDto
        {
            public Guid Id { get; set; }

            public Guid ProductCategoryId { get; set; }

            public string ProductCategoryName { get; set; }

            public int Quantity { get; set; }

            public List<ComboProductGroupProductPriceDto> ComboProductGroupProductPrices { get; set; }
        }

        public class ComboPricingDto
        {
            public Guid Id { get; set; }

            public Guid ComboId { get; set; }

            public string ComboName { get; set; }

            public decimal OriginalPrice { get; set; }

            public decimal SellingPrice { get; set; }

            public List<ComboPricingProductPriceDto> ComboPricingProducts { get; set; }
        }

        public class ComboProductGroupProductPriceDto
        {
            public Guid ProductPriceId { get; set; }

            public ProductPriceDto ProductPrice { get; set; }
        }

        public class ComboPricingProductPriceDto
        {
            public Guid Id { get; set; }

            public Guid ComboPricingId { get; set; }

            public Guid ProductPriceId { get; set; }

            public decimal SellingPrice { get; set; }

            public ProductPriceDto ProductPrice { get; set; }
        }

        public class ProductDto
        {
            public Guid ProductId { get; set; }

            public string Name { get; set; }

            public string Thumbnail { get; set; }

            public List<ProductOptionDto> ProductOptions { get; set; }
        }

        public class ProductPriceDto
        {
            public Guid ProductId { get; set; }

            public string PriceName { get; set; }

            public decimal PriceValue { get; set; }

            public ProductDto Product { get; set; }
        }
    }
}
