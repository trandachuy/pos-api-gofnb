using System;
using System.Collections.Generic;

namespace GoFoodBeverage.Models.Staff
{
    public class StaffGroupPermissionBranchRequestModel
    {
        public IList<Guid> BranchIds { get; set; }

        public IList<Guid> GroupPermissionIds { get; set; }
    }
}
