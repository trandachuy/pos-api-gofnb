using System;
using System.Collections.Generic;

namespace GoFoodBeverage.Models.Staff
{
    public class StaffByIdModel
    {
        public Guid Id { get; set; }

        public Guid StaffId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public bool Gender { get; set; }

        public DateTime? Birthday { get; set; }

        public List<GroupPermissionBranchesDto> PermissionGroupControls { get; set; }

        public class GroupPermissionBranchesDto
        {
            public Guid? BranchId { get; set; }

            public List<Guid> BranchIds { get; set; }

            public List<Guid> GroupPermissionIds { get; set; }
        }

    }
}
