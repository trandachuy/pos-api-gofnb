using System.Threading.Tasks;

namespace GoFoodBeverage.Interfaces
{
    public interface IUserActivityService
    {
        Task LogAsync(string activityName);

        Task LogAsync<T>(T newData);

        Task LogAsync<T>(T previousData, T newData);
    }
}
