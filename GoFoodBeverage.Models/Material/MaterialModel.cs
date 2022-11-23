using System;
using GoFoodBeverage.Models.Unit;

namespace GoFoodBeverage.Models.Material
{
    public class MaterialModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Sku { get; set; }

        public int? Quantity { get; set; }

        public decimal? Cost { get; set; }

        public decimal? CostPerUnit { get; set; }

        public string UnitName { get; set; }

        public UnitModel Unit { get; set; }

        public bool IsActive { get; set; }

        public string Thumbnail { get; set; }
    }
}
