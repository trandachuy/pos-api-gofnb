using System;
using GoFoodBeverage.Domain.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(FeeBranch))]
    public class FeeBranch : BaseAuditEntity
    {
        [Key]
        public Guid FeeId { get; set; }

        [Key]
        public Guid BranchId { get; set; }

        public Guid? StoreId { get; set; }

        public virtual Fee Fee { get; set; }

        public virtual StoreBranch Branch { get; set; }
    }
}
