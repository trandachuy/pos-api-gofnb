using System;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Models.Address;

namespace GoFoodBeverage.Models.Order
{
    public class OrderDetailModel
    {
        public Guid Id { get; set; }

        public Guid? StoreId { get; set; }

        public Guid? BranchId { get; set; }

        public Guid? ShiftId { get; set; }

        public Guid? CustomerId { get; set; }

        public string Code { get; set; }

        public EnumOrderStatus StatusId { get; set; }

        public string StatusName { get { return StatusId.GetName(); } }

        public EnumOrderType OrderTypeId { get; set; }

        public string StringCode { get; set; }

        public string OrderTypeFirstCharacter { get { return OrderTypeId.GetFirstCharacter(); } }

        public string OrderTypeName { get { return OrderTypeId.GetName(); } }

        public decimal GrossTotalAmount { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal TotalAmount { get { return GrossTotalAmount - DiscountAmount; } }

        public EnumPaymentMethod PaymentMethodId { get; set; }

        public string PaymentMethodName { get { return PaymentMethodId.GetName(); } }

        public DateTime? CreatedTime { get; set; }

        public Guid? CreatedUser { get; set; }

        public CustomerDto Customer { get; set; }

        public class CustomerDto
        {
            public Guid Id { get; set; }

            public string FullName { get; set; }

            public string PhoneNumber { get; set; }

            public int AccumulatedPoint { get; set; }

            public virtual AddressModel Address { get; set; }
        }
    }
}
