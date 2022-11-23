using System;
using GoFoodBeverage.Domain.Enums;

namespace GoFoodBeverage.Models.Order
{
    public class BaseOrderModel
    {
        public Guid Id { get; set; }

        public Guid BranchId { get; set; }

        public string Code { get; set; }

        public string StringCode { get; set; }

        public decimal TotalPrices { get; set; }

        public int TotalItems { get; set; }

        public DateTime? CreatedTime { get; set; }

        public EnumOrderStatus StatusId { get; set; }

        public EnumOrderType OrderTypeId { get; set; }

        public EnumPaymentMethod PaymentMethodId { get; set; }

        public EnumOrderPaymentStatus OrderPaymentStatusId { get; set; }

        public StoreModel Store { get; set; }

        public class StoreModel
        {
            public Guid? Id { get; set; }

            public string Title { get; set; }

            public string Logo { get; set; }
        }
    }
}
