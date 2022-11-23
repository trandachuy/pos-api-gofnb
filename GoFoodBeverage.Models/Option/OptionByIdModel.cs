using GoFoodBeverage.Models.Material;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Models.Option
{
    public class OptionByIdModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid? MaterialId { get; set; }

        public List<OptionLevelModel> OptionLevel { get; set; }
    }
}
