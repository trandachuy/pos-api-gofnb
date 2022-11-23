using Newtonsoft.Json.Linq;

namespace GoFoodBeverage.Payment.MoMo.Model
{
    public class QueryStatusResponseModel
    {
        public QueryStatusResponseModel(string json)
        {
            JObject jObject = JObject.Parse(json);
            PartnerCode = (string)jObject["partnerCode"];
            RequestId = (string)jObject["requestId"];
            OrderId = (string)jObject["orderId"];
            ExtraData = (string)jObject["extraData"];
            Amount = (int)jObject["amount"];
            TransId = (long)jObject["transId"];
            PayType = (string)jObject["payType"];
            ResultCode = (int)jObject["resultCode"];
            Message = (string)jObject["message"];
            ResponseTime = (long)jObject["responseTime"];
        }

        public bool IsSuccess { get { return ResultCode == 0; } }

        public string PartnerCode { get; set; }

        public string RequestId { get; set; }

        public string OrderId { get; set; }

        public string ExtraData { get; set; }

        public int Amount { get; set; }

        public long TransId { get; set; }

        public string PayType { get; set; }

        public int ResultCode { get; set; }

        //public string RefundTrans { get; set; }

        public string Message { get; set; }

        public long ResponseTime { get; set; }
    }
}
