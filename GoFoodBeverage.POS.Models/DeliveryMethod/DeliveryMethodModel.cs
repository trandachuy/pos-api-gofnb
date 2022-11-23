using System;
using GoFoodBeverage.Domain.Enums;
using System.Collections.Generic;

namespace GoFoodBeverage.POS.Models.DeliveryMethod
{
    public class DeliveryMethodModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public EnumDeliveryMethod EnumId { get; set; }
    }
}
