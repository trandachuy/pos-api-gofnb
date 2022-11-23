using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleServices.Geolocation.Models
{
    public class GeolocationResultItem
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("results")]
        public IEnumerable<GoogleAddress> Results { get; set; }

        [JsonProperty("error_message")]
        public string ErrorMessage { get; set; }
    }
}
