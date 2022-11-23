using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Models.Order
{
    public class OrderStatisticModel
    {
        public Guid Id { get; set; }

        public decimal PriceAfterDiscount { get; set; }

        public DateTime? CreatedTime { get; set; }
    }
}
