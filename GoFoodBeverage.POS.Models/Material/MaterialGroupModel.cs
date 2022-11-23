using System;

namespace GoFoodBeverage.POS.Models.Material
{
    public class MaterialGroupModel
    {
        public Guid? MaterialId { get; set; }

        public decimal? Quota { get; set; }

        public Guid? ProductPriceId { get; set; }

    }
}
