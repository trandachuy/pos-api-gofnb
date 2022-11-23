using System;

namespace GoFoodBeverage.POS.Models.Product
{
    /// <summary>
    /// The ProductPriceModel inheritance from ProductPrice 
    /// </summary>
    public class PosProductPriceModel
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public string PriceName { get; set; }

        public decimal PriceValue { get; set; }
    }
}
