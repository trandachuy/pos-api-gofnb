using GoFoodBeverage.Domain.Entities;
using System;
using System.Linq;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IEmailCampaignRepostiory : IGenericRepository<EmailCampaign>
    {
        IQueryable<EmailCampaign> GetAllEmailCampaignInStore(Guid storeId);

        IQueryable<EmailCampaign> GetAllByLastMinutes(double minutes);
    }
}
