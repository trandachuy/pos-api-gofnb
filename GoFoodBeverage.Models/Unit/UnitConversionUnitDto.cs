using System;

namespace GoFoodBeverage.Models.Unit
{
    public class UnitConversionUnitDto
    {
        public Guid Id { get; set; }

        public int Capacity { get; set; }

        public Guid UnitId { get; set; }

        public Guid? MaterialId { get; set; }

        public UnitModel Unit { get; set; }

        public class UnitModel
        {
            public Guid Id { get; set; }

            public string Name { get; set; }
        }
    }
}
