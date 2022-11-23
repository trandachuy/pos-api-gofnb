using System;
using System.Collections.Generic;

namespace GoFoodBeverage.Models.PurchaseOrderModel
{
    public class GetGroupPermissionByIdModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public virtual IEnumerable<GroupPermissionPermission> GroupPermissionPermissions { get; set; }

        public class GroupPermissionPermission
        {
            public Guid PermissionId { get; set; }
        }
    }
}
