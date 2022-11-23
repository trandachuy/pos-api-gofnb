using GoFoodBeverage.Common.Constants;
using GoFoodBeverage.Delivery.Ahamove.Model;
using GoFoodBeverage.Domain.Settings;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace GoFoodBeverage.Delivery.Ahamove
{
    public class AhamoveService : IAhamoveService
    {
        private readonly HttpClient _httpClient;
        private readonly AppSettings _appSettings;
        private readonly AhaMoveSettings _ahamoveSettings;

        public AhamoveService(HttpClient httpClient, IOptions<AppSettings> appSettings)
        {
            _httpClient = httpClient;
            _appSettings = appSettings.Value;
            _ahamoveSettings = _appSettings.DeliverySettings.AhaMoveSettings;
        }

        public async Task<AhamoveTokenModel> GetAhamoveTokenAsync(AhamoveConfigByStoreRequestModel request)
        {
            var registerAhamoveAccountUrl = $"{_ahamoveSettings.DomainProduction}/v1/partner/register_account";

            var queryCreateTokenAhaMove = new Dictionary<string, string>()
            {
                ["mobile"] = request.Mobile,
                ["name"] = request.Name,
                ["api_key"] = request.ApiKey,
                ["address"] = request.Address
            };

            var url = QueryHelpers.AddQueryString(registerAhamoveAccountUrl, queryCreateTokenAhaMove);
            var httpResponseMessage = await _httpClient.GetAsync(url);
            var responseData = await httpResponseMessage.Content.ReadAsStringAsync();
            var ahamoveToken = JsonConvert.DeserializeObject<AhamoveTokenModel>(responseData);

            return ahamoveToken;
        }

        public async Task<EstimatedOrderAhamoveFeeResponseModel> EstimateOrderFee(string token, EstimateOrderAhamoveRequestModel request)
        {
            var estimateOrderFeeUrl = $"{_ahamoveSettings.DomainProduction}/v1/order/estimated_fee";

            var pathAddress = BuidAddressForEstimateOrderFee(request);
            var queryCreateOrderToAhaMove = new Dictionary<string, string>()
            {
                ["token"] = token,
                ["order_time"] = AhamoveDeliveryConfigConstants.ORDER_TIME,
                ["service_id"] = AhamoveDeliveryConfigConstants.SERVICE_ID,
                ["path"] = pathAddress,
            };

            var uri = QueryHelpers.AddQueryString(estimateOrderFeeUrl, queryCreateOrderToAhaMove);
            var httpResponseMessage = await _httpClient.GetAsync(uri);
            var responseData = await httpResponseMessage.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<EstimatedOrderAhamoveFeeResponseModel>(responseData);

            return response;
        }

        public async Task<CreateOrderAhamoveResponseModel> CreateOrderAsync(string token, CreateOrderAhamoveRequestModel request)
        {
            var createAhamoveOrderUrl = $"{_ahamoveSettings.DomainProduction}/v1/order/create";

            var pathAddress = BuidAddress(request);
            var productItems = GetProductItems(request);
            var queryCreateOrderToAhaMove = new Dictionary<string, string>()
            {
                ["token"] = token,
                ["order_time"] = AhamoveDeliveryConfigConstants.ORDER_TIME,
                ["service_id"] = AhamoveDeliveryConfigConstants.SERVICE_ID,
                ["payment_method"] = request.PaymentMethod,
                ["path"] = pathAddress,
                ["items"] = productItems,
            };

            var uri = QueryHelpers.AddQueryString(createAhamoveOrderUrl, queryCreateOrderToAhaMove);
            var httpResponseMessage = await _httpClient.GetAsync(uri);
            var responseData = await httpResponseMessage.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<CreateOrderAhamoveResponseModel>(responseData);

            return response;
        }

        public async Task<bool> CancelOrderAsync(string token, string orderId, string comment)
        {
            var cancelOrderUrl = $"{_ahamoveSettings.DomainProduction}/v1/order/cancel";

            var queryCreateOrderToAhaMove = new Dictionary<string, string>()
            {
                ["token"] = token,
                ["order_id"] = orderId,
                ["comment"] = comment,
            };

            var uri = QueryHelpers.AddQueryString(cancelOrderUrl, queryCreateOrderToAhaMove);
            var httpResponseMessage = await _httpClient.GetAsync(uri);
            var result = httpResponseMessage.StatusCode == System.Net.HttpStatusCode.OK;

            return result;
        }

        public async Task<OrderDetailAhamoveResponseModel> GetOrderDetailAsync(string token, string orderId)
        {
            var orderDetailUrl = $"{_ahamoveSettings.DomainProduction}/v1/order/detail";

            var queryCreateOrderToAhaMove = new Dictionary<string, string>()
            {
                ["token"] = token,
                ["order_id"] = orderId,
            };

            var uri = QueryHelpers.AddQueryString(orderDetailUrl, queryCreateOrderToAhaMove);
            var httpResponseMessage = await _httpClient.GetAsync(uri);
            var responseData = await httpResponseMessage.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<OrderDetailAhamoveResponseModel>(responseData);

            return response;
        }

        public async Task<List<GetListOrderAhamoveResponseModel.OrderDto>> GetOrdersAsync(string token)
        {
            var orderListUrl = $"{_ahamoveSettings.DomainProduction}/v1/order/list";

            var queryListOrderToAhaMove = new Dictionary<string, string>()
            {
                ["token"] = token,
            };

            var uri = QueryHelpers.AddQueryString(orderListUrl, queryListOrderToAhaMove);
            var httpResponseMessage = await _httpClient.GetAsync(uri);
            var responseData = await httpResponseMessage.Content.ReadAsStringAsync();
            var response = JsonConvert.DeserializeObject<List<GetListOrderAhamoveResponseModel.OrderDto>>(responseData);

            return response;
        }

        #region Private function
        private static string BuidAddress(CreateOrderAhamoveRequestModel request)
        {
            var senderName = request.SenderAddress.Name;
            var senderPhone = request.SenderAddress.Phone ?? "";
            var senderAddress = request.SenderAddress.Address;
            var senderLat = request.SenderAddress.Lat;
            var senderLng = request.SenderAddress.Lng;
            var senderRemarks = request.SenderAddress.Remarks ?? "";

            var senderInfo = $"\"address\":\"{senderAddress}\", \"lat\":{senderLat}, \"lng\":{senderLng}, \"name\":\"{senderName}\", \"mobile\":\"{senderPhone}\", \"remarks\":\"{senderRemarks}\"";

            var receiverName = request.ReceiverAddress.Name;
            var receiverPhone = request.ReceiverAddress.Phone ?? "";
            var receiverAddress = request.ReceiverAddress.Address;
            var receiverLat = request.ReceiverAddress.Lat;
            var receiverLng = request.ReceiverAddress.Lng;
            var receiverRemarks = request.ReceiverAddress.Remarks ?? "";
            var receiverCod = request.ReceiverAddress.Cod ?? 0;

            var receiverInfo = $"\"address\":\"{receiverAddress}\", \"lat\":{receiverLat}, \"lng\":{receiverLng}, \"name\":\"{receiverName}\", \"mobile\":\"{receiverPhone}\", \"remarks\":\"{receiverRemarks}\", \"cod\":{receiverCod}";

            var address = "[{" + senderInfo + "}, {" + receiverInfo + "}]";

            return address;
        }

        private static string GetProductItems(CreateOrderAhamoveRequestModel request)
        {
            var listItems = "";
            foreach (var product in request.Products)
            {
                var productInfo = $"\"_id\":\"{product.Id}\", \"num\":\"{product.Amount}\", \"name\":\"{product.Name}\", \"price\":\"{product.Price}\"";
                listItems = listItems + "{" + productInfo + "},";
            }

            listItems = "[" + listItems + "]";
            var listItemsFormat = listItems[0..^2] + ']';

            return listItemsFormat;
        }

        private static string BuidAddressForEstimateOrderFee(EstimateOrderAhamoveRequestModel request)
        {
            var senderAddress = request.SenderAddress.Address;
            var senderLat = request.SenderAddress.Lat;
            var senderLng = request.SenderAddress.Lng;
            var senderInfo = $"\"address\":\"{senderAddress}\", \"lat\":{senderLat}, \"lng\":{senderLng}";

            var receiverAddress = request.ReceiverAddress.Address;
            var receiverLat = request.ReceiverAddress.Lat;
            var receiverLng = request.ReceiverAddress.Lng;
            var receiverInfo = $"\"address\":\"{receiverAddress}\", \"lat\":{receiverLat}, \"lng\":{receiverLng}";

            var address = "[{" + senderInfo + "}, {" + receiverInfo + "}]";

            return address;
        }

        #endregion
    }
}
