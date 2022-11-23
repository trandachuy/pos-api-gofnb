using GoFoodBeverage.Domain.Enums;
using System;
using System.Collections.Generic;

namespace GoFoodBeverage.Models.Payment
{
    public class PaymentMethodByStoreModel
    {
        public Guid Id { get; set; }

        public EnumPaymentMethod EnumId { get; set; }

        public string Name { get; set; }

        public string Icon { get; set; }

        public IEnumerable<PaymentConfigDto> PaymentConfigs { get; set; }

        public class PaymentConfigDto
        {
            public string PartnerCode { get; set; }
        }
    }
}
