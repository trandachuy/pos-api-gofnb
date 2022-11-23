using GoFoodBeverage.Domain.Base;
using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(UnitConversion))]
    public class UnitConversion: BaseEntity
    {
        public Guid? StoreId { get; set; }

        public Guid? UnitId { get; set; }

        public int Capacity { get; set; } /// Value allowed from 0 to 999,999,999

        public Guid? MaterialId { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public virtual Unit Unit { get; set; }

        public virtual Material Material { get; set; }
    }
}
