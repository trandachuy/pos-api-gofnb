using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace GoFoodBeverage.Common.AutoWire
{
    /// <summary>
    /// Extension methods for registration of classes using the <see cref="AutoServiceAttribute"/>
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AutoWire(this IServiceCollection serviceCollection)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                AutoWire(serviceCollection, assembly);
            }

            return serviceCollection;
        }

        public static IServiceCollection AutoWire(this IServiceCollection serviceCollection, Assembly assembly)
        {
            var typesImplementingServiceAttribute = ScanForTypes(assembly);
            foreach (var tuple in typesImplementingServiceAttribute)
            {
                var type = tuple.Item1;
                var attribute = tuple.Item2;
                var serviceTypes = GetServiceTypes(type, attribute);

                foreach (var st in serviceTypes)
                {
                    var sd = new ServiceDescriptor(st, type, attribute.Lifetime);
                    serviceCollection.Add(sd);
                }
            }

            return serviceCollection;
        }

        private static Type[] GetServiceTypes(Type typeWithAttributeApplies, AutoServiceAttribute attribute)
        {
            if (attribute.ServiceTypes?.Length > 0)
            {
                return attribute.ServiceTypes;
            }

            var implementedInterfaces = typeWithAttributeApplies.GetInterfaces();
            if (implementedInterfaces?.Length > 0)
            {
                return implementedInterfaces;
            }

            return new[] { typeWithAttributeApplies };
        }

        /// <summary>
        /// Scan all services from class has register by AutoServiceAttribute
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        private static IEnumerable<(Type, AutoServiceAttribute)> ScanForTypes(Assembly assembly)
        {
            var services = from type in assembly.GetTypes()
                   where type.IsClass
                   let attr = type.GetCustomAttribute<AutoServiceAttribute>()
                   where attr != null
                   select (type, attr);

            return services;
        }
    }
}
