using System;

namespace GoFoodBeverage.POS.Models.Product
{
    public class ProductPriceModel
    {
        public Guid Id { get; set; }

        public bool IsApplyPromotion { get; set; } = false;

        public string PriceName { get; set; }

        public decimal PriceValue { get; set; }

        public decimal OriginalPrice { get; set; }
    }
}
