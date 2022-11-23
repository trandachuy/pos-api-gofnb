using Newtonsoft.Json;
using System.Collections.Generic;

namespace GoFoodBeverage.Delivery.Ahamove.Model
{
    public class OrderDetailAhamoveResponseModel
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("total_price")]
        public decimal TotalPrice { get; set; }

        public IEnumerable<PathDto> Path { get; set; }

        public class PathDto
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("address")]
            public string Address { get; set; }
        }

        public IEnumerable<ItemDto> Items { get; set; }
        public class ItemDto
        {
            [JsonProperty("_id")]
            public string Id { get; set; }

            [JsonProperty("num")]
            public string Quantity { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("price")]
            public string Price { get; set; }
        }
    }
}
