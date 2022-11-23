using System;
using System.Collections.Generic;

namespace GoFoodBeverage.POS.Models.DeliveryMethod
{
    public class DeliveryConfigModel
    {
        public bool IsFixedFee { get; set; }

        public decimal FeeValue { get; set; }

        public IEnumerable<DeliveryConfigPricingDto> DeliveryConfigPricings { get; set; }

        public class DeliveryConfigPricingDto
        {
            public int FromDistance { get; set; }

            public double FromDistanceMeter
            {
                get
                {
                    return FromDistance * 1000;
                }
            }

            public int ToDistance { get; set; }

            public double ToDistanceMeter
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
