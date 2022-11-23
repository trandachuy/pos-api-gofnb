using GoFoodBeverage.POS.Models.Combo;
using GoFoodBeverage.POS.Models.Product;
using System;
using System.Collections.Generic;

namespace GoFoodBeverage.POS.Models.Order
{
    public class OrderCartItemRequestModel
    {
        public Guid? OrderItemId { get; set; }

        public Guid ProductPriceId { get; set; }

        public int Quantity { get; set; }

        public string Notes { get; set; }

        public List<ProductOptionDto> Options { get; set; } = new List<ProductOptionDto>();

        public List<ProductToppingModel> Toppings { get; set; } = new List<ProductToppingModel>();

        public bool IsCombo { get; set; }

        public ComboOrderItemDto Combo { get; set; }

        public Guid ProductId { get; set; }

        public Guid? OrderId { get; set; }

        #region Promotion information of CartItem if exist
        public Guid? PromotionId { get; set; }
        public string PromotionName { get; set; }
        public decimal PromotionValue { get; set; }
        public bool IsPercentDiscount { get; set; }
        #endregion
    }
}
