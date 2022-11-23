using System;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Models.Product;

namespace GoFoodBeverage.Models.Order
{
    public class OrderComboProductPriceItemModel
    {
        public Guid? OrderComboItemId { get; set; }

        public Guid? ProductPriceId { get; set; }

        public string ItemName { get; set; }

        public string Note { get; set; }

        public EnumOrderItemStatus StatusId { get; set; }

        public Guid? StoreId { get; set; }

        public ProductPriceModel ProductPrice { get; set; }

        public ICollection<OrderComboItemOptionModel> OrderItemOptions { get; set; }

        public ICollection<OrderComboItemToppingModel> OrderItemToppings { get; set; }
    }
}
