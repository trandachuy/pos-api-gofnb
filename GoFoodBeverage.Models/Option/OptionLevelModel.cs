using System;

namespace GoFoodBeverage.Models.Option
{
    public class OptionLevelModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool IsSetDefault { get; set; }

        public decimal? Quota { get; set; }
    }
}
