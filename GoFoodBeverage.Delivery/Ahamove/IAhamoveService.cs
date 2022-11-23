using GoFoodBeverage.Delivery.Ahamove.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoFoodBeverage.Delivery.Ahamove
{
    public interface IAhamoveService
    {
        Task<AhamoveTokenModel> GetAhamoveTokenAsync(AhamoveConfigByStoreRequestModel request);

        Task<List<GetListOrderAhamoveResponseModel.OrderDto>> GetOrdersAsync(string token);

        Task<CreateOrderAhamoveResponseModel> CreateOrderAsync(string token, CreateOrderAhamoveRequestModel request);

        Task<OrderDetailAhamoveResponseModel> GetOrderDetailAsync(string token, string orderId);

        Task<bool> CancelOrderAsync(string token, string orderId, string comment);

        Task<EstimatedOrderAhamoveFeeResponseModel> EstimateOrderFee(string token, EstimateOrderAhamoveRequestModel request);
    }
}
