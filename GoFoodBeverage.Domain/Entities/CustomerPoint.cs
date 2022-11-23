using GoFoodBeverage.Domain.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(CustomerPoint))]
    public class CustomerPoint : BaseEntity
    {
        public Guid CustomerId { get; set; }

        public int AccumulatedPoint { get; set; }

        public int AvailablePoint { get; set; }
        
        public Guid? StoreId { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
