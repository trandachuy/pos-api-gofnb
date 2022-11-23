using System;

namespace GoFoodBeverage.Models.Permission
{
    public class PermissionModel
    {
        public Guid Id { get; set; }

        public Guid PermissionGroupId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}
