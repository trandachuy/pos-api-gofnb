using GoFoodBeverage.Domain.Entities;
using System;
using System.Linq;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IPurchaseOrderMaterialRepostiory : IGenericRepository<PurchaseOrderMaterial>
    {
        IQueryable<PurchaseOrderMaterial> GetPurchaseOrderMaterialByPurchaseOrderId(Guid purchaseOrderId, Guid? storeId);
    }
}
