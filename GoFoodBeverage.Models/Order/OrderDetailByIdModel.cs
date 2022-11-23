using System;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Models.Store;

namespace GoFoodBeverage.Models.Order
{
    public class OrderDetailByIdModel
    {
        public Guid Id { get; set; }

        public string Code { get; set; }

        public EnumOrderStatus StatusId { get; set; }

        public string StringCode { get; set; }

        public string Note { get; set; }

        public decimal OriginalPrice { get; set; }

        public decimal TotalDiscountAmount { get; set; }

        public decimal TotalFee { get; set; }

        public decimal TotalTax { get; set; }

        public decimal DeliveryFee { get; set; }

        public decimal TotalAmount { get { return OriginalPrice - TotalDiscountAmount + TotalFee + TotalTax + DeliveryFee; } }

        public EnumPaymentMethod PaymentMethodId { get; set; }

        public string PaymentMethodName { get { return PaymentMethodId.GetName(); } }

        public EnumOrderPaymentStatus OrderPaymentStatusId { get; set; }

        public string OrderPaymentStatusName { get { return OrderPaymentStatusId.GetName(); } }

        public DateTime? CreatedTime { get; set; }

        public OrderDeliveryDto OrderDelivery { get; set; }

        public class OrderDeliveryDto
        {
            public string SenderAddress { get; set; }

            public string ReceiverAddress { get; set; }

            public string SenderName { get; set; }

            public string SenderPhone { get; set; }

            public string PhoneCode { get; set; }
        }

        public Guid StoreId { get; set; }

        public Guid DeliveryMethodId { get; set; }

        public Guid BranchId { get; set; }

        public StoreModel Store { get; set; }
    }
}
