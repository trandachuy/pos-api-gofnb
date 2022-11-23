using System;
using GoFoodBeverage.Domain.Enums;

namespace GoFoodBeverage.Models.DeliveryMethod
{
    public class EstimateFeeDeliveryMethodsModel
    {
        public Guid DeliveryMethodId { get; set; }

        public EnumDeliveryMethod EnumId { get; set; }

        public decimal FeeValue { get; set; }
    }
}
