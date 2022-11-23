namespace GoFoodBeverage.Payment.MoMo.Model
{
    /// <summary>
    /// Reverse & Refund
    /// Docs: https://developers.momo.vn/v3/vi/docs/payment/api/payment-api/refund/
    /// </summary>
    public class CreateRefundRequest
    {
        /// <summary>
        /// Request ID, unique for each request, MoMo's partner uses the requestId field for idempotency control
        /// </summary>
        public string RequestId { get; set; }

        public string OrderId { get; set; }

        /// <summary>
        /// Amount to be refunded
        /// </summary>
        public long Amount { get; set; }

        /// <summary>
        /// This ID is provided by MoMo when the purchase transaction is successful
        /// </summary>
        public long TransId { get; set; }

        /// <summary>
        /// Language of returned message (vi or en)
        /// </summary>
        public string Lang { get; set; }

        /// <summary>
        /// Description of refund request
        /// </summary>
        public string Description { get; set; }
    }
}
