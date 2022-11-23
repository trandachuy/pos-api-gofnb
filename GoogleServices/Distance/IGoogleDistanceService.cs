using System.Threading;
using System.Threading.Tasks;

namespace GoogleServices.Distance
{
    public interface IGoogleDistanceService
    {
        Task<int> GetDistanceBetweenPointsAsync(double senderLat, double senderLng, double receiverLat, double receiverLng, CancellationToken cancellationToken);
    }
}
