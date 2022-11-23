using GoFoodBeverage.Payment.VNPay.Model;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace GoFoodBeverage.Payment.VNPay
{
    /// <summary>
    /// Status code VNPAY: https://sandbox.vnpayment.vn/apis/docs/thanh-toan-pay/pay.html#Bang-ma-loi-PAY
    /// </summary>
    public interface IVNPayService
    {
        /// <summary>
        /// Create VNPAY payment url
        /// Docs: https://sandbox.vnpayment.vn/apis/docs/thanh-toan-pay/pay.html#tao-url-thanh-toan
        /// </summary>
        /// <param name="config"></param>
        /// <param name="order"></param>
        /// <param name="locale"></param>
        /// <param name="returnUrl"></param>
        /// <returns>string: payment url</returns>
        Task<string> CreatePaymentUrlAsync(
            VNPayConfigModel config,
            VNPayOrderInfoModel order,
            string locale,
            string returnUrl);


        /// <summary>
        /// Query payment status
        /// Docs: https://sandbox.vnpayment.vn/apis/docs/thanh-toan-pay/pay.html#truy-van-ket-qua-thanh-toan-PAY
        /// </summary>
        /// <param name="config"></param>
        /// <param name="orderId"></param>
        /// <param name="orderInfo"></param>
        /// <param name="transDate"></param>
        /// <param name="createDate"></param>
        /// <returns></returns>
        public Task<VNPayQueryPaymentStatusResponse> QueryAsync(VNPayConfigModel config,
            string orderId,
            string orderInfo,
            string transDate,
            string createDate);

        /// <summary>
        /// Refund
        /// Docs: https://sandbox.vnpayment.vn/apis/docs/thanh-toan-pay/pay.html#hoan-tien-thanh-toan-PAY
        /// </summary>
        /// <param name="config"></param>
        /// <param name="orderId"></param>
        /// <param name="amount"></param>
        /// <param name="orderInfo"></param>
        /// <param name="transDate"></param>
        /// <param name="transactionType"></param>
        /// <param name="createBy"></param>
        /// <param name="createDate"></param>
        /// <returns></returns>
        public Task<VNPayRefundResponseModel> RefundAsync(VNPayConfigModel config, VNPayRefundRequestModel vNPayRefundModel);

        /// <summary>
        /// This method is used to verify the VNPAY signature.
        /// </summary>
        /// <param name="inputHash">The value from the URL parameter vnp_SecureHash.</param>
        /// <param name="secretKey">The value from the URL parameter vnp_HashSecret.</param>
        /// <returns></returns>
        bool ValidateSignature(IQueryCollection vnpayData, string inputHash, string secretKey);
    }
}
