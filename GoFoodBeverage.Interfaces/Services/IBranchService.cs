using GoFoodBeverage.Domain.Entities;
using System;
using System.Collections.Generic;

namespace GoFoodBeverage.Interfaces
{
    public interface IBranchService
    {
        IEnumerable<StoreBranch> GetBranches(Guid storeId, Guid? accountId);
    }
}
