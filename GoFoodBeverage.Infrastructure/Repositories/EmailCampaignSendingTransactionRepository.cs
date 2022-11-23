using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Interfaces.Repositories;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class EmailCampaignSendingTransactionRepository : GenericRepository<EmailCampaignSendingTransaction>, IEmailCampaignSendingTransactionRepostiory
    {
        public EmailCampaignSendingTransactionRepository(GoFoodBeverageDbContext dbContext) : base(dbContext)
        {
        }
    }
}