using GoFoodBeverage.Domain.Base;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(OrderDeliveryTransaction))]
    public class OrderDeliveryTransaction : BaseEntity
    {
        public Guid? OrderId { get; set; }

        public Guid? DeliveryMethodId { get; set; }

        [MaxLength(100)]
        [Description("Status of delivery transaction")]
        public string Status { get; set; }

        [Description("Time to create transaction request")]
        public DateTime RequestTime { get; set; }

        public string RequestData { get; set; }

        public string ResponseData { get; set; }

        public Guid? StoreId { get; set; }

        public virtual DeliveryMethod DeliveryMethod { get; set; }

        public virtual Order Order { get; set; }
    }
}
