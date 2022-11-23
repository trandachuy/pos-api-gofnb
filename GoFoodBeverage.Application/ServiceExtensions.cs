using MediatR;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace GoFoodBeverage.Application
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
