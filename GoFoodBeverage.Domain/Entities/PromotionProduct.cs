using System;
using GoFoodBeverage.Domain.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(PromotionProduct))]
    public class PromotionProduct : BaseAuditEntity
    {
        [Key]
        public Guid PromotionId { get; set; }

        [Key]
        public Guid ProductId { get; set; }

        public Guid? StoreId { get; set; }

        public virtual Promotion Promotion { get; set; }

        public virtual Product Product { get; set; }
    }
}
