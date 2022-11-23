using System.Collections.Generic;

namespace GoFoodBeverage.Models.Order
{
    public class OrderRevenueModel
    {
        public IEnumerable<PaymentMethodDto> PaymentMethods { get; set; }

        public class PaymentMethodDto
        {
            public string Name { get; set; }

            public int TotalOrder { get; set; }

            public decimal TotalAmount { get; set; }
        }
    }
}
