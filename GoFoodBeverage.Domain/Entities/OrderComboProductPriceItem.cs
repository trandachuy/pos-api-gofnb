using System;
using GoFoodBeverage.Domain.Base;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(OrderComboProductPriceItem))]
    public class OrderComboProductPriceItem : BaseEntity
    {
        public Guid? OrderComboItemId { get; set; }

        public Guid? ProductPriceId { get; set; }

        public string ItemName { get; set; }

        public string Note { get; set; }

        /// <summary>
        /// The order session item status.
        /// </summary>
        public EnumOrderItemStatus StatusId { get; set; }

        public Guid? StoreId { get; set; }

        public virtual ProductPrice ProductPrice { get; set; }

        public virtual ICollection<OrderItemOption> OrderItemOptions { get; set; }

        public virtual ICollection<OrderItemTopping> OrderItemToppings { get; set; }
    }
}
