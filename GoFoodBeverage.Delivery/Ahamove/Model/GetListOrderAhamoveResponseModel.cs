using Newtonsoft.Json;
using System.Collections.Generic;

namespace GoFoodBeverage.Delivery.Ahamove.Model
{
    public class GetListOrderAhamoveResponseModel
    {
        public IEnumerable<OrderDto> Orders { get; set; }

        public class OrderDto
        {
            [JsonProperty("_id")]
            public string Id { get; set; }

            [JsonProperty("currency")]
            public string Currency { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("total_price")]
            public decimal TotalPrice { get; set; }
        }
    }
}
