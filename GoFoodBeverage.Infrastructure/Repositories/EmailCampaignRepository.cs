using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Interfaces.Repositories;

using Microsoft.EntityFrameworkCore;

using System;
using System.Linq;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class EmailCampaignRepository : GenericRepository<EmailCampaign>, IEmailCampaignRepostiory
    {
        public EmailCampaignRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }

        public IQueryable<EmailCampaign> GetAllByLastMinutes(double minutes)
        {
            var utcNow = DateTime.UtcNow;
            var lastMinutes = utcNow.AddMinutes(-1 * minutes);

            var emailCampaigns = dbSet
                .Include(x => x.EmailCampaignCustomerSegments)
                .ThenInclude(x=>x.CustomerSegment)
                .ThenInclude(x=>x.CustomerCustomerSegments)
                .ThenInclude(x=>x.Customer)
                .Include(x=>x.EmailCampaignSendingTransactions)
                .Where(s => lastMinutes <= s.SendingTime && s.SendingTime <= utcNow);

            return emailCampaigns;
        }

        public IQueryable<EmailCampaign> GetAllEmailCampaignInStore(Guid storeId)
        {
            var emailCampaigns = dbSet.Where(s => s.StoreId == storeId);

            return emailCampaigns;
        }
    }
}
