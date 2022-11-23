using GoFoodBeverage.Domain.Entities;
using System;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IGroupPermissionPermissionRepository : IGenericRepository<GroupPermissionPermission>
    {
        void DeleteByGroupPermissionId(Guid id);
    }
}
