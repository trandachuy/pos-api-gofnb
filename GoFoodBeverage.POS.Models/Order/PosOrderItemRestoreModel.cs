using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.Models.Order
{
    public class PosOrderItemRestoreModel
    {
        public Guid Id { get; set; }

        public Guid? OrderId { get; set; }

        public Guid? ProductPriceId { get; set; }

        /// <summary>
        /// This name compiled from the product name and price name
        /// </summary>
        public string ProductPriceName { get; set; }

        /// <summary>
        /// Original price of product price
        /// </summary>
        public decimal OriginalPrice { get; set; }

        /// <summary>
        /// Price of item after discount
        /// </summary>
        public decimal? PriceAfterDiscount { get; set; }

        public int Quantity { get; set; }

        public decimal TotalPriceAfterDiscount { get { return PriceAfterDiscount.Value * Quantity; } }

        public string Notes { get; set; }

        public bool IsPromotionDiscountPercentage { get; set; }

        public decimal PromotionDiscountValue { get; set; }


        public Guid? PromotionId { get; set; }

        public string PromotionName { get; set; }

        public Guid? ProductId { get; set; }

        public string ProductName { get; set; }

        public bool IsCombo { get; set; }

        public virtual Models.Product.PosProductPriceModel ProductPrice { get; set; }

        public virtual PosPromotionModel Promotion { get; set; }

        /// <summary>
        /// If the order item is a combo, this field will not null
        /// </summary>
        public virtual PosOrderComboItemModel OrderComboItem { get; set; }

        public virtual ICollection<PosOrderItemOptionModel> OrderItemOptions { get; set; }

        public virtual ICollection<PosOrderItemToppingModel> OrderItemToppings { get; set; }

    }
}
