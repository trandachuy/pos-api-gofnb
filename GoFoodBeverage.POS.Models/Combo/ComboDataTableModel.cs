using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.POS.Models.Product;
using System;
using System.Collections.Generic;

namespace GoFoodBeverage.POS.Models.Combo
{
    public class ComboDataTableModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsShowAllBranches { get; set; }

        public string Thumbnail { get; set; }

        public EnumComboType ComboTypeId { get; set; }

        public EnumComboPriceType ComboPriceTypeId { get; set; }

        public decimal? SellingPrice { get; set; }

        public List<StoreBranch> Branch { get; set; }

        public class StoreBranch
        {
            public Guid Id { get; set; }

            public string Name { get; set; }
        }

        public List<ComboStoreBranchModel> ComboStoreBranches { get; set; }

        public class ComboStoreBranchModel
        {
            public Guid BranchId { get; set; }

            public StoreBranch Branch { get; set; }

            public class StoreBranch
            {
                public Guid Id { get; set; }

                public string Name { get; set; }
            }
        }

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

        public List<ComboProductGroupModel> ComboProductGroups { get; set; }

        public class ComboProductGroupModel
        {
            public Guid Id { get; set; }

            public Guid ProductCategoryId { get; set; }

            public string ProductCategoryName { get; set; }

            public int Quantity { get; set; }

            public List<ComboProductGroupProductPriceModel> ComboProductGroupProductPrices { get; set; }

            public class ComboProductGroupProductPriceModel
            {
                public Guid ProductPriceId { get; set; }

                public ProductPriceModel ProductPrice { get; set; }

                public class ProductPriceModel
                {
                    public Guid ProductId { get; set; }

                    public string PriceName { get; set; }

                    public decimal PriceValue { get; set; }

                    public ProductModel Product { get; set; }

                    public class ProductModel
                    {
                        public Guid Id { get; set; }

                        public string Name { get; set; }

                        public string Thumbnail { get; set; }

                        public List<ProductOptionDto> ProductOptions { get; set; }
                    }
                }
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
        }

        public Guid ComboId { get; set; }

        public string ComboName { get; set; }

        public decimal OriginalPrice { get; set; }

        public Guid ComboPricingId { get; set; }
    }
}
