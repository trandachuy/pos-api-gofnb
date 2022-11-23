using System;

namespace GoFoodBeverage.Common.Models.Base
{
    public class BaseAuditModel
    {
        public Guid? LastSavedUser { get; set; }

        public DateTime? LastSavedTime { get; set; }

        public Guid? CreatedUser { get; set; }

        public DateTime? CreatedTime { get; set; }
    }
}
