using System;

namespace GoFoodBeverage.Models.Permission
{
    public class GroupPermissionModel
    {
        public Guid Id { get; set; }

        public Guid? CreatedByStaffId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string CreatedByStaffName { get; set; }

        public int NumberOfMember { get; set; }
    }
}
