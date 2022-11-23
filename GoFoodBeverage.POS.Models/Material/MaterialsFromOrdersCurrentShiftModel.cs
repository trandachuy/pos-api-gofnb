using System;

namespace GoFoodBeverage.POS.Models.Material
{
    public class MaterialsFromOrdersCurrentShiftModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Sku { get; set; }

        public int? Quantity { get; set; }

        public string UnitName { get; set; }

        public int? Used { get; set; }

        public string Thumbnail { get; set; }
    }
}
