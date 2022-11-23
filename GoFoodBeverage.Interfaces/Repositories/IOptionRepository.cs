using System;
using System.Linq;
using System.Threading.Tasks;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Models.Option;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IOptionRepository : IGenericRepository<Option>
    {
        IQueryable<Option> GetAllOptionsInStore(Guid? storeId);

        Task<Option> GetOptionDetailByIdAsync(Guid optionId, Guid? storeId);

        Task<Option> CheckExistOptionNameInStoreAsync(Guid optionId, string optionName, Guid storeId);
    }
}
