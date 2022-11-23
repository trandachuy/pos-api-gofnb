using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(ProductPlatform))]
    public class ProductPlatform
    {
        [Key]
        public Guid ProductId { get; set; }

        [Key]
        public Guid PlatformId { get; set; }

        public Guid? StoreId { get; set; }

        public virtual Product Product { get; set; }

        public virtual Platform Platform { get; set; }
    }
}
