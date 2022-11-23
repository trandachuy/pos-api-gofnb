using GoFoodBeverage.Domain.Base;
using GoFoodBeverage.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(CustomerSegment))]
    public class CustomerSegment : BaseEntity
    {
        public Guid? StoreId { get; set; }

        public string Name { get; set; }

        public bool IsAllMatch { get; set; }

        public virtual ICollection<CustomerCustomerSegment> CustomerCustomerSegments { get; set; }

        public virtual ICollection<CustomerSegmentCondition> CustomerSegmentConditions { get; set; }

    }
}
