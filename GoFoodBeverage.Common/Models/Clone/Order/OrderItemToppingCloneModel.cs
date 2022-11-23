using GoFoodBeverage.Common.Models.Base;
using System;

namespace GoFoodBeverage.Common.Models.Clone.Order
{
    public class OrderItemToppingCloneModel : BaseAuditModel
    {
        public Guid Id { get; set; }

        public Guid OrderItemId { get; set; }

        public Guid? ToppingId { get; set; }

        public Guid? PromotionId { get; set; }

        public string ToppingName { get; set; }

        public decimal ToppingValue { get; set; }

        public decimal OriginalPrice { get; set; }

        public decimal PriceAfterDiscount { get; set; }

        public int Quantity { get; set; }

        public decimal TotalPriceAfterDiscount { get { return PriceAfterDiscount * Quantity; } }
    }
}
