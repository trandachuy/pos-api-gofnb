using GoFoodBeverage.Common.Models.Base;
using System;

namespace GoFoodBeverage.Common.Models.Clone.Order
{
    public class ProductPriceCloneModel : BaseAuditModel
    {
        public Guid ProductId { get; set; }

        public string PriceName { get; set; }

        public decimal PriceValue { get; set; }
    }
}
