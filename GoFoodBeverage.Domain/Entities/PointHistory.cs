using GoFoodBeverage.Domain.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(PointHistory))]
    public class PointHistory : BaseEntity
    {
        public Guid? OrderId { get; set; }

        public bool IsEarning { get; set; }

        public int Change { get; set; }

        public virtual Order Order { get; set; }
    }
}
