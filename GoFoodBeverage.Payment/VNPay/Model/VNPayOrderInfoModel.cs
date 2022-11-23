using System;

namespace GoFoodBeverage.Payment.VNPay.Model
{
    public class VNPayOrderInfoModel
    {
        public long OrderId { get; set; }

        public string Title { get; set; }

        public long Amount { get; set; }

        public string OrderDesc { get; set; }

        public DateTime CreatedDate { get; set; }

        public string VnPayCreateDate { get; set; }

        public string Status { get; set; }

        public long PaymentTranId { get; set; }

        public string BankCode { get; set; }

        public string PayStatus { get; set; }

        public string CurrencyCode { get; set; }
    }
}
