using System;
using Microsoft.Extensions.DependencyInjection;

namespace GoFoodBeverage.Common.AutoWire
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class AutoServiceAttribute : Attribute
    {
        public Type[] ServiceTypes { get; }
        public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Singleton;

        public AutoServiceAttribute()
        {
            ServiceTypes = Array.Empty<Type>();
        }

        public AutoServiceAttribute(params Type[] serviceTypes)
        {
            ServiceTypes = serviceTypes ?? Array.Empty<Type>();
        }
    }
}
