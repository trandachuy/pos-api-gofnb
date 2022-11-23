using System;

namespace GoFoodBeverage.Models.Product
{
    public class ProductPriceModel
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public string PriceName { get; set; }

        public decimal PriceValue { get; set; }
    }
}
