using GoFoodBeverage.Domain.Base;
using GoFoodBeverage.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(PaymentConfig))]
    public class PaymentConfig : BaseEntity
    {
        public Guid? StoreId { get; set; }

        public Guid? PaymentMethodId { get; set; }

        public EnumPaymentMethod PaymentMethodEnumId { get; set; }

        /// <summary>
        /// Partner Code for MOMO | TerminateId for VNPAY
        /// </summary>
        public string PartnerCode { get; set; }

        public string AccessKey { get; set; }

        public string SecretKey { get; set; }

        public string QRCode { get; set; }

        public bool IsActivated { get; set; }

        public bool IsAuthenticated { get; set; }


        public virtual Store Store { get; set; }

        public virtual PaymentMethod PaymentMethod { get; set; }
    }
}
