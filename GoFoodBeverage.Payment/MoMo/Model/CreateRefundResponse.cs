using Newtonsoft.Json.Linq;

namespace GoFoodBeverage.Payment.MoMo.Model
{
    public class CreateRefundResponse
    {
        public CreateRefundResponse(string json)
        {
            JObject jObject = JObject.Parse(json);
            PartnerCode = (string)jObject["partnerCode"];
            RequestId = (string)jObject["requestId"];
            OrderId = (string)jObject["orderId"];
            Amount = (long)jObject["amount"];
            TransId = (long)jObject["transId"];
            ResultCode = (int)jObject["resultCode"];
            Message = (string)jObject["message"];
            ResponseTime = (long)jObject["responseTime"];
        }

        public string PartnerCode { get; set; }

        public string RequestId { get; set; }

        public string OrderId { get; set; }

        public long Amount { get; set; }

        public long TransId { get; set; }

        public int ResultCode { get; set; }

        /// <summary>
        /// Error description based on lang
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Time to respond payment results to partner
        /// </summary>
        public long ResponseTime { get; set; }
    }
}
