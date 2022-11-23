using System;
using System.Linq;
using System.Threading.Tasks;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IBillRepository : IGenericRepository<BillConfiguration>
    {
        public IQueryable<BillConfiguration> GetBillConfigurationByFrameSize(EnumBillFrameSize size, Guid? storeId);

        public IQueryable<BillConfiguration> GetDefaultBillConfigurationByStore(Guid? storeId);
    }
}
