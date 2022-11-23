using GoFoodBeverage.Domain.Enums;
using System;
using System.Collections.Generic;

namespace GoFoodBeverage.Models.Payment
{
    public class PaymentMethodModel
    {
        public Guid Id { get; set; }

        public EnumPaymentMethod EnumId { get; set; }

        public string Name { get; set; }

        public string Icon { get; set; }

        public virtual IEnumerable<PaymentConfigModel> PaymentConfigs { get; set; }
    }
}
