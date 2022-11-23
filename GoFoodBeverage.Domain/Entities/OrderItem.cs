using System;
using GoFoodBeverage.Domain.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Enums;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(OrderItem))]
    public class OrderItem : BaseEntity
    {
        public Guid? OrderId { get; set; }

        public Guid? OrderSessionId { get; set; }

        public Guid? ProductPriceId { get; set; }

        /// <summary>
        /// This name compiled from the product name and price name
        /// </summary>
        public string ProductPriceName { get; set; }

        /// <summary>
        /// Original price of product price
        /// </summary>
        public decimal OriginalPrice { get; set; }

        /// <summary>
        /// Price of item after discount
        /// </summary>
        public decimal? PriceAfterDiscount { get; set; }

        public int Quantity { get; set; }

        public decimal TotalPriceAfterDiscount { get { return PriceAfterDiscount.Value * Quantity; } }

        public string ItemName { get { return $"{ProductName} {ProductPriceName}"; } }

        public string Notes { get; set; }

        public bool IsPromotionDiscountPercentage { get; set; }

        public decimal PromotionDiscountValue { get; set; }

        public EnumOrderItemStatus StatusId { get; set; }

        public Guid? PromotionId { get; set; }

        public string PromotionName { get; set; }

        public Guid? ProductId { get; set; }

        public string ProductName { get; set; }

        public bool IsCombo { get; set; }

        public Guid? StoreId { get; set; }

        public virtual Order Order { get; set; }

        public virtual ProductPrice ProductPrice { get; set; }

        public virtual Promotion Promotion { get; set; }

        /// <summary>
        /// If the order item is a combo, this field will not null
        /// </summary>
        public virtual OrderComboItem OrderComboItem { get; set; }

        public virtual ICollection<OrderItemOption> OrderItemOptions { get; set; }

        public virtual ICollection<OrderItemTopping> OrderItemToppings { get; set; }
    }
}
