using GoFoodBeverage.POS.Models.Product;
using System;
using System.Collections.Generic;

namespace GoFoodBeverage.POS.Models.Combo
{
    public class ComboOrderItemDto
    {
        public Guid? ComboId { get; set; }

        public Guid? ComboPricingId { get; set; }

        public string ComboName { get; set; }

        public string ItemName { get; set; }

        public string Thumbnail { get; set; }

        public decimal OriginalPrice { get; set; }

        public decimal SellingPrice { get; set; }

        public decimal SellingPriceAfterDiscount { get; set; }

        /// <summary>
        /// The quantity of combo
        /// Notes: Please do not use this field to calculate the price, after split cart item => should be use the quantity from cart item
        /// </summary>
        public int Quantity { get; set; }

        public string Notes { get; set; }

        public List<ComboItemDto> ComboItems { get; set; }

        public class ComboItemDto
        {
            public Guid? ProductPriceId { get; set; }

            public string ItemName { get; set; }

            public string Thumbnail { get; set; }

            public int Quantity { get; set; }

            public string Note { get; set; }

            public List<ProductOptionDto> Options { get; set; } = new List<ProductOptionDto>();

            public List<ProductToppingDto> Toppings { get; set; } = new List<ProductToppingDto>();

            public Guid ProductId { get; set; }
        }

        public class ProductToppingDto
        {
            public Guid ToppingId { get; set; }

            public string Name { get; set; }

            public decimal OriginalPrice { get; set; }

            public decimal PriceAfterDiscount { get; set; }

            public int Quantity { get; set; }
        }
    }
}
