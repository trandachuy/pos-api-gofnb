using System;
using System.Collections.Generic;

namespace GoFoodBeverage.POS.Models.Order
{
    public class PosOrderComboProductPriceItemModel
    {
        public Guid? OrderComboItemId { get; set; }

        public Guid? ProductPriceId { get; set; }

        public string ItemName { get; set; }

        public virtual ICollection<PosOrderItemOptionModel> OrderItemOptions { get; set; }

        public virtual ICollection<PosOrderItemToppingModel> OrderItemToppings { get; set; }
    }
}
