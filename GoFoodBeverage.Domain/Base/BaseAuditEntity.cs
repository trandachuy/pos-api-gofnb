using System;
using System.ComponentModel.DataAnnotations;

namespace GoFoodBeverage.Domain.Base
{
    public abstract class BaseAuditEntity
    {
        public Guid? LastSavedUser { get; set; }

        public DateTime? LastSavedTime { get; set; }

        public Guid? CreatedUser { get; set; }

        public DateTime? CreatedTime { get; set; }
    }
}
