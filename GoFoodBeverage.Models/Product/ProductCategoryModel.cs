using System;
using System.Collections.Generic;

namespace GoFoodBeverage.Models.Product
{
    public class ProductCategoryModel
    {
        public int No { get; set; }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Priority { get; set; }

        public int NumberOfProduct { get; set; }

        public IEnumerable<ProductDatatableModel> Products { get; set; }
    }
}
