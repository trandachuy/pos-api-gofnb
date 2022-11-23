using GoFoodBeverage.Common.AutoWire;
using GoFoodBeverage.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace GoFoodBeverage.Services.Store
{
    [AutoService(typeof(IProductService), Lifetime = ServiceLifetime.Scoped)]
    public class ProductService : IProductService
    {
        public ProductService()
        {
        }

        public string BuildProductName(string productName, string priceName)
        {
            if (string.IsNullOrWhiteSpace(priceName))
            {
                return productName;
            }

            return $"{productName} ({priceName})";
        }
    }
}
