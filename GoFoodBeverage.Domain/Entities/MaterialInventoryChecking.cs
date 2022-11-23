using System;
using GoFoodBeverage.Domain.Base;
using System.ComponentModel.DataAnnotations;

namespace GoFoodBeverage.Domain.Entities
{
    public class MaterialInventoryChecking : BaseEntity
    {
        public Guid? StoreId { get; set; }

        public Guid? BranchId { get; set; }

        public Guid? StaffId { get; set; }

        public Guid? ShiftId { get; set; }

        public Guid? MaterialId { get; set; }

        public int OriginalQuantity { get; set; }

        public int InventoryQuantity { get; set; }

        [MaxLength(255)]
        public string Reason { get; set; }


        public virtual Store Store { get; set; }

        public virtual StoreBranch Branch { get; set; }

        public virtual Staff Staff { get; set; }

        public virtual Shift Shift { get; set; }

        public virtual Material Material { get; set; }
    }
}
