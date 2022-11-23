using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IStaffRepository : IGenericRepository<Staff>
    {
        IQueryable<Staff> GetAllStaffInStore(Guid storeId);

        Task<Staff> GetStaffByIdAsync(Guid staffId);

        Task<Staff> GetStaffByAccountIdAsync(Guid accountId);

        Task<Staff> GetStaffByIdForEditAsync(Guid staffId, Guid? storeId);

        Task<Staff> CheckExistStaffCodeInStoreAsync(Guid staffId, string staffCode, Guid storeId);

        IQueryable<Staff> GetAllStaffByInitialStoreAccounts(List<Guid> initialStoreAccountIds);

        IQueryable<Staff> GetAllStaffByInitialStoreAccount(Guid storeAccountId);

        IQueryable<Staff> GetAllStaffByListStaffId(List<Guid?> listStaffId);

        IQueryable<Staff> GetAllStaffByListStaff(IEnumerable<Guid> listStaffId, Guid? storeId);

        IQueryable<Staff> GetStaffByShift(Shift shift, Guid? storeId);

        IQueryable<Staff> GetStaffById(Guid staffId, Guid? storeId);

        IQueryable<Staff> GetStaffByAccountId(Guid accountId);
    }
}
