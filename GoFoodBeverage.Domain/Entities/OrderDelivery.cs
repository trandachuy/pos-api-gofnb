using GoFoodBeverage.Domain.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(OrderDelivery))]
    public class OrderDelivery : BaseEntity
    {
        public string SenderName { get; set; }

        public string SenderPhone { get; set; }

        public string SenderAddress { get; set; }

        public double? SenderLat { get; set; }

        public double? SenderLng { get; set; }

        public string ReceiverName { get; set; }

        public string ReceiverPhone { get; set; }

        public string ReceiverAddress { get; set; }

        public double? ReceiverLat { get; set; }

        public double? ReceiverLng { get; set; }

        public Guid? StoreId { get; set; }

        public virtual Order Order { get; set; }
    }
}
