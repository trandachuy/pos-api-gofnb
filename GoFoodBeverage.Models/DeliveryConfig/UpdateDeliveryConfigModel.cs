using System;
using System.Collections.Generic;

namespace GoFoodBeverage.Models.DeliveryConfig
{
    public class UpdateDeliveryConfigModel
    {

        public Guid DeliveryMethodId { get; set; }

        public bool IsFixedFee { get; set; }

        public decimal FeeValue { get; set; }

        public IEnumerable<DeliveryConfigPricingDto> DeliveryConfigPricings { get; set; }

        public class DeliveryConfigPricingDto
        {
            public Guid Id { get; set; }

            public int Position { get; set; }

            public int FromDistance { get; set; }

            public int ToDistance { get; set; }

            public decimal FeeValue { get; set; }
        }
    }
}
