using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(GroupPermissionPermission))]
    public class GroupPermissionPermission
    {
        [Key]
        public Guid GroupPermissionId { get; set; }

        [Key]
        public Guid PermissionId { get; set; }


        public virtual Permission Permission { get; set; }

        public virtual GroupPermission GroupPermission { get; set; }
    }
}
