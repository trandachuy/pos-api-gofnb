using Microsoft.AspNetCore.Http;

namespace GoFoodBeverage.Common.Providers.DomainUrl
{
    public class DomainUrlProvider : IDomainUrlProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DomainUrlProvider(
            IHttpContextAccessor httpContextAccessor
        )
        {
            _httpContextAccessor = httpContextAccessor;
        }


        public string GetCurrentRootDomainFromRequest() => $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
    }
}
