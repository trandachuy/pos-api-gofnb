using GoFoodBeverage.Domain.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(StaffGroupPermissionBranch))]
    public class StaffGroupPermissionBranch : BaseEntity
    {
        public Guid StaffId { get; set; }

        public Guid? StoreId { get; set; }


        public virtual Staff Staff { get; set; }

        public virtual ICollection<GroupPermissionBranch> GroupPermissionBranches { get; set; } 
    }
}
