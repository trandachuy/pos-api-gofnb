using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.Models.Order
{
    public class PosOrderItemOptionModel
    {
        public Guid? OptionId { get; set; }

        public Guid? OptionLevelId { get; set; }

        public Guid OrderItemId { get; set; }

        public string OptionName { get; set; }

        public string OptionLevelName { get; set; }
    }
}
