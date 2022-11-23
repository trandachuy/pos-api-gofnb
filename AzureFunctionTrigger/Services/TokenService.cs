using AzureFunctionTrigger.Constants;
using AzureFunctionTrigger.Helpers;
using AzureFunctionTrigger.Models;
using AzureFunctionTrigger.Services.Interfaces;

using Newtonsoft.Json;

using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunctionTrigger.Services
{
    public class TokenService : ITokenService
    {
        private readonly HttpClient _httpClient;

        public TokenService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetInternalToolAccessTokenAsync()
        {
            var internalAuthenticateModel = new InternalAuthenticateRequestModel()
            {
                UserName = CommonHelper.GetEnvironmentVariable(VariableConstants.USERNAME),
                Password = CommonHelper.GetEnvironmentVariable(VariableConstants.PASSWORD),
            };

            var host = CommonHelper.GetEnvironmentVariable(VariableConstants.HOST);
            var internalAuthenEnpoint = "api/login/internal-tool";
            var uri = $"{host}/{internalAuthenEnpoint}";

            var httpContent = new StringContent(JsonConvert.SerializeObject(internalAuthenticateModel), Encoding.UTF8);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage httpResponseMessage = await _httpClient.PostAsync(uri, httpContent);
            var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
            var internalAuthenticateResponse = JsonConvert.DeserializeObject<InternalAuthenticateResponseModel>(responseContent);

            return internalAuthenticateResponse.Token;
        }
    }
}
