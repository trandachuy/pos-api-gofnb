
namespace GoFoodBeverage.Payment.MoMo.Model
{
    public class CreatePosGatewayRequest
    {
        /// <summary>
        /// Required
        /// </summary>
        public string PartnerCode { get; set; }

        public string OrderId { get; set; }

        public long Amount { get; set; }

        public string RequestId { get; set; }

        public string PaymentCode { get; set; }

        public string OrderInfo { get; set; }

        public string ExtraData { get; set; } = "";

        public string Lang { get; set; }


        /// <summary>
        /// Optional
        /// </summary>
        public string StoreId { get; set; }

        public string StoreName { get; set; }

        public long OrderGroupId { get; set; }

        public bool AutoCapture { get; set; } = true;

        public string IpnUrl { get; set; }
    }
}
