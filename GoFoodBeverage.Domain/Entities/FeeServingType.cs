using GoFoodBeverage.Domain.Base;
using GoFoodBeverage.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(FeeServingType))]
    public class FeeServingType : BaseEntity
    {
        public Guid FeeId { get; set; }

        public Guid? StoreId { get; set; }

        public EnumOrderType OrderServingType { get; set; }

        public virtual Fee Fee { get; set; }
    }
}