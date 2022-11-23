using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces.Repositories;
using System.Linq;
using System;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class PurchaseOrderMaterialRepostiory : GenericRepository<PurchaseOrderMaterial>, IPurchaseOrderMaterialRepostiory
    {
        public PurchaseOrderMaterialRepostiory(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public IQueryable<PurchaseOrderMaterial> GetPurchaseOrderMaterialByPurchaseOrderId(Guid purchaseOrderId, Guid? storeId)
        {
            var purchaseOrderMaterials = dbSet.Where(s => s.StoreId == storeId && s.PurchaseOrderId == purchaseOrderId);

            return purchaseOrderMaterials;
        }
    }
}
