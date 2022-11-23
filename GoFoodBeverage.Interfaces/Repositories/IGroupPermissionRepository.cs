using System;
using System.Linq;
using System.Threading.Tasks;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IGroupPermissionRepository : IGenericRepository<GroupPermission>
    {
        IQueryable<GroupPermission> GetGroupPermissionsByStoreId(Guid? storeId);

        /// <summary>
        /// Get group permission by Id and store Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="storeId"></param>
        /// <returns>GroupPermission object</returns>
        Task<GroupPermission> GetGroupPermissionByIdInStoreAsync(Guid groupPermissionId, Guid storeId);
    }
}
