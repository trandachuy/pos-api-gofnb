using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Models.Permission;
using GoFoodBeverage.Models.Store;
using System;
using System.Collections.Generic;

namespace GoFoodBeverage.Models.Staff
{
    public class StaffModel
    {
        public Guid Id { get; set; }

        public int No { get; set; }

        public string FullName { get; set; }

        public string PhoneNumber { get; set; }

        public virtual AccountDto Account { get; set; }

        public IEnumerable<StoreBranchModel> StoreBranches { get; set; }

        public IEnumerable<GroupPermissionModel> GroupPermissions { get; set; }

        public bool IsInitialStoreAccount { get; set; }

        public class AccountDto
        {
            public string Email { get; set; }
        }
    }
}
