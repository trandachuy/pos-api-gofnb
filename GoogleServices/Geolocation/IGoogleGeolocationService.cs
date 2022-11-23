using GoogleServices.Geolocation.Models;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleServices.Geolocation
{
    public interface IGoogleGeolocationService
    {
        Task<GoogleAddress> GeocodeAsync(string formattedAddress, CancellationToken cancellationToken);

        Task<GoogleAddress> GeocodeAsync(string street, string city, string state, string postalCode, string country, CancellationToken cancellationToken);
    }
}
