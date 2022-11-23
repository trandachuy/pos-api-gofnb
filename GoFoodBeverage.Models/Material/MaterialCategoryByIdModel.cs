using System;
using System.Collections.Generic;

namespace GoFoodBeverage.Models.Material
{
    public class MaterialCategoryByIdModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<MaterialDto> Materials { get; set; }

        public class MaterialDto
        {
            public Guid Id { get; set; }

            public string Name { get; set; }

            public int? Quantity { get; set; }

            public string Sku { get; set; }

            public string UnitName { get; set; }

            public string Thumbnail { get; set; }
        }
    }
}
