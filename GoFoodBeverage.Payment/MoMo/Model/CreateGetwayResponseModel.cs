using Newtonsoft.Json.Linq;

namespace GoFoodBeverage.Payment.MoMo.Model
{
    public class CreateGetwayResponseModel
    {
        public CreateGetwayResponseModel(string json)
        {
            JObject jObject = JObject.Parse(json);
            PartnerCode = (string)jObject["partnerCode"];
            OrderId = (string)jObject["orderId"];
            RequestId = (string)jObject["requestId"];
            Amount = (long)jObject["amount"];
            ResponseTime = (long)jObject["responseTime"];
            Message = (string)jObject["message"];
            ResultCode = (int)jObject["resultCode"];
            PayUrl = (string)jObject["payUrl"];
            PartnerClientId = (string)jObject["partnerClientId"];
            DeepLink = (string)jObject["deeplink"];
        }

        public string PartnerCode { get; set; }

        public string OrderId { get; set; }

        public string RequestId { get; set; }

        public long Amount { get; set; }

        public long ResponseTime { get; set; }

        public string Message { get; set; }

        public int ResultCode { get; set; }

        public string PayUrl { get; set; }

        public string PartnerClientId { get; set; }

        public string DeepLink { get; set; }
    }
}
