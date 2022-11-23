using System;
using System.Collections.Generic;

namespace GoFoodBeverage.Models.Option
{
    public class OptionModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public IList<OptionLevelModel> OptionLevel { get; set; }
    }
}
