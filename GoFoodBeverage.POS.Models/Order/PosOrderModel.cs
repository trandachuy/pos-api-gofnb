using System;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;

namespace GoFoodBeverage.POS.Models.Order
{
    public class PosOrderModel
    {
        public Guid Id { get; set; }

        public string Code { get; set; }

        public string StringCode { get; set; }

        public Guid? AreaTableId { get; set; }

        public decimal TotalFee { get; set; }

        public decimal TotalTax { get; set; }

        public decimal DeliveryFee { get; set; }

        public string ReceiverAddress { get; set; }

        public string AreaTableName { get; set; }

        public string AreaName { get; set; }

        public List<OrderItemDto> OrderItems { get; set; }

        public EnumOrderStatus StatusId { get; set; }

        public EnumOrderType OrderTypeId { get; set; }

        public EnumOrderPaymentStatus? OrderPaymentStatusId { get; set; }

        public string OrderTypeFirstCharacter { get { return OrderTypeId.GetFirstCharacter(); } }

        public string OrderTypeName { get { return OrderTypeId.GetName(); } }

        public DateTime? CreatedTime { get; set; }

        public decimal OriginalPrice { get; set; }

        public decimal TotalDiscountAmount { get; set; }

        public decimal PriceAfterDiscount { get { return OriginalPrice - TotalDiscountAmount; } }

        public Platform Platform { get; set; }

        public class OrderItemDto
        {
            public Guid? OrderId { get; set; }

            public EnumOrderItemStatus StatusId { get; set; }

            public int Quantity { get; set; }
        }
    }
}
