using AutoMapper;
using GoFoodBeverage.Common.Providers.DomainUrl;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Application.Mappings.Resolvers
{
    public class ThemeImageResolver : IValueResolver<Theme, object, string>
    {
        private IDomainUrlProvider _domainUrlProvider;

        public ThemeImageResolver(IDomainUrlProvider domainUrlProvider)
        {
            _domainUrlProvider = domainUrlProvider;
        }

        public string Resolve(Theme source, object destination, string destMember, ResolutionContext context)
        {
            if (source != null && !string.IsNullOrWhiteSpace(source.Thumbnail))
            {
                try
                {
                    string logoStoragePath = $"{_domainUrlProvider.GetCurrentRootDomainFromRequest()}/{source.Thumbnail}";
                    return logoStoragePath;
                }
                catch { }
            }

            return string.Empty;
        }
    }
}
