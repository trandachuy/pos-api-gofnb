using Newtonsoft.Json;

namespace GoFoodBeverage.Delivery.Ahamove.Model
{
    public class EstimatedOrderAhamoveFeeResponseModel
    {
        [JsonProperty("distance")]
        public double Distance { get; set; }

        [JsonProperty("duration")]
        public double Duration { get; set; }

        [JsonProperty("distance_fee")]
        public decimal DistanceFee { get; set; }

        [JsonProperty("stoppoint_price")]
        public decimal StoppointPrice { get; set; }

        [JsonProperty("total_price")]
        public decimal TotalPrice { get; set; }
    }
}
