using System;
using System.Collections.Generic;

namespace GoFoodBeverage.Models.Staff
{
    public class StaffGroupPermissionModel
    {
        public Guid StaffId { get; set; }

        public IList<GroupPermissionDto> GroupPermissions { get; set; }
        public class GroupPermissionDto
        {
            public Guid Id { get; set; }

            public string Name { get; set; }
        }

        public IList<BranchDto> Branches { get; set; }
        public class BranchDto
        {
            public Guid Id { get; set; }

            public string Name { get; set; }
        }
    }

}
