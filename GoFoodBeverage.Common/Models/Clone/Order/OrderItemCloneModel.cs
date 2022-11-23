using GoFoodBeverage.Common.Models.Base;
using System;
using System.Collections.Generic;

namespace GoFoodBeverage.Common.Models.Clone.Order
{
    public class OrderItemCloneModel: BaseAuditModel
    {
        public Guid Id { get; set; }

        public Guid? OrderId { get; set; }

        public Guid? ProductPriceId { get; set; }

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

        public ProductPriceCloneModel ProductPrice { get; set; }

        public PromotionCloneModel Promotion { get; set; }

        public IEnumerable<OrderItemOptionCloneModel> OrderItemOptions { get; set; }

        public IEnumerable<OrderItemToppingCloneModel> OrderItemToppings { get; set; }
    }
}
