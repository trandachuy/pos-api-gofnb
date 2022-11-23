using GoFoodBeverage.Domain.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(PromotionBranch))]
    public class PromotionBranch : BaseAuditEntity
    {
        [Key]
        public Guid PromotionId { get; set; }

        [Key]
        public Guid BranchId { get; set; }

        public Guid? StoreId { get; set; }

        public virtual Promotion Promotion { get; set; }

        public virtual StoreBranch Branch { get; set; }
    }
}
