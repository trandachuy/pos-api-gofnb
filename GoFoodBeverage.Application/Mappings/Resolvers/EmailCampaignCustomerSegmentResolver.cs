using AutoMapper;
using GoFoodBeverage.Application.Features.EmailCampaigns.Commands;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GoFoodBeverage.Application.Mappings.Resolvers
{
    public class EmailCampaignCustomerSegmentResolver : IValueResolver<CreateEmailCampaignRequest, object, ICollection<EmailCampaignCustomerSegment>>
    {
        public ICollection<EmailCampaignCustomerSegment> Resolve(CreateEmailCampaignRequest source, object destination, ICollection<EmailCampaignCustomerSegment> destMember, ResolutionContext context)
        {
            ICollection<EmailCampaignCustomerSegment> emailCampaignCustomerSegments = new Collection<EmailCampaignCustomerSegment>();

            if (source.EmailCampaignType == EnumEmailCampaignType.SendToCustomerGroup)
            {
                List<Guid> customerSegmentIds = source.CustomerSegmentIds;
                foreach (Guid customerSegmentId in customerSegmentIds)
                {
                    emailCampaignCustomerSegments.Add(new EmailCampaignCustomerSegment()
                    {
                        StoreId = source.StoreId,
                        CustomerSegmentId = customerSegmentId,
                    });
                }
            }

            return emailCampaignCustomerSegments;
        }
    }
}
