using GoFoodBeverage.Models.Unit;
using System;
using System.Collections.Generic;

namespace GoFoodBeverage.Models.Product
{
    /// <summary>
    /// This modal return fields: 
    /// Id, Name, ProductPrices
    /// </summary>
    public class ProductModel
    {
        public Guid Id { get; set; }

        public Guid ProductCategoryId { get; set; }

        public string Name { get; set; }

        public string Thumbnail { get; set; }

        public UnitModel Unit { get; set; }

        public IEnumerable<ProductPriceModel> ProductPrices { get; set; }
    }
}
