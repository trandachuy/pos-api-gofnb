using AzureFunctionTrigger.Constants;
using AzureFunctionTrigger.Helpers;
using AzureFunctionTrigger.Services.Interfaces;

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

/// <summary>
/// Update local.setting.json
/// Required: "az login" to authen before fetch-app-settings
/// Power shell: func azure functionapp fetch-app-settings dev-gofnb-CheckBranchesExpirationTime-fnc
/// </summary>
namespace AzureFunctionTrigger
{
    public class CheckBranchesExpirationTimeFunction
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenService _tokenService;

        public CheckBranchesExpirationTimeFunction(HttpClient httpClient, ITokenService tokenService)
        {
            _httpClient = httpClient;
            _tokenService = tokenService;
        }

        [FunctionName("CheckBranchesExpirationTime")]
        public async Task RunAsync([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer, ILogger log) // 5 mins
        //public async Task RunAsync([TimerTrigger("*/5 * * * * *")]TimerInfo myTimer, ILogger log) // 5 seconds
        {
            if (myTimer.IsPastDue)
            {
                log.LogInformation("Timer is running late!");
            }

            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            var internalToolAccessToken = await CheckBranchExpirationAsync();
            log.LogInformation($"CheckBranchExpiration status >> {internalToolAccessToken.StatusCode}");
        }

        private async Task<HttpResponseMessage> CheckBranchExpirationAsync()
        {
            var host = CommonHelper.GetEnvironmentVariable(VariableConstants.HOST);
            var endpoint = "api/store/check-branch-expiration";
            var uri = $"{host}/{endpoint}";

            var internalToolAccessToken = await _tokenService.GetInternalToolAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", internalToolAccessToken);
            HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync(uri);

            return httpResponseMessage;
        }
    }
}
