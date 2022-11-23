using System;
using System.Collections.Generic;

namespace GoFoodBeverage.Models.Order
{
    public class OrderComboItemModel
    {
        public Guid? OrderItemId { get; set; }

        public Guid? ComboId { get; set; }

        public Guid? ComboPricingId { get; set; }

        public string ComboName { get; set; }

        public decimal OriginalPrice { get; set; }

        public decimal SellingPrice { get; set; }

        public Guid? StoreId { get; set; }

        public string Thumbnail { get; set; }

        public ICollection<OrderComboProductPriceItemModel> OrderComboProductPriceItems { get; set; }
    }
}
