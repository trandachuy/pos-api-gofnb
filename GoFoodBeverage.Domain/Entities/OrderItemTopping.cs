using System;
using GoFoodBeverage.Domain.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(OrderItemTopping))]
    public class OrderItemTopping : BaseEntity
    {
        public Guid OrderItemId { get; set; }

        public Guid? ToppingId { get; set; }

        public Guid? PromotionId { get; set; }

        public string ToppingName { get; set; }

        public decimal ToppingValue { get; set; }

        public decimal OriginalPrice { get; set; }

        public decimal PriceAfterDiscount { get; set; }

        public int Quantity { get; set; }

        public decimal TotalPriceAfterDiscount { get { return PriceAfterDiscount * Quantity; } }

        public Guid? StoreId { get; set; }
    }
}
