using System;
using System.Threading.Tasks;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.Interfaces.Repositories
{
    public interface IDeliveryConfigRepository : IGenericRepository<DeliveryConfig>
    {
        /// <summary>
        /// Get delivery config by delivery method Id and store Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="storeId"></param>
        /// <returns>GroupPermission object</returns>
        Task<DeliveryConfig> GetDeliveryConfigByDeliveryMethodIdAsync(Guid deliveryMethodId, Guid storeId);

        /// <summary>
        /// Get AhaMove config by delivery method Id and store Id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="storeId"></param>
        /// <returns>GroupPermission object</returns>
        Task<DeliveryConfig> GetAhaMoveConfigByDeliveryMethodIdAsync(Guid deliveryMethodId, Guid storeId);
    }
}
