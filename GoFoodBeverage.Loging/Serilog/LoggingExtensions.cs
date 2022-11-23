using System;
using Serilog;
using Serilog.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GoFoodBeverage.Loging.Serilog
{
    public static class LoggingExtensions
    {
        public static LoggerConfiguration WithHttpContext(this LoggerEnrichmentConfiguration enrichConfiguration, IServiceProvider serviceProvider)
        {
            if (enrichConfiguration is null) throw new ArgumentNullException(nameof(enrichConfiguration));

            var enricher = serviceProvider.GetService<HttpContextEnricher>();
            var enrichConfig = enrichConfiguration.With(enricher);

            return enrichConfig;
        }
    }
}
