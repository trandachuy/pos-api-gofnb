using System;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Enums;

namespace GoFoodBeverage.Models.DeliveryMethod
{
    public class DeliveryMethodByStoreIdModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public EnumDeliveryMethod EnumId { get; set; }

        public bool IsEnabled { get; set; }

        public IEnumerable<DeliveryConfigDto> DeliveryConfigs { get; set; }

        public class DeliveryConfigDto
        {
            public EnumDeliveryMethod DeliveryMethodEnumId { get; set; }

            public bool? IsFixedFee { get; set; }

            public decimal FeeValue { get; set; }

            public IEnumerable<DeliveryConfigPricingDto> DeliveryConfigPricings { get; set; }

            public class DeliveryConfigPricingDto
            {
                public int FromDistance { get; set; }

                public int FromDistanceMeter
                {
                    get
                    {
                        return FromDistance * 1000;
                    }
                }

                public int ToDistance { get; set; }

                public int ToDistanceMeter
                {
                    get
                    {
                        return ToDistance * 1000;
                    }
                }

                public decimal FeeValue { get; set; }
            }
        }
    }
}
