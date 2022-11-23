using GoFoodBeverage.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Models.Order
{
    public class OrderStatusModel
    {
        public EnumOrderStatus OrderStatus { get; set; } = EnumOrderStatus.Draft;

        public EnumOrderPaymentStatus? PaymentStatus { get; set; }
    }
}
