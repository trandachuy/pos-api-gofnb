using GoFoodBeverage.Domain.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(CustomerSegmentCondition))]
    public class CustomerSegmentCondition : BaseEntity
    {
        public Guid CustomerSegmentId { get; set; }

        public int? ObjectiveId { get; set; }

        public int? CustomerDataId { get; set; }

        public int? OrderDataId { get; set; }

        public int? RegistrationDateConditionId { get; set; }

        public DateTime? RegistrationTime { get; set; }

        public int? Birthday { get; set; }

        public bool? IsMale { get; set; }

        public Guid? TagId { get; set; }

        public Guid? StoreId { get; set; }

        public virtual CustomerSegment CustomerSegment { get; set; }
    }
}
