using System;
using System.Collections.Generic;

namespace GoFoodBeverage.Models.Order
{
    public class OrderItemModel
    {
        public Guid Id { get; set; }

        public Guid? OrderId { get; set; }

        public bool IsCombo { get; set; }

        public Guid? ProductPriceId { get; set; }

        public decimal? Price { get; set; }

        public string ProductPriceName { get; set; }

        public decimal ProductPriceValue { get; set; }

        public int Quantity { get; set; }

        public string Notes { get; set; }

        public bool IsPromotionDiscountPercentage { get; set; }

        public decimal PromotionDiscountValue { get; set; }

        public decimal OriginalPrice { get; set; }

        public Guid? PromotionId { get; set; }

        public ProductPriceDto ProductPrice { get; set; }

        public class ProductPriceDto
        {
            public string PriceName { get; set; }

            public decimal PriceValue { get; set; }

            public ProductDto Product { get; set; }

            public class ProductDto
            {
                public string Name { get; set; }

                public Guid Id { get; set; }

                public string Description { get; set; }

                public string Thumbnail { get; set; }
            }
        }

        public IEnumerable<OrderItemOptionModel> OrderItemOptions { get; set; }

        public IEnumerable<OrderItemToppingModel> OrderItemToppings { get; set; }

        public OrderComboItemModel OrderComboItem { get; set; }

        public decimal? PriceAfterDiscount { get; set; }
    }
}
