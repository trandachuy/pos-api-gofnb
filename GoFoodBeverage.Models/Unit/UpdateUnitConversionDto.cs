using System;

namespace GoFoodBeverage.Models.Unit
{
    public class UpdateUnitConversionDto
    {
        public Guid Id { get; set; }

        public Guid? UnitId { get; set; }

        public int Capacity { get; set; }

        public Guid? MaterialId { get; set; }
    }
}
