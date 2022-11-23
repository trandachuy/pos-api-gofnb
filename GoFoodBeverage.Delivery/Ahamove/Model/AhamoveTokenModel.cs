using Newtonsoft.Json;

namespace GoFoodBeverage.Delivery.Ahamove.Model
{
    public class AhamoveTokenModel
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
    }
}
