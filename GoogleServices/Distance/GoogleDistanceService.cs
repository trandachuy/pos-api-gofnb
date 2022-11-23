using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using GoFoodBeverage.Domain.Settings;
using GoogleServices.Geolocation.Models;

namespace GoogleServices.Distance
{
    public class GoogleDistanceService : IGoogleDistanceService
    {
        private readonly GoogleApiSettings _googleApiSettings;
        private readonly AppSettings _appSettings;
        private readonly HttpClient _httpClient;

        public GoogleDistanceService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            _googleApiSettings = _appSettings.GoogleApiSettings;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://maps.googleapis.com/")
            };
        }

        public async Task<int> GetDistanceBetweenPointsAsync(double senderLat, double senderLng, double receiverLat, double receiverLng, CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"/maps/api/distancematrix/json?units=metric&origins={senderLat},{senderLng}&destinations={receiverLat},{receiverLng}&key={_googleApiSettings.GeolocationApiKey}", cancellationToken);
            
            var distanceValue = ProcessWebResponse(response);

            return distanceValue;
        }

        private static int ProcessWebResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode) throw new Exception(response.ReasonPhrase);

            string serialized = string.Empty;
            using (var stream = response.Content.ReadAsStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    serialized = reader.ReadToEnd();
                }
            }

            DistanceResponse result = JsonConvert.DeserializeObject<DistanceResponse>(serialized);
            
            var rows = result.Rows.ToArray();
            var elements = rows[0].Elements.ToArray();
            var distanceValue = elements[0].Distance != null ? elements[0].Distance.Value : 0;

            return distanceValue;
        }
    }
}
