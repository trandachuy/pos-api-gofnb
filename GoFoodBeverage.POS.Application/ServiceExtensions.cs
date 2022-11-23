using MediatR;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace GoFoodBeverage.POS.Application
{
    public static class ServiceExtensions
    {
        public static void AddApplicationLayer(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());
        }
    }
}
