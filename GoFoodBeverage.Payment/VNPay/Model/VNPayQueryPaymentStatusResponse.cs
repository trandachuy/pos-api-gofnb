using Newtonsoft.Json.Linq;

namespace GoFoodBeverage.Payment.VNPay.Model
{
    public class VNPayQueryPaymentStatusResponse
    {
        public VNPayQueryPaymentStatusResponse() { }

        public VNPayQueryPaymentStatusResponse(string json)
        {
            JObject jObject = JObject.Parse(json);
            Amount = (string)jObject["vnp_Amount"];
            BankCode = (string)jObject["vnp_BankCode"];
            Message = (string)jObject["vnp_Message"];
            OrderInfo = (string)jObject["vnp_OrderInfo"];
            PayDate = (string)jObject["vnp_PayDate"];
            ResponseCode = (string)jObject["vnp_ResponseCode"];
            TerminalId = (string)jObject["vnp_TmnCode"];
            TransactionNo = (string)jObject["vnp_TransactionNo"];
            TransactionStatus = (string)jObject["vnp_TransactionStatus"];
            TransactionType = (string)jObject["vnp_TransactionType"];
            OrderId = (string)jObject["vnp_TxnRef"];
            SecureHash = (string)jObject["vnp_SecureHash"];
        }

        public string Amount { get; set; }

        public string BankCode { get; set; }

        public string Message { get; set; }

        public string OrderInfo { get; set; }

        public string PayDate { get; set; }

        public string ResponseCode { get; set; }

        public string TerminalId { get; set; }

        public string TransactionNo { get; set; }

        public string TransactionStatus { get; set; }

        public string TransactionType { get; set; }

        public string OrderId { get; set; }

        public string SecureHash { get; set; }
    }
}
