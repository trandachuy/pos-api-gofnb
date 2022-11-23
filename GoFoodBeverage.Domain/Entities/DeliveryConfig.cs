using System;
using System.ComponentModel;
using GoFoodBeverage.Domain.Base;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(DeliveryConfig))]
    public class DeliveryConfig : BaseEntity
    {
        public Guid? StoreId { get; set; }

        public EnumDeliveryMethod DeliveryMethodEnumId { get; set; }

        public Guid? DeliveryMethodId { get; set; }

        public bool? IsFixedFee { get; set; }

        public decimal? FeeValue { get; set; }

        /// <summary>
        /// ApiKey for AhaMove
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Your phone number used to register account ID
        /// </summary>
        [MaxLength(50)]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Store name
        /// </summary>
        [MaxLength(1024)]
        public string Name { get; set; }

        /// <summary>
        /// Store address
        /// </summary>
        [MaxLength(1024)]
        public string Address { get; set; }

        [Description("If the value is true, the delivery method is not supported")]
        public bool? IsActivated { get; set; }

        public virtual Store Store { get; set; }

        public virtual ICollection<DeliveryConfigPricing> DeliveryConfigPricings { get; set; }
    }
}
