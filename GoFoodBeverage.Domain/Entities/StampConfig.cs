using GoFoodBeverage.Domain.Base;
using GoFoodBeverage.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(StampConfig))]
    public class StampConfig : BaseEntity
    {
        public Guid? StoreId { get; set; }

        public EnumStampType StampType { get; set; }

        public bool IsShowLogo { get; set; }

        public bool IsShowTime { get; set; }

        public bool IsShowNumberOfItem { get; set; }

        public bool IsShowNote { get; set; }


        public virtual Store Store { get; set; }
    }
}
