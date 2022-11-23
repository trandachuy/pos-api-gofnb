using GoFoodBeverage.Domain.Base;
using GoFoodBeverage.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(OrderSession))]
    public class OrderSession : BaseEntity
    {
        public Guid? OrderId { get; set; }

        public EnumOrderSessionStatus StatusId { get; set; }

        public Guid? StoreId { get; set; }

        public virtual Order Order { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
