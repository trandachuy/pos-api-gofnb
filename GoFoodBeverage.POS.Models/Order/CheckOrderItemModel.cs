using GoFoodBeverage.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.Models.Order
{
    public class CheckOrderItemModel
    {
        public Guid? Id { get; set; }

        public Guid? OrderId { get; set; }

        public Guid? OrderSessionId { get; set; }

        public bool IsCombo { get; set; }

        public EnumOrderItemStatus StatusId { get; set; }

        public OrderComboItemModel OrderComboItem { get; set; }

        public class OrderComboItemModel
        {
            public Guid? OrderItemId { get; set; }

            public Guid? ComboId { get; set; }
            
            public List<OrderComboProductPriceItemModel> OrderComboProductPriceItems { get; set; }

            public class OrderComboProductPriceItemModel
            {
                public Guid? OrderComboItemId { get; set; }

                public EnumOrderItemStatus StatusId { get; set; }
            }
        }
    }
}
