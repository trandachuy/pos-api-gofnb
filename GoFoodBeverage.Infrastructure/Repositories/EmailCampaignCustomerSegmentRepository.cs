using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Interfaces.Repositories;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    public class EmailCampaignCustomerSegmentRepository : GenericRepository<EmailCampaignCustomerSegment>, IEmailCampaignCustomerSegmentRepostiory
    {
        public EmailCampaignCustomerSegmentRepository(GoFoodBeverageDbContext dbContext) : base(dbContext) { }
    }
}
