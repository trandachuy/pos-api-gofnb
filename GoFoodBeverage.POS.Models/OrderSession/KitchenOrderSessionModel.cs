using GoFoodBeverage.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.Models.OrderSession
{
    public class KitchenOrderSessionModel
    {
        public Guid? SessionId { get; set; }

        public Guid? OrderId { get; set; }

        public string OrderCode { get; set; }

        public string SessionCode { get; set; }

        public string SessionIndex { get; set; }

        public EnumOrderType? OrderTypeId { get; set; }

        public string OrderTypeName { get; set; }

        public string AreaName { get; set; }

        public string TableName { get; set; }

        public DateTime? CreatedTime { get; set; }

        public List<OrderItemDto> OrderItems { get; set; }

        public class OrderItemDto
        {
            public Guid? OrderItemId { get; set; }

            public Guid? ProductId { get; set; }

            public string ProductName { get; set; }

            public Guid? ProductPriceId { get; set; }

            public string ProductPriceName { get; set; }

            public int? CurrentQuantity { get; set; }

            public int? DefaultQuantity { get; set; }

            public string Notes { get; set; }

            public DateTime? CreatedTime { get; set; }

            public EnumOrderItemStatus? StatusId { get; set; }

            public Guid? OrderComboProductPriceItemId { get; set; }

            public List<OrderItemOptionsDto> OrderItemOptions { get; set; }

            public class OrderItemOptionsDto
            {
                public Guid? OptionId { get; set; }

                public Guid? OptionLevelId { get; set; }

                public string OptionName { get; set; }

                public string OptionLevelName { get; set; }

                public bool? IsSetDefault { get; set; }
            }

            public List<OrderItemToppingsDto> OrderItemToppings { get; set; }

            public class OrderItemToppingsDto
            {
                public Guid? ToppingId { get; set; }

                public string ToppingName { get; set; }

                public decimal? ToppingValue { get; set; }

                public int? Quantity { get; set; }
            }
        }

        public int? TotalOrderItem { get; set; } //Count all orderitem

        public int? TotalOrderItemCanceled { get; set; } //Count all cancel orderitem

        public int? TotalOrderItemCompleted { get; set; } //Use for kitchen 
    }
}
