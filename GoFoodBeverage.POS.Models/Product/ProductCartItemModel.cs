using GoFoodBeverage.POS.Models.Combo;
using System;
using System.Collections.Generic;

namespace GoFoodBeverage.POS.Models.Product
{
    public class ProductCartItemModel
    {
        public Guid? OrderItemId { get; set; }

        public Guid? ProductPriceId { get; set; }

        public Guid? ComboId { get; set; }

        public string ProductPriceName { get; set; }

        public Guid? ProductCategoryId { get; set; }

        public Guid? ProductId { get; set; }

        public decimal? ProductTax { get; set; }

        public string ProductName { get; set; }

        public string Thumbnail { get; set; }

        /// <summary>
        /// Product name and price name.
        /// Example: Milk Size S
        /// </summary>
        public string ItemName { get; set; }

        public string Notes { get; set; }

        public int Quantity { get; set; }

        /// <summary>
        /// Price per item
        /// </summary>
        public decimal OriginalPrice { get; set; }

        /// <summary>
        /// Price after discount per item
        /// </summary>
        public decimal PriceAfterDiscount { get; set; }

        /// <summary>
        /// Total price after discount per item * quantity
        /// </summary>
        public decimal TotalPriceAfterDiscount { get; set; }

        /// <summary>
        /// Total discount value for this item has included quantity
        /// </summary>
        public decimal TotalDiscount { get; set; }

        public PromotionDto Promotion { get; set; }

        public IEnumerable<ProductOptionDto> Options { get; set; }

        public IEnumerable<ToppingDto> Toppings { get; set; }

        public bool IsCombo { get; set; }

        public ComboOrderItemDto Combo { get; set; }

        public class PromotionDto
        {
            public Guid Id { get; set; }

            public int? PromotionTypeId { get; set; }

            public string Name { get; set; }

            public bool IsPercentDiscount { get; set; }

            public decimal PercentNumber { get; set; }

            public decimal DiscountValue { get; set; }

            public decimal MaximumDiscountAmount { get; set; }
        }

        public class ToppingDto
        {
            public Guid? ToppingId { get; set; }

            public Guid? PromotionId { get; set; }

            public string Name { get; set; }

            public decimal Price { get; set; }

            public decimal OriginalPrice { get; set; }

            /// <summary>
            /// Return the value of PriceAfterDiscount * quantity
            /// </summary>
            public decimal PriceAfterDiscount { get; set; }

            public int Quantity { get; set; }
        }

        /// <summary>
        /// /Remove
        /// </summary>
        public IEnumerable<ComboOrderItemDto.ComboItemDto> ComboItems { get; set; }

        public string ComboName { get; set; }

        public Guid? OrderId { get; set; }

        #region Get Promotion information before Create New POS's order
        public Guid? PromotionId { get; set; }
        public string PromotionName { get; set; }
        public decimal PromotionValue { get; set; }
        public bool IsPercentDiscount { get; set; }
        #endregion

        public bool IsProductFromOrder { get; set; } = false;

        /// <summary>
        /// Back end update order item status
        /// </summary>
        public bool IsCanceled { get; set; }
    }
}
