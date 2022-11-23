using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Interfaces.Repositories;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class PromotionRepository : GenericRepository<Promotion>, IPromotionRepository
    {
        public PromotionRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public Task<Promotion> GetPromotionByNameInStoreAsync(string promotionName, Guid storeId)
        {
            var product = dbSet.FirstOrDefaultAsync(p => p.Name.ToLower().Contains(promotionName.ToLower()) && p.StoreId == storeId);

            return product;
        }

        public IQueryable<Promotion> GetAllPromotionInStore(Guid storeId)
        {
            var promotionQuery = dbSet.Where(s => s.StoreId == storeId);

            return promotionQuery;
        }

        /// <summary>
        /// Get Promotion by Id and store Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="storeId"></param>
        /// <returns></returns>
        public async Task<Promotion> GetPromotionByIdInStoreAsync(Guid id, Guid storeId)
        {
            var promotion = await dbSet.Where(m => m.Id == id && m.StoreId == storeId)
                // Include if needed
                .Include(p => p.PromotionBranches)
                    .ThenInclude(pb => pb.Branch)
                .Include(p => p.PromotionProducts)
                    .ThenInclude(pp => pp.Product)
                .Include(p => p.PromotionProductCategories)
                    .ThenInclude(ppc => ppc.ProductCategory)
                .FirstOrDefaultAsync();

            return promotion;
        }
    }
}
