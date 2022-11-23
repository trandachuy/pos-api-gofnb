using GoFoodBeverage.Domain.Base;
using GoFoodBeverage.Domain.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(OrderPaymentTransaction))]
    public class OrderPaymentTransaction : BaseEntity
    {
        public Guid? OrderId { get; set; }

        public EnumPaymentMethod PaymentMethodId { get; set; }

        [Description("Return value: PAYMENT or REFUND")]
        public EnumTransactionType? TransactionType { get; set; }

        public long? TransId { get; set; }

        public string OrderInfo { get; set; }

        public decimal Amount { get; set; }

        public string ExtraData { get; set; }

        public string PaymentUrl { get; set; }

        public string ResponseData { get; set; }

        public bool IsSuccess { get; set; }

        public Guid? StoreId { get; set; }
    }
}
