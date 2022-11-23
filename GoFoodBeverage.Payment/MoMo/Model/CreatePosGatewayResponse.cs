using Newtonsoft.Json.Linq;

namespace GoFoodBeverage.Payment.MoMo.Model
{
    public class CreatePosGatewayResponse
    {
        public CreatePosGatewayResponse()
        {
        }

        public CreatePosGatewayResponse(string json)
        {
            JObject jObject = JObject.Parse(json);
            PartnerCode = (string)jObject["partnerCode"];
            OrderId = (string)jObject["orderId"];
            RequestId = (string)jObject["requestId"];
            Amount = (int)jObject["amount"];
            TransId = (long)jObject["transId"];
            ResponseTime = (long)jObject["responseTime"];
            ResultCode = (int)jObject["resultCode"];
            Message = (string)jObject["message"];
        }
        
        public string PartnerCode { get; set; }
        public string OrderId { get; set; }
        public string RequestId { get; set; }
        public long Amount { get; set; }
        public long TransId { get; set; }
        public long ResponseTime { get; set; }
        public int ResultCode { get; set; }
        public string Message { get; set; }
    }

    public class CreatePosResponse
    {
        public int Status { get; set; }
        public MessageDto Message { get; set; }

        public class MessageDto
        {
            public string Description { get; set; }
            public long Transid { get; set; }
            public int Amount { get; set; }
            public string PhoneNumber { get; set; }
            public string WalletId { get; set; }
        }
    }
}
