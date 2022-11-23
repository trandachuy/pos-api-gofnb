using Newtonsoft.Json;

namespace GoFoodBeverage.Delivery.Ahamove.Model
{
    public class CreateOrderAhamoveResponseModel
    {
        [JsonProperty("order_id")]
        public string OrderId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("shared_link")]
        public string SharedLink { get; set; }

        [JsonProperty("order")]
        public OrderDto Order { get; set; }

        public class OrderDto
        {
            [JsonProperty("total_fee")]
            public decimal TotalFee { get; set; }

            [JsonProperty("voucher_discount")]
            public decimal VoucherDiscount { get; set; }

            [JsonProperty("subtotal_price")]
            public decimal SubtotalPrice { get; set; }
        }
    }
}
