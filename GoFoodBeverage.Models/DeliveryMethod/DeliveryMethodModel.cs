using System;
using GoFoodBeverage.Domain.Enums;
using System.Collections.Generic;

namespace GoFoodBeverage.Models.DeliveryMethod
{
    public class DeliveryMethodModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public EnumDeliveryMethod EnumId { get; set; }

        public DeliveryConfigDto DeliveryConfig { get; set; }

        public class DeliveryConfigDto
        {
            public Guid Id { get; set; }

            public bool? IsFixedFee { get; set; }

            public decimal? FeeValue { get; set; }

            public string ApiKey { get; set; }

            public string PhoneNumber { get; set; }

            public string Name { get; set; }

            public string Address { get; set; }

            public bool IsActivated { get; set; }

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
}
