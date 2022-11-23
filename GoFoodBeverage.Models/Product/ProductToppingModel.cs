using System;

namespace GoFoodBeverage.Models.Product
{
    public class ProductToppingModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Thumbnail { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }
    }
}
