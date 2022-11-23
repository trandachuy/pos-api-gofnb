using System;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Models.Option;
using GoFoodBeverage.Models.Product;

namespace GoFoodBeverage.Models.Combo
{
    public class ComboActivatedModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsShowAllBranches { get; set; }

        public string Thumbnail { get; set; }

        public EnumComboType ComboTypeId { get; set; }

        public EnumComboPriceType ComboPriceTypeId { get; set; }

        public decimal? SellingPrice { get; set; }

        public List<ComboProductPriceModel> ComboProductPrices { get; set; }

        public class ComboProductPriceModel
        {
            public Guid ProductPriceId { get; set; }

            public ProductPriceModel ProductPrice { get; set; }

            public decimal PriceValue { get; set; }

            public string PriceName { get; set; }

            public class ProductPriceModel
            {
                public Guid ProductId { get; set; }

                public ProductModel Product { get; set; }
            }
        }

        public List<ComboPricingModel> ComboPricings { get; set; }

        public class ComboPricingModel
        {
            public Guid Id { get; set; }

            public Guid ComboId { get; set; }

            public string ComboName { get; set; }

            public decimal OriginalPrice { get; set; }

            public decimal SellingPrice { get; set; }

            public List<ComboPricingProductPriceModel> ComboPricingProducts { get; set; }

            public class ComboPricingProductPriceModel
            {
                public Guid Id { get; set; }

                public Guid ComboPricingId { get; set; }

                public Guid ProductPriceId { get; set; }

                public decimal SellingPrice { get; set; }

                public ProductPriceModel ProductPrice { get; set; }

                public class ProductPriceModel
                {
                    public Guid ProductId { get; set; }

                    public ProductModel Product { get; set; }

                    public string PriceName { get; set; }

                    public decimal PriceValue { get; set; }
                }
            }
        }

        public class ProductModel
        {
            public Guid ProductId { get; set; }

            public string Name { get; set; }

            public string Thumbnail { get; set; }

            public List<ProductOptionDto> ProductOptions { get; set; }

            public class ProductOptionDto
            {
                public Guid ProductId { get; set; }

                public Guid OptionId { get; set; }

                public OptionModel Option { get; set; }
            }

            public List<ProductToppingModel> ProductToppings { get; set; }
        }
    }
}
