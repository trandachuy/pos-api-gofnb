using System;

namespace GoFoodBeverage.Models.Order
{
    public class OrderItemToppingModel
    {
        public string ToppingName { get; set; }

        public decimal ToppingValue { get; set; }

        public int Quantity { get; set; }

        public Guid Id { get; set; }
    }
}
