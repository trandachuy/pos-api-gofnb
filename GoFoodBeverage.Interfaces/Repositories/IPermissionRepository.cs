using System;
using System.Linq;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IPermissionRepository : IGenericRepository<Permission>
    {
        IQueryable<Permission> GetPermissionsByPermissionGroupId(Guid permissionGroupId);
    }
}
