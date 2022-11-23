using GoFoodBeverage.Domain.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(OrderItemRestore))]
    public class OrderItemRestore : BaseAuditEntity
    {
        public Guid Id { get; set; }

        public Guid? OrderId { get; set; }

        public Guid? ProductPriceId { get; set; }

        public Guid? OrderRestoreId { get; set; }

        public string ItemName { get; set; }

        public string ProductPriceName { get; set; }

        public decimal OriginalPrice { get; set; }

        public decimal? PriceAfterDiscount { get; set; }

        public int Quantity { get; set; }

        public decimal TotalPriceAfterDiscount { get { return PriceAfterDiscount.Value * Quantity; } }

        public string Notes { get; set; }

        public bool IsPromotionDiscountPercentage { get; set; }

        public decimal PromotionDiscountValue { get; set; }

        public Guid? PromotionId { get; set; }

        public string PromotionName { get; set; }

        public Guid? ProductId { get; set; }

        public string ProductName { get; set; }

        public bool IsCombo { get; set; }

        /// <summary>
        /// Json object ProductPrice
        /// </summary>
        public string ProductPrice { get; set; }

        /// <summary>
        /// Json object Promotion
        /// </summary>
        public string Promotion { get; set; }

        /// <summary>
        /// Json list OrderItemOptions
        /// </summary>
        public string OrderItemOptions { get; set; }

        /// <summary>
        /// Json list OrderItemToppings
        /// </summary>
        public string OrderItemToppings { get; set; }

        public Guid? StoreId { get; set; }

        public virtual OrderRestore OrderRestore { get; set; }
    }
}
