using GoFoodBeverage.Models.Option;
using System;
using System.Collections.Generic;

namespace GoFoodBeverage.Models.Product
{
    public class ProductCategoryActivatedModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<ProductProductCategoryModel> ProductProductCategories { get; set; }

        public class ProductProductCategoryModel
        {
            public Guid ProductId { get; set; }

            public Guid ProductCategoryId { get; set; }

            public ProductModel Product { get; set; }

            public class ProductModel
            {
                public string Name { get; set; }

                public string Description { get; set; }

                public string Thumbnail { get; set; }

                public List<ProductPriceDto> ProductPrices { get; set; }

                public class ProductPriceDto
                {
                    public Guid Id { get; set; }

                    public string PriceName { get; set; }

                    public decimal PriceValue { get; set; }
                }

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
}
