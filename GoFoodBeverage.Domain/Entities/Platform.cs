using GoFoodBeverage.Domain.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(Platform))]
    public class Platform : BaseEntity
    {
        public string Name { get; set; }

        public int StatusId { get; set; }

        public virtual ICollection<ProductPlatform> ProductPlatforms { get; set; }
    }
}
