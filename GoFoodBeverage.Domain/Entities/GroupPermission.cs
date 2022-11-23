using GoFoodBeverage.Domain.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    /// <summary>
    /// Description: This is group as role permission.
    /// Example: Admin, staff, manager, etc,..
    /// </summary>
    [Table(nameof(GroupPermission))]
    public class GroupPermission : BaseEntity
    {
        /// Not reference to staff.
        public Guid? CreatedByStaffId { get; set; }

        public Guid? StoreId { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }

        public Store Store { get; set; }

        public virtual ICollection<GroupPermissionPermission> GroupPermissionPermissions { get; set; }

        public virtual ICollection<GroupPermissionBranch> GroupPermissionBranches { get; set; }
    }
}
