using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Interfaces.Repositories;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class EmailCampaignSocialRepository : GenericRepository<EmailCampaignSocial>, IEmailCampaignSocialRepostiory
    {
        public EmailCampaignSocialRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }
    }
}
