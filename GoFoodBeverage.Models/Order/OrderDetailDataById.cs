using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Models.Address;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Models.Order
{
    public class OrderDetailDataById
    {
        public Guid Id { get; set; }

        public Guid? StoreId { get; set; }

        public Guid? BranchId { get; set; }

        public string BranchName { get; set; }

        public string CashierName { get; set; }

        public Guid? ShiftId { get; set; }

        public Guid? CustomerId { get; set; }

        public string Code { get; set; }

        public EnumOrderStatus StatusId { get; set; }

        public string StatusName { get { return StatusId.GetName(); } }

        public string Reason { get; set; }

        public EnumOrderType OrderTypeId { get; set; }

        public string StringCode { get; set; }

        public string OrderTypeFirstCharacter { get { return OrderTypeId.GetFirstCharacter(); } }

        public string OrderTypeName { get { return OrderTypeId.GetName(); } }

        public decimal TotalFee { get; set; }

        public decimal DeliveryFee { get; set; }

        public decimal OriginalPrice { get; set; }

        public decimal TotalDiscountAmount { get; set; }

        public decimal GrossTotalAmount { get; set; }

        public decimal CustomerDiscountAmount { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal TotalAmount { get { return OriginalPrice - TotalDiscountAmount + TotalFee + DeliveryFee - CustomerDiscountAmount; } }

        public decimal TotalCost { get; set; }

        public decimal Profit { get { return TotalAmount - TotalCost; } }

        public EnumPaymentMethod PaymentMethodId { get; set; }

        public string PaymentMethodName { get { return PaymentMethodId.GetName(); } }

        public DateTime? CreatedTime { get; set; }

        public Guid? CreatedUser { get; set; }

        public CustomerDto Customer { get; set; }

        public OrderDeliveryDto OrderDelivery { get; set; }

        public string Note { get; set; }

        public IEnumerable<OrderFeeDto> OrderFees { get; set; }

        public IEnumerable<OrderItemDto> OrderItems { get; set; }

        public class CustomerDto
        {
            public Guid Id { get; set; }

            public string FullName { get; set; }

            public string PhoneNumber { get; set; }

            public int AccumulatedPoint { get; set; }

            public virtual AddressModel Address { get; set; }

            public string Rank { get; set; }
        }

        public class OrderDeliveryDto
        {
            public Guid Id { get; set; }

            public string SenderName { get; set; }

            public string SenderPhone { get; set; }

            public string SenderAddress { get; set; }

            public string ReceiverName { get; set; }

            public string ReceiverPhone { get; set; }

            public string ReceiverAddress { get; set; }
        }

        public class OrderFeeDto
        {
            public Guid FeeId { get; set; }

            public bool IsPercentage { get; set; }

            public decimal FeeValue { get; set; }

            public string FeeName { get; set; }
        }

        public class OrderItemDto
        {
            public Guid Id { get; set; }

            public Guid? OrderId { get; set; }

            public bool IsCombo { get; set; }

            public Guid? ProductPriceId { get; set; }

            public decimal? Price { get; set; }

            public string ProductPriceName { get; set; }

            public decimal ProductPriceValue { get; set; }

            public int Quantity { get; set; }

            public string Notes { get; set; }

            public bool IsPromotionDiscountPercentage { get; set; }

            public decimal PromotionDiscountValue { get; set; }

            public decimal OriginalPrice { get; set; }

            public decimal? PriceAfterDiscount { get; set; }

            public Guid? PromotionId { get; set; }

            public ProductPriceDto ProductPrice { get; set; }

            public class ProductPriceDto
            {
                public string PriceName { get; set; }

                public decimal PriceValue { get; set; }

                public ProductDto Product { get; set; }

                public class ProductDto
                {
                    public string Name { get; set; }

                    public Guid Id { get; set; }

                    public string Description { get; set; }

                    public string Thumbnail { get; set; }
                }
            }

            public IEnumerable<OrderItemOptionModel> OrderItemOptions { get; set; }

            public IEnumerable<OrderItemToppingModel> OrderItemToppings { get; set; }

            public OrderComboItemDto OrderComboItem { get; set; }

            public class OrderComboItemDto
            {
                public Guid? OrderItemId { get; set; }

                public Guid? ComboId { get; set; }

                public Guid? ComboPricingId { get; set; }

                public string ComboName { get; set; }

                public decimal OriginalPrice { get; set; }

                public decimal SellingPrice { get; set; }

                public IEnumerable<OrderComboProductPriceItemDto> OrderComboProductPriceItems { get; set; }

                public class OrderComboProductPriceItemDto
                {
                    public Guid? OrderComboItemId { get; set; }

                    public Guid? ProductPriceId { get; set; }

                    public string ItemName { get; set; }

                    /// <summary>
                    /// The order session item status.
                    /// </summary>
                    public EnumOrderItemStatus StatusId { get; set; }

                    public ProductPriceDto ProductPrice { get; set; }

                    public IEnumerable<OrderItemOptionModel> OrderItemOptions { get; set; }

                    public IEnumerable<OrderItemToppingModel> OrderItemToppings { get; set; }


                }
            }
        }
    }
}
