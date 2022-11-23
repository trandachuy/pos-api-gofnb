using System;
using System.Collections.Generic;
using GoFoodBeverage.Models.Option;

namespace GoFoodBeverage.Models.Product
{
    public class ProductDetailAppOrderModel
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
            public Guid OptionId { get; set; }

            public OptionModel Option { get; set; }
        }

        public List<ProductToppingModel> ProductToppings { get; set; }
    }
}
