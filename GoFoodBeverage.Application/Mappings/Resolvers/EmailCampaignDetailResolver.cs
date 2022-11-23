using AutoMapper;
using GoFoodBeverage.Application.Features.EmailCampaigns.Commands;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Models.EmailCampaign;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GoFoodBeverage.Application.Mappings.Resolvers
{
    public class EmailCampaignDetailResolver : IValueResolver<CreateEmailCampaignRequest, object, ICollection<EmailCampaignDetail>>
    {
        public ICollection<EmailCampaignDetail> Resolve(CreateEmailCampaignRequest source, object destination, ICollection<EmailCampaignDetail> destMember, ResolutionContext context)
        {
            ICollection<EmailCampaignDetail> emailCampaignDetailsResult = new Collection<EmailCampaignDetail>();
            List<EmailCampaignDetailModel> emailCampaignDetails = source.EmailCampaignDetails;

            foreach (var emailCampaignDetail in emailCampaignDetails)
            {
                emailCampaignDetailsResult.Add(new EmailCampaignDetail()
                {
                    StoreId = source.StoreId,
                    ButtonLink = emailCampaignDetail.ButtonUrl,
                    Position = emailCampaignDetail.Position,
                    Thumbnail = emailCampaignDetail.ImageUrl,
                    Title = emailCampaignDetail.Title,
                    Description = emailCampaignDetail.Description,
                    ButtonName = emailCampaignDetail.ButtonName
                });
            }

            return emailCampaignDetailsResult;
        }
    }
}
