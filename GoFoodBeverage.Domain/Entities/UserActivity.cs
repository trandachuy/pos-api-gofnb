using GoFoodBeverage.Domain.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(UserActivity))]
    public class UserActivity : BaseEntity
    {
        public Guid? AccountId { get; set; }

        public string ActivityName { get; set; }

        public string PreviousData { get; set; }

        public string NewData { get; set; }

        public string Platform { get; set; }

        public DateTime Time { get; set; }

        public int UsedTime { get; set; }

        public Guid? StoreId { get; set; }
    }
}
