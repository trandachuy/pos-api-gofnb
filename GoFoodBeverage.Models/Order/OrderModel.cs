using System;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Models.Customer;

namespace GoFoodBeverage.Models.Order
{
    public class OrderModel
    {
        public Guid Id { get; set; }

        public Guid? StoreId { get; set; }

        public Guid? BranchId { get; set; }

        public Guid? ShiftId { get; set; }

        public Guid? CustomerId { get; set; }

        public string Code { get; set; }

        public string StringCode
        {
            get
            {
                return $"{OrderTypeId.GetFirstCharacter()}{Code}";
            }
        }

        public EnumOrderStatus StatusId { get; set; }

        public string StatusName { get { return StatusId.GetName(); } }

        public EnumOrderType OrderTypeId { get; set; }

        public string OrderTypeFirstCharacter { get { return OrderTypeId.GetFirstCharacter(); } }

        public string OrderTypeName { get { return OrderTypeId.GetName(); } }

        public decimal OriginalPrice { get; set; }

        public decimal TotalDiscountAmount { get; set; }

        public decimal TotalFee { get; set; }

        public decimal DeliveryFee { get; set; }

        public decimal TotalAmount { get { return OriginalPrice - TotalDiscountAmount + TotalFee + DeliveryFee; } }

        public EnumPaymentMethod PaymentMethodId { get; set; }

        public string PaymentMethodName { get { return PaymentMethodId.GetName(); } }

        public CustomerDto Customer { get; set; }

        public class CustomerDto
        {
            public Guid Id { get; set; }

            public string FullName { get; set; }

            public string PhoneNumber { get; set; }

            public int AccumulatedPoint { get; set; }

            public string Rank { get; set; }
        }
    }
}
