using AzureFunctionTrigger.Services;
using AzureFunctionTrigger.Services.Interfaces;

using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(AzureFunctionTrigger.Startup))]

namespace AzureFunctionTrigger
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();
            builder.Services.AddScoped<ITokenService, TokenService>();
        }
    }
}
