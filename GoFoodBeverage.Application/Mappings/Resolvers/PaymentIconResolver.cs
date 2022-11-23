using AutoMapper;
using GoFoodBeverage.Common.Providers.DomainUrl;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Models.Payment;

namespace GoFoodBeverage.Application.Mappings.Resolvers
{
    public class PaymentIconResolver : IValueResolver<PaymentMethod, object, string>
    {
        private IDomainUrlProvider _domainUrlProvider;

        public PaymentIconResolver(IDomainUrlProvider domainUrlProvider)
        {
            _domainUrlProvider = domainUrlProvider;
        }

        public string Resolve(PaymentMethod source, object destination, string destMember, ResolutionContext context)
        {
            if (source != null && !string.IsNullOrWhiteSpace(source.Icon))
            {
                try
                {
                    string logoStoragePath = $"{_domainUrlProvider.GetCurrentRootDomainFromRequest()}/{source.Icon}";
                    return logoStoragePath;
                }
                catch {}
            }

            return string.Empty;
        }
    }
}
