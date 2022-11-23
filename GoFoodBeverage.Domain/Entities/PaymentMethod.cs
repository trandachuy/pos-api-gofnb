using GoFoodBeverage.Domain.Base;
using GoFoodBeverage.Domain.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(PaymentMethod))]
    public class PaymentMethod : BaseEntity
    {
        public EnumPaymentMethod EnumId { get; set; }

        public string Name { get; set; }

        public string Icon { get; set; }


        public virtual ICollection<PaymentConfig> PaymentConfigs { get; set; }
    }
}
