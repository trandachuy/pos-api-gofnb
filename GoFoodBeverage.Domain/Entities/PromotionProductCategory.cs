using System;
using GoFoodBeverage.Domain.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(PromotionProductCategory))]
    public class PromotionProductCategory : BaseAuditEntity
    {
        [Key]
        public Guid PromotionId { get; set; }

        [Key]
        public Guid ProductCategoryId { get; set; }

        public Guid? StoreId { get; set; }

        public virtual Promotion Promotion { get; set; }

        public virtual ProductCategory ProductCategory { get; set; }
    }
}
