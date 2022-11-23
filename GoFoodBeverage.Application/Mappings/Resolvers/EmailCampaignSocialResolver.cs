using AutoMapper;
using GoFoodBeverage.Application.Features.EmailCampaigns.Commands;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Models.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GoFoodBeverage.Application.Mappings.Resolvers
{
    public class EmailCampaignSocialResolver : IValueResolver<CreateEmailCampaignRequest, object, ICollection<EmailCampaignSocial>>
    {
        public ICollection<EmailCampaignSocial> Resolve(CreateEmailCampaignRequest source, object destination, ICollection<EmailCampaignSocial> destMember, ResolutionContext context)
        {
            ICollection<EmailCampaignSocial> result = new Collection<EmailCampaignSocial>();
            List<EnumDetailModel> listEnumEmailCampaignSocial = GetListOfEnumEmailCampaignSocial();

            foreach (var social in listEnumEmailCampaignSocial)
            {
                var emailCampaignSocialItem = source.EmailCampaignSocials.SingleOrDefault(emailCampaignSocial => (int)emailCampaignSocial.EnumEmailCampaignSocialId == social.Id);
                if (emailCampaignSocialItem != null)
                {
                    result.Add(new EmailCampaignSocial()
                    {
                        IsActive = true,
                        Name = social.Name,
                        StoreId = source.StoreId,
                        Url = emailCampaignSocialItem.Url
                    });
                }
                else
                {
                    result.Add(new EmailCampaignSocial()
                    {
                        IsActive = false,
                        Name = social.Name,
                        StoreId = source.StoreId
                    });
                }
            }

            return result;
        }

        private List<EnumDetailModel> GetListOfEnumEmailCampaignSocial()
        {
            List<EnumDetailModel> enumListEmailCampaignSocial = Enum.GetValues(typeof(EnumEmailCampaignSocial))
                .Cast<EnumEmailCampaignSocial>()
                .Select(emailCampaignSocial => new EnumDetailModel()
                {
                    Id = (int)emailCampaignSocial,
                    Name = emailCampaignSocial.ToString()
                })
                .ToList();

            return enumListEmailCampaignSocial;
        }
    }
}
