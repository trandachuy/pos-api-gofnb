using AzureFunctionTrigger.Constants;
using AzureFunctionTrigger.Services.Interfaces;

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunctionTrigger
{
    public class SendEmailCampaignFunction
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenService _tokenService;

        public SendEmailCampaignFunction(HttpClient httpClient, ITokenService tokenService)
        {
            _httpClient = httpClient;
            _tokenService = tokenService;
        }

        [FunctionName("SendEmailCampaignFunction")]
        public async Task RunAsync([TimerTrigger("0 */15 * * * *")] TimerInfo myTimer, ILogger log)
        {
            if (myTimer.IsPastDue)
            {
                log.LogInformation("Timer is running late!");
            }

            log.LogInformation($"C# SendEmailCampaignFunction trigger function executed at: {DateTime.Now}");
            var status = await SendingEmailCampaignAsync();
            log.LogInformation($"SendEmailCampaignFunction status >> {status}");
        }

        private async Task<bool> SendingEmailCampaignAsync()
        {
            var host = Environment.GetEnvironmentVariable(VariableConstants.HOST, EnvironmentVariableTarget.Process);
            var endpoint = "api/emailcampaign/trigger-send-email-campaign";
            var uri = $"{host}/{endpoint}";

            var internalToolAccessToken = await _tokenService.GetInternalToolAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", internalToolAccessToken);

            var payload = new
            {
                interval = DefaultConstants.SENDING_EMAIL_CAMPAIGN_INTERVAL
            };

            var httpContent = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await _httpClient.PostAsync(uri, httpContent);

            return response.IsSuccessStatusCode;
        }
    }
}