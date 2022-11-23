using GoFoodBeverage.Domain.Base;
using GoFoodBeverage.Domain.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(DeliveryMethod))]
    public class DeliveryMethod : BaseEntity
    {
        public EnumDeliveryMethod EnumId { get; set; }

        public string Name { get; set; }

        public string Icon { get; set; }

        public virtual ICollection<DeliveryConfig> DeliveryConfigs { get; set; }
    }
}
