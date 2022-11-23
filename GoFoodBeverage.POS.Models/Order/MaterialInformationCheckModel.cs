using System;

namespace GoFoodBeverage.POS.Models.Order
{
    public class MaterialInformationCheckModel
    {
        public Guid MaterialId { get; set; }

        public decimal? TotalAmountNeeded { get; set; }
    }
}
