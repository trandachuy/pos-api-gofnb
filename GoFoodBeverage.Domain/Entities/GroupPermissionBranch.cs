using GoFoodBeverage.Domain.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table("GroupPermissionBranches")]
    public class GroupPermissionBranch : BaseEntity
    {
        public Guid StaffGroupPermissionBranchId { get; set; }

        public Guid? StoreBranchId { get; set; } // Null when IsApplyAllBranch is True

        public Guid GroupPermissionId { get; set; }

        public bool IsApplyAllBranch { get; set; }

        public Guid? StoreId { get; set; }

        public virtual StoreBranch StoreBranch { get; set; }

        public virtual StaffGroupPermissionBranch StaffGroupPermissionBranch { get; set; }

        public virtual GroupPermission GroupPermission { get; set; }
    }
}
