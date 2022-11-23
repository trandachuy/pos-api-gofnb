using GoFoodBeverage.Domain.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(ComboStoreBranch))]
    public class ComboStoreBranch : BaseAuditEntity
    {
        [Key]
        public Guid ComboId { get; set; }

        [Key]
        public Guid BranchId { get; set; }

        public Guid? StoreId { get; set; }

        public virtual Combo Combo { get; set; }

        public virtual StoreBranch Branch { get; set; }
    }
}
