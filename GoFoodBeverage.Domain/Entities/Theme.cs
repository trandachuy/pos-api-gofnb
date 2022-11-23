using System;
using GoFoodBeverage.Domain.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(Theme))]
    public class Theme : BaseEntity
    {
        public string Name { get; set; }

        public string Tags { get; set; }

        public string Description { get; set; }

        public string Thumbnail { get; set; }

        public virtual ICollection<StoreTheme> StoreThemes { get; set; }
    }
}
