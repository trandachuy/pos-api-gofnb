using GoFoodBeverage.Models.Payment;
using GoFoodBeverage.Payment.MoMo.Model;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace GoFoodBeverage.Payment.MoMo
{
    /// <summary>
    /// Follow MOMO payment docs
    /// https://developers.momo.vn/v3/docs/payment/api/wallet/pay-with-token
    /// https://developers.momo.vn/v2/#/docs/qr_payment
    /// </summary>
    public interface IMoMoPaymentService
    {
        /// <summary>
        /// Create gateway request type is linkWallet on TESTING env
        /// </summary>
        /// <param name="config"></param>
        /// <param name="request"></param>
        /// <returns>CreateGetwayResponseModel</returns>
        Task<CreateGetwayResponseModel> CreateGatewayTestingAsync(PartnerMoMoPaymentConfigModel config, CreateGetwayRequestModel request);

        /// <summary>
        /// Create gateway
        /// Docs: https://developers.momo.vn/v3/vi/docs/payment/api/wallet/onetime
        /// </summary>
        /// <param name="config"></param>
        /// <param name="request"></param>
        /// <param name="requestType"></param>
        /// <returns></returns>
        Task<CreateGetwayResponseModel> CreateGatewayAsync(PartnerMoMoPaymentConfigModel config, CreateGetwayRequestModel request, string requestType);

        /// <summary>
        /// Thanh toan qua POS
        /// </summary>
        /// <param name="config"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<object> CreatePosGatewayAsync(PartnerMoMoPaymentConfigModel config, CreatePosGatewayRequest request);

        /// <summary>
        /// Check Transaction Status
        /// This API gets the transaction status corresponding to requested OrderId for specific merchant.
        /// Docs: https://developers.momo.vn/v3/docs/payment/api/payment-api/query
        /// </summary>
        /// <param name="config"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<QueryStatusResponseModel> QueryStatusAsync(PartnerMoMoPaymentConfigModel config, QueryStatusRequestModel request);

        /// <summary>
        /// Create Dynamic QR Code Payment
        /// https://developers.momo.vn/v2/#/docs/qr_payment
        /// </summary>
        /// <param name="config"></param>
        /// <param name="request"></param>
        /// <returns>QRCode contents: string</returns>
        Task<string> CreateDynamicQRCodeContentAsync(PartnerMoMoPaymentConfigModel config, CreateDynamicQRCodeModel request);

        Task<CreateRefundResponse> CreateRefundAsync(PartnerMoMoPaymentConfigModel config, CreateRefundRequest request);

        string Base64Encode(JObject jObject);
    }
}
