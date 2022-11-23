using GoFoodBeverage.Domain.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(CustomerCustomerSegment))]
    public class CustomerCustomerSegment : BaseAuditEntity
    {
        [Key]
        public Guid CustomerId { get; set; }

        [Key]
        public Guid CustomerSegmentId { get; set; }

        public Guid? StoreId { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual CustomerSegment CustomerSegment { get; set; }
    }
}
