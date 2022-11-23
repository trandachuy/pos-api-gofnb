using GoFoodBeverage.Common.Models.User;
using GoFoodBeverage.POS.Models.Kitchen;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Interfaces
{
    public interface IKitchenService
    {
        Task UpdateOrderItemStatusAsync(UpdateOrderItemStatusRequestModel request);

        Task UpdateOrderSessionStatusAsync(UpdateOrderSessionStatusRequestModel request);

        Task GetKitchenOrderSessionsAsync(CancellationToken cancellationToken);

        Task GetOrderCodeFromKitchenAsync(string orderCode, LoggedUserModel loggedUser, CancellationToken cancellationToken);
    }
}
