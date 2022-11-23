using Newtonsoft.Json;
using System.Collections.Generic;

namespace GoogleServices.Geolocation.Models
{
    public class GoogleAddress
    {
        [JsonProperty("address_components")]
        public IEnumerable<GoogleAddressComponent> AddressComponents { get; set; }

        [JsonProperty("formatted_address")]
        public string FormattedAddress { get; set; }

        [JsonProperty("geometry")]
        public GoogleGeometry Geometry { get; set; }

        [JsonProperty("place_id")]
        public string PlaceId { get; set; }

        [JsonProperty("plus_code")]
        public GooglePlusCode PlusCode { get; set; }

        [JsonProperty("types")]
        public string[] Types { get; set; }
    }

    public class GoogleAddressComponent
    {
        [JsonProperty("long_name")]
        public string LongName { get; set; }

        [JsonProperty("short_name")]
        public string ShortName { get; set; }

        [JsonProperty("types")]
        public string[] Types { get; set; }
    }

    public class GoogleGeometry
    {
        [JsonProperty("bounds")]
        public GoogleViewportAndBounds Bounds { get; set; }

        [JsonProperty("location")]
        public GoogleLocation Location { get; set; }

        [JsonProperty("location_type")]
        public string LocationType { get; set; }

        [JsonProperty("viewport")]
        public GoogleViewportAndBounds Viewport { get; set; }
    }

    public class GooglePlusCode
    {
        [JsonProperty("compound_code")]
        public string CompoundCode { get; set; }

        [JsonProperty("global_code")]
        public string GlobalCode { get; set; }
    }

    public class GoogleViewportAndBounds
    {
        [JsonProperty("northeast")]
        public GoogleLocation Northeast { get; set; }

        [JsonProperty("southwest")]
        public GoogleLocation Southwest { get; set; }
    }

    public class GoogleLocation
    {
        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lng")]
        public double Lng { get; set; }
    }
}