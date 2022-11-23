
using System;

namespace GoFoodBeverage.Models.Order
{
    public class OrderTopSellingProductModel
    {
        public int Quantity { get; set; }

        public string ProductName { get; set; }

        public string PriceName { get; set; }

        public decimal TotalCost { get; set; }

        public int No { get; set; }

        public string Thumbnail { get; set; }

        public string Category { get; set; }

        public decimal TotalProductCost { get; set; }
    }
}
