using GoFoodBeverage.Domain.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(MaterialInventoryBranch))]
    public class MaterialInventoryBranch : BaseEntity
    {
        public Guid? StoreId { get; set; }

        public Guid? BranchId { get; set; }

        public Guid? MaterialId { get; set; }

        public int Position { get; set; }

        public int Quantity { get; set; }

        public virtual Store Store { get; set; }

        public virtual StoreBranch Branch { get; set; }

        public virtual Material Material { get; set; }

        public virtual ICollection<MaterialInventoryHistory> MaterialInventoryHistories { get; set; }
    }
}
