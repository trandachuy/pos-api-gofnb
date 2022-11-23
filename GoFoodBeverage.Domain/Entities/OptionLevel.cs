using GoFoodBeverage.Domain.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(OptionLevel))]
    public class OptionLevel : BaseEntity
    {
        public Guid OptionId { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        public decimal? Quota { get; set; }

        public bool? IsSetDefault { get; set; }

        public Guid? StoreId { get; set; }

        public virtual Option Option { get; set; }
    }
}
