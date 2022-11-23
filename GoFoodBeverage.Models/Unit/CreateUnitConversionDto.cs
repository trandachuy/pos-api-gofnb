using System;

namespace GoFoodBeverage.Models.Unit
{
    public class CreateUnitConversionDto
    {
        public Guid? UnitId { get; set; }

        public int Capacity { get; set; }
    }
}
