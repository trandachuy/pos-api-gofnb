using GoFoodBeverage.Domain.Enums;
using System;
using System.Collections.Generic;

namespace GoFoodBeverage.POS.Models.Order
{
    public class DetailOrderToPrintModel
    {
        public string StoreName { get; set; }

        public string Address { get; set; }

        public string Country { get; set; }

        public string State { get; set; }

        public string City { get; set; }

        public string District { get; set; }

        public string Ward { get; set; }

        public string FullAddress
        {
            get { return $"{Address}, {District}, {Ward}, {State}, {City}, {Country}."; }
        }

        public string OrderCode { get; set; }

        public string OrderTime { get; set; }

        public string CashierName { get; set; }

        public string CustomerName { get; set; }

        public List<ProductListModel> OrderItems { get; set; }

        public class ProductListModel
        {
            public Guid? ProductPriceId { get; set; }

            public EnumOrderItemStatus StatusId { get; set; }

            public string ProductName { get; set; }

            public string PriceName { get; set; }

            public int Quantity { get; set; }

            public decimal Price { get; set; }

            public OrderComboItemDto OrderComboItem { get; set; }

            public bool IsCombo { get; set; }

            public decimal TotalPrice
            {
                get { return Price * Quantity ; }
            }

            public IEnumerable<ToppingListModel> OrderItemToppings { get; set; }

            public class OrderComboItemDto
            {
                public Guid? OrderItemId { get; set; }

                public Guid? ComboId { get; set; }

                public Guid? ComboPricingId { get; set; }

                public string ComboName { get; set; }

                public decimal OriginalPrice { get; set; }

                public decimal SellingPrice { get; set; }

                public Guid? StoreId { get; set; }

                public IEnumerable<ComboItemDto> OrderComboProductPriceItems { get; set; }

                public class ComboItemDto
                {
                    public Guid? ProductPriceId { get; set; }

                    public string ItemName { get; set; }

                    public string Thumbnail { get; set; }

                    public int Quantity { get; set; }

                    public Guid ProductId { get; set; }

                    public List<ProductOptionModel> OrderItemOptions { get; set; } = new List<ProductOptionModel>();

                    public List<ProductToppingDto> OrderItemToppings { get; set; } = new List<ProductToppingDto>();
                }

                public class ProductOptionModel
                {
                    public Guid? OptionId { get; set; }

                    public Guid? OptionLevelId { get; set; }

                    public string OptionName { get; set; }

                    public string OptionLevelName { get; set; }

                    public bool IsSetDefault { get; set; }
                }

                public class ProductToppingDto
                {
                    public Guid ToppingId { get; set; }

                    public string Name { get; set; }

                    public decimal OriginalPrice { get; set; }

                    public decimal PriceAfterDiscount { get; set; }

                    public int Quantity { get; set; }
                }
            }

            public class ToppingListModel
            {
                public Guid ToppingId { get; set; }
                public string ToppingName { get; set; }

                public int Quantity { get; set; }

                public decimal Price { get; set; }

                public decimal TotalPrice
                {
                    get { return Price * Quantity; }
                }
            }

            public IEnumerable<OptionListModel> OrderItemOptions { get; set; }

            public class OptionListModel
            {
                public string OptionName { get; set; }

                public string OptionValue { get; set; }
                public Guid? OptionLevelId { get; set; }
            }
        }

        public decimal TotalPrice { get; set; }

        public decimal Discount { get; set; }

        public EnumOrderType OrderTypeId { get; set; }

        public decimal DeliveryFee { get; set; }

        public decimal FeeAndTax { get; set; }

        public decimal FinalPrice
        {
            get
            {
                // If Order type is Delivery => the final price will include the shipping fee
                if (OrderTypeId == EnumOrderType.Delivery)
                    return (TotalPrice + FeeAndTax + DeliveryFee) - Discount;
                return (TotalPrice + FeeAndTax) - Discount;
            }
        }

        public decimal ReceivedAmount { get; set; }

        public decimal Change { get; set; }

        public EnumPaymentMethod PaymentMethodId { get; set; }
    }
}
