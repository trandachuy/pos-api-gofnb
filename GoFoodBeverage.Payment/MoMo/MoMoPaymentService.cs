using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.Domain.Settings;
using GoFoodBeverage.Models.Payment;
using GoFoodBeverage.Payment.MoMo.Enums;
using GoFoodBeverage.Payment.MoMo.Model;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace GoFoodBeverage.Payment.MoMo
{
    /// <summary>
    /// Payment services integrate with momo api
    /// </summary>
    public class MoMoPaymentService : IMoMoPaymentService
    {
        private readonly AppSettings _appSettings;
        private readonly MoMoSettings _momoSettings;

        public MoMoPaymentService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            _momoSettings = _appSettings.PaymentSettings.MoMoSettings;
        }

        public Task<CreateGetwayResponseModel> CreateGatewayTestingAsync(PartnerMoMoPaymentConfigModel config, CreateGetwayRequestModel request)
        {
            string endpoint = $"{_momoSettings.DomainSandBox}/v2/gateway/api/create";

            /// Momo Configs
            string SecretKey = config.SecretKey;
            string partnerCode = config.PartnerCode;
            string accessKey = config.AccessKey;

            /// Request data
            string requestId = request.RequestId;
            string amount = request.Amount;
            string extraData = request.ExtraData;
            string orderId = request.OrderId;
            string orderInfo = request.OrderInfo;
            string partnerClientId = request.PartnerClientId;
            string redirectUrl = request.RedirectUrl;
            string ipnUrl = request.IpnUrl;

            string rawHash = $"accessKey={accessKey}&" +
                $"amount={amount}&" +
                $"extraData={extraData}&" +
                $"ipnUrl={ipnUrl}&" +
                $"orderId={orderId}&" +
                $"orderInfo={orderInfo}&" +
                $"partnerClientId={partnerClientId}&" +
                $"partnerCode={partnerCode}&" +
                $"redirectUrl={redirectUrl}&" +
                $"requestId={requestId}&" +
                $"requestType=linkWallet";

            var momoSecurity = new MoMoSecurity();
            string signature = momoSecurity.SignSHA256(rawHash, SecretKey);
            var requestData = new JObject
            {
                { "partnerCode", partnerCode },
                { "accessKey", accessKey },
                { "requestId", requestId },
                { "amount", amount },
                { "extraData", extraData },
                { "orderId", orderId },
                { "orderInfo", orderInfo },
                { "ipnUrl", ipnUrl },
                { "redirectUrl", redirectUrl },
                { "partnerClientId", partnerClientId },
                { "requestType", "linkWallet" },
                { "lang", "vi" },
                { "signature", signature },
            };

            var responseFromMomo = MoMoPaymentRequest.SendPaymentRequest(endpoint, requestData.ToString());
            if (responseFromMomo.Success)
            {
                var createGetwayResponseModel = new CreateGetwayResponseModel(responseFromMomo.Data);
                return Task.FromResult(createGetwayResponseModel);
            }
            else
            {
                return null;
            }
        }

        public Task<CreateGetwayResponseModel> CreateGatewayAsync(PartnerMoMoPaymentConfigModel config, CreateGetwayRequestModel request, string requestType)
        {
            string endpoint = $"{_momoSettings.DomainProduction}/v2/gateway/api/create";
            /// Momo Configs
            string SecretKey = config.SecretKey;
            string partnerCode = config.PartnerCode;
            string accessKey = config.AccessKey;

            /// Request data
            string requestId = request.RequestId;
            string amount = request.Amount;
            string extraData = request.ExtraData;
            string orderId = request.OrderId;
            string orderInfo = request.OrderInfo;
            string partnerClientId = request.PartnerClientId;
            string redirectUrl = request.RedirectUrl;
            string ipnUrl = request.IpnUrl;

            string rawHash = $"accessKey={accessKey}&" +
                $"amount={amount}&" +
                $"extraData={extraData}&" +
                $"ipnUrl={ipnUrl}&" +
                $"orderId={orderId}&" +
                $"orderInfo={orderInfo}&" +
                $"partnerClientId={partnerClientId}&" +
                $"partnerCode={partnerCode}&" +
                $"redirectUrl={redirectUrl}&" +
                $"requestId={requestId}&" +
                $"requestType={requestType}";

            if (requestType.Equals(RequestTypes.CaptureWallet))
            {
                rawHash = $"accessKey={accessKey}&" +
                $"amount={amount}&" +
                $"extraData={extraData}&" +
                $"ipnUrl={ipnUrl}&" +
                $"orderId={orderId}&" +
                $"orderInfo={orderInfo}&" +
                $"partnerCode={partnerCode}&" +
                $"redirectUrl={redirectUrl}&" +
                $"requestId={requestId}&" +
                $"requestType={requestType}";
            }

            var momoSecurity = new MoMoSecurity();
            string signature = momoSecurity.SignSHA256(rawHash, SecretKey);
            var requestData = new JObject
            {
                { "partnerCode", partnerCode },
                { "accessKey", accessKey },
                { "requestId", requestId },
                { "amount", amount },
                { "extraData", extraData },
                { "orderId", orderId },
                { "orderInfo", orderInfo },
                { "ipnUrl", ipnUrl },
                { "redirectUrl", redirectUrl },
                { "partnerClientId", partnerClientId },
                { "requestType", requestType },
                { "lang", "vi" },
                { "signature", signature },
            };

            if (requestType.Equals(RequestTypes.CaptureWallet))
            {
                requestData = new JObject
                {
                    { "partnerCode", partnerCode },
                    { "accessKey", accessKey },
                    { "requestId", requestId },
                    { "amount", amount },
                    { "extraData", extraData },
                    { "orderId", orderId },
                    { "orderInfo", orderInfo },
                    { "ipnUrl", ipnUrl },
                    { "redirectUrl", redirectUrl },
                    { "requestType", requestType },
                    { "lang", "vi" },
                    { "signature", signature },
                };
            }

            var responseFromMomo = MoMoPaymentRequest.SendPaymentRequest(endpoint, requestData.ToString());
            if (responseFromMomo.Success)
            {
                var createGetwayResponseModel = new CreateGetwayResponseModel(responseFromMomo.Data);
                return Task.FromResult(createGetwayResponseModel);
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// Using pay pos v2
        /// docs: https://developers.momo.vn/v2/#/docs/pos_payment
        /// </summary>
        /// <param name="config"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<object> CreatePosGatewayAsync(PartnerMoMoPaymentConfigModel config, CreatePosGatewayRequest request)
        {
            //string endpoint = $"{_momoSettings.DomainProduction}/v2/gateway/api/pos";
            string endpoint = $"{_momoSettings.DomainProduction}/pay/pos";
            /// Momo Configs
            string partnerCode = config.PartnerCode;
            long amount = request.Amount;

            /// Related to request
            string partnerRefId = Guid.NewGuid().ToString();
            string storeId = request.StoreId;
            string storeName = request.StoreName;
            string description = "";
            string version = "2.0";

            // momo public key from
            // https://business.momo.vn/merchant/integrateQRAIAInfo
            var publicKey = "MIICIjANBgkqhkiG9w0BAQEFAAOCAg8AMIICCgKCAgEAyDyCFO8wztbw+xkWmAJ3axm/Xv0KDxrZ3L/X68VC/HuRDOBjKRUNggdS33P5cYyUqZlZgV3kaKCwfKwi2juReAw6K+Wta/EKGPYN+oSCQBpLrq+W5bG6Dfs6AF2kiX6we+/adA7Q/05M9OT2npRv61CtNMd89zDAWTpu7hNqbnz6xswXNx3iozzSQgA7VPZuy65kgTvKzZuiwMhORDkwpsqjCnKfwB3pdJGRE6GXAjZWKNvJXY4O/kRDPlLxDSGzYb67zflpyH2IeYOLvA17opmMJWUMGuWCprIsKyjsX5HIvTxbiizj/l1CSO4BgyrIrEVN8BJ2Os/al9TKjNzqEVCWczIjuaeIzIzhTzQBJPzSQYI8d0vYUri1zdg3ty/yi8cpptb9hBhayaGkNaNUVouYNQv/WqqYfaMZrq6efLkdwPLtWeahSdzAjMrRyDLyG6UJacPP3PXr+veUZZKRHEQye7rQLmJXpCmiONg97QEjcGHneiZME/akPo0gkbcebSBhyJKR5WinFmhAgWP81ieaEr9J1Hp9nAPDzsWM3fr8aMNUbeFpPkv81KgTWV2pTLnjv5tpQgfQMEJhvu9DpqQjNQrbYcGgJXZGpIA4NBtQLBviUlITd2RoIr+Td1Bm1dn+X35NqP6bkDW20AkiU6P4ok6ov5P7jFNORDOzhZUCAwEAAQ==";
            var rsaPublicKey = publicKey.ConvertPublicKeyToXmlString();
            var momoSecurity = new MoMoSecurity();
            var paymentCode = request.PaymentCode;
            string hash = momoSecurity.GetHash(partnerCode, partnerRefId, amount.ToString(), paymentCode, storeId, storeName, rsaPublicKey);
            var requestData = new JObject
            {
                { "partnerCode", partnerCode },
                { "partnerRefId", partnerRefId },
                { "description", description },
                { "amount", amount },
                { "version", version },
                { "hash", hash },
            };

            var responseFromMomo = MoMoPaymentRequest.SendPaymentRequest(endpoint, requestData.ToString());
            if (responseFromMomo.Success)
            {
                var createGetwayResponseModel = JsonConvert.DeserializeObject<object>(responseFromMomo.Data);
                return Task.FromResult(createGetwayResponseModel);
            }
            else
            {
                return null;
            }
        }

        public Task<QueryStatusResponseModel> QueryStatusAsync(PartnerMoMoPaymentConfigModel config, QueryStatusRequestModel request)
        {
            string endpoint = $"{_momoSettings.DomainProduction}/v2/gateway/api/query";

            /// Momo Configs
            string SecretKey = config.SecretKey;
            string partnerCode = config.PartnerCode;
            string accessKey = config.AccessKey;

            /// Request data
            string requestId = request.RequestId;
            string amount = request.Amount;
            string orderId = request.OrderId;

            string rawHash =
                $"accessKey={accessKey}&" +
                $"orderId={orderId}&" +
                $"partnerCode={partnerCode}&" +
                $"requestId={requestId}";

            var momoSecurity = new MoMoSecurity();
            string signature = momoSecurity.SignSHA256(rawHash, SecretKey);
            var requestData = new JObject
            {
                { "partnerCode", partnerCode },
                { "requestId", requestId },
                { "orderId", orderId },
                { "amount", amount },
                { "lang", "vi" },
                { "signature", signature },
            };

            var responseFromMomo = MoMoPaymentRequest.SendPaymentRequest(endpoint, requestData.ToString());
            if (responseFromMomo.Success)
            {
                var createGetwayResponseModel = new QueryStatusResponseModel(responseFromMomo.Data);
                return Task.FromResult(createGetwayResponseModel);
            }
            else
            {
                return null;
            }
        }

        public Task<string> CreateDynamicQRCodeContentAsync(PartnerMoMoPaymentConfigModel config, CreateDynamicQRCodeModel request)
        {
            string domain = $"{_momoSettings.DomainProduction}";
            string SecretKey = config.SecretKey;
            string rawHash =
                $"storeSlug={request.StoreSlug}&" +
                $"amount={request.Amount}&" +
                $"billId={request.BillId}&" +
                $"extra={request.Extra}";
            var momoSecurity = new MoMoSecurity();
            string signature = momoSecurity.SignSHA256(rawHash, SecretKey);
            var qrCodeContent = $"{domain}/pay/store/{request.StoreSlug}?a={request.Amount}&b={request.BillId}&extra={request.Extra}&s={signature}";

            if (string.IsNullOrEmpty(request.Extra))
            {
                rawHash =
                $"storeSlug={request.StoreSlug}&" +
                $"amount={request.Amount}&" +
                $"billId={request.BillId}";
                signature = momoSecurity.SignSHA256(rawHash, SecretKey);
                qrCodeContent = $"{domain}/pay/store/{request.StoreSlug}?a={request.Amount}&b={request.BillId}&s={signature}";
            }

            return Task.FromResult(qrCodeContent);
        }

        /// <summary>
        /// docs: https://developers.momo.vn/v3/vi/docs/payment/api/payment-api/refund/#http-request
        /// </summary>
        /// <param name="config"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<CreateRefundResponse> CreateRefundAsync(PartnerMoMoPaymentConfigModel config, CreateRefundRequest request)
        {
            string endpoint = $"{_momoSettings.DomainProduction}/v2/gateway/api/refund";

            // Momo Configs
            string SecretKey = config.SecretKey;
            string partnerCode = config.PartnerCode;
            string accessKey = config.AccessKey;

            /// Request data
            string requestId = request.RequestId;
            long amount = request.Amount;
            string orderId = request.OrderId;
            string description = request.Description;
            long transId = request.TransId;

            string rawHash =
                $"accessKey={accessKey}&" +
                $"amount={amount}&" +
                $"description={description}&" +
                $"orderId={orderId}&" +
                $"partnerCode={partnerCode}&" +
                $"requestId={requestId}&" +
                $"transId={transId}";

            var momoSecurity = new MoMoSecurity();
            string signature = momoSecurity.SignSHA256(rawHash, SecretKey);
            var requestData = new JObject
            {
                { "partnerCode", partnerCode },
                { "orderId", orderId },
                { "requestId", requestId },
                { "amount", amount },
                { "transId", transId },
                { "lang", "vi" },
                { "description", description },
                { "signature", signature },
            };

            var responseFromMomo = MoMoPaymentRequest.SendPaymentRequest(endpoint, requestData.ToString());
            if (responseFromMomo.Success)
            {
                var createRefundResponseModel = new CreateRefundResponse(responseFromMomo.Data);
                return Task.FromResult(createRefundResponseModel);
            }
            else
            {
                return null;
            }
        }

        public string Base64Encode(JObject jObject)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(jObject.ToString());
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}
