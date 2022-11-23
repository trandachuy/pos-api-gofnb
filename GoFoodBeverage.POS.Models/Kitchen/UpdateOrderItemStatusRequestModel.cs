using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.Models.Kitchen
{
    public class UpdateOrderItemStatusRequestModel
    {
        public Guid OrderItemId { get; set; }

        public Guid SessionId { get; set; }

        public Guid ProductId { get; set; }

        public DateTime? CreatedTime { get; set; }
    }
}
