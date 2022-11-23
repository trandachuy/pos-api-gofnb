using GoFoodBeverage.Domain.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(ProductProductCategory))]
    public class ProductProductCategory : BaseAuditEntity
    {
        [Key]
        public Guid ProductId { get; set; }

        [Key]
        public Guid ProductCategoryId { get; set; }

        public int Position { get; set; } = 0;

        public Guid? StoreId { get; set; }

        public virtual Product Product { get; set; }

        public virtual ProductCategory ProductCategory { get; set; }
    }
}
