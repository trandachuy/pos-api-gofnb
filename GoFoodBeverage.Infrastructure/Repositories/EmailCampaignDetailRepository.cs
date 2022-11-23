using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Interfaces.Repositories;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class EmailCampaignDetailRepository : GenericRepository<EmailCampaignDetail>, IEmailCampaignDetailRepostiory
    {
        public EmailCampaignDetailRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }
    }
}
