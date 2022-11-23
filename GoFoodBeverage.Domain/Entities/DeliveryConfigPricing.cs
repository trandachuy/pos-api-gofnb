using GoFoodBeverage.Domain.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(DeliveryConfigPricing))]
    public class DeliveryConfigPricing : BaseEntity
    {
        public Guid? DeliveryConfigId { get; set; }

        public int Position { get; set; }

        public int FromDistance { get; set; }

        public int ToDistance { get; set; }

        public decimal FeeValue { get; set; }

        public Guid? StoreId { get; set; }

        public virtual DeliveryConfig DeliveryConfig { get; set; }
    }
}
