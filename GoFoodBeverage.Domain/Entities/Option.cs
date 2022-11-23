using GoFoodBeverage.Domain.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(Option))]
    public class Option : BaseEntity
    {
        public Guid? StoreId { get; set; }

        public Guid? MaterialId { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(15)]
        public string Code { get; set; }

        public virtual Material Material { get; set; }

        public virtual Store Store { get; set; }

        public virtual ICollection<OptionLevel> OptionLevel { get; set; }

        public virtual ICollection<ProductOption> ProductOptions { get; set; }
    }
}
