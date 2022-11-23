using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Interfaces
{
    public interface IProvider<TEntity>
    {
        /// <summary>
        /// Provide logged user information from JWT access token
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns> id, accountId, storeId, fullName, email</returns>
        Task<TEntity> ProvideAsync(CancellationToken cancellationToken = default);
    }
}
