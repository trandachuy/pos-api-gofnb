using System;
using GoFoodBeverage.Domain.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(Permission))]
    public class Permission : BaseEntity
    {
        public Guid PermissionGroupId { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }

        public PermissionGroup PermissionGroup { get; set; }

        public virtual ICollection<GroupPermissionPermission> GroupPermissionPermissions { get; set; }

        public virtual ICollection<FunctionPermission> FunctionPermissions { get; set; }
    }
}
