using GoFoodBeverage.Domain.Base;
using GoFoodBeverage.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(OrderHistory))]
    public class OrderHistory : BaseEntity
    {
        public Guid OrderId { get; set; }

        public string ActionName { get; set; }

        public string OldOrrderData { get; set; }

        public string NewOrderData { get; set; }

        public string Note { get; set; }

        [MaxLength(255)]
        public string CancelReason { get; set; }

        public Guid? StoreId { get; set; }

        public virtual Order Order { get; set; }
    }
}
