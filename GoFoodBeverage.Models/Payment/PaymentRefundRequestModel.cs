using System;

namespace GoFoodBeverage.Models.Payment
{
    public class PaymentRefundRequestModel
    {
        public Guid StoreId { get; set; }

        public Guid OrderId { get; set; }

        public string PaymentMethod { get; set; }
    }
}
