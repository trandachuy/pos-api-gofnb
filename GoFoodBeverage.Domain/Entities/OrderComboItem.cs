using System;
using GoFoodBeverage.Domain.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(OrderComboItem))]
    public class OrderComboItem : BaseEntity
    {
        public Guid? OrderItemId { get; set; }

        public Guid? ComboId { get; set; }

        public Guid? ComboPricingId { get; set; }

        public string ComboName { get; set; }

        public decimal OriginalPrice { get; set; }

        public decimal SellingPrice { get; set; }

        public Guid? StoreId { get; set; }

        public virtual ICollection<OrderComboProductPriceItem> OrderComboProductPriceItems { get; set; }
    }
}
