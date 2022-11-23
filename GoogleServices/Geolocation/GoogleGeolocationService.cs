using GoFoodBeverage.Domain.Settings;
using GoogleServices.Geolocation.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleServices.Geolocation
{
    public class GoogleGeolocationService : IGoogleGeolocationService
    {
        private readonly GoogleApiSettings _googleApiSettings;
        private readonly AppSettings _appSettings;
        private readonly HttpClient _httpClient;

        public GoogleGeolocationService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            _googleApiSettings = _appSettings.GoogleApiSettings;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://maps.google.com/")
            };
        }

        public async Task<GoogleAddress> GeocodeAsync(string street, string city, string state, string postalCode, string country, CancellationToken cancellationToken)
        {
            return await GeocodeAsync(BuildAddress(street, city, state, postalCode, country), cancellationToken);
        }

        public async Task<GoogleAddress> GeocodeAsync(string address, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(address)) throw new ArgumentNullException("address");

            HttpResponseMessage response = await _httpClient.GetAsync($"/maps/api/geocode/json?address={WebUtility.UrlEncode(address)}&key={_googleApiSettings.GeolocationApiKey}", cancellationToken);
            return ProcessWebResponse(response);
        }

        private static GoogleAddress ProcessWebResponse(HttpResponseMessage response)
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
            GeolocationResultItem result = JsonConvert.DeserializeObject<GeolocationResultItem>(serialized);
            if (!string.IsNullOrWhiteSpace(result.ErrorMessage)) throw new Exception($"{result.Status}: {result.ErrorMessage}");

            return result?.Results?.FirstOrDefault();
        }

        private static string BuildAddress(string street, string city, string state, string postalCode, string country)
        {
            return $"{street} {city}, {state} {postalCode}, {country}";
        }
    }
}
