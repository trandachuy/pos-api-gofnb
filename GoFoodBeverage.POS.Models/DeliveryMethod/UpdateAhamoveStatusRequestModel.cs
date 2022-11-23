using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GoFoodBeverage.POS.Models.DeliveryMethod
{
    public class UpdateAhamoveStatusRequestModel
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("accept_time")]
        public double? AcceptTime { get; set; }

        [JsonProperty("board_time")]
        public double? BoardTime { get; set; }

        [JsonProperty("cancel_by_user")]
        public bool? CancelByUser { get; set; }

        [JsonProperty("cancel_comment")]
        public string CancelComment { get; set; }

        [JsonProperty("cancel_image_url")]
        public string CancelImageUrl { get; set; }

        [JsonProperty("cancel_time")]
        public double? CancelTime { get; set; }

        [JsonProperty("city_id")]
        public string CityId { get; set; }

        [JsonProperty("complete_time")]
        public double? CompleteTime { get; set; }

        [JsonProperty("create_time")]
        public double CreateTime { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("order_time")]
        public double OrderTime { get; set; }

        [JsonProperty("partner")]
        public string Partner { get; set; }

        [JsonProperty("path")]
        public List<AhamovePath> Path { get; set; }

        [JsonProperty("payment_method")]
        public string PaymentMethod { get; set; }

        [JsonProperty("pickup_time")]
        public double? PickupTime { get; set; }

        [JsonProperty("service_id")]
        public string ServiceId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("sub_status")]
        public string SubStatus { get; set; }

        [JsonProperty("supplier_id")]
        public string SupplierId { get; set; }

        [JsonProperty("supplier_name")]
        public string SupplierName { get; set; }

        [JsonProperty("surcharge")]
        public double Surcharge { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("user_name")]
        public string UserName { get; set; }

        [JsonProperty("total_pay")]
        public double? TotalPay { get; set; }

        [JsonProperty("promo_code")]
        public string PromoCode { get; set; }

        [JsonProperty("stoppoint_price")]
        public double? StoppointPrice { get; set; }

        [JsonProperty("special_request_price")]
        public double? SpecialRequestPrice { get; set; }

        [JsonProperty("vat")]
        public double? Vat { get; set; }

        [JsonProperty("distance_price")]
        public double? DistancePrice { get; set; }

        [JsonProperty("voucher_discount")]
        public double? VoucherDiscount { get; set; }

        [JsonProperty("subtotal_price")]
        public double? SubtotalPrice { get; set; }

        [JsonProperty("total_price")]
        public double? TotalPrice { get; set; }

        [JsonProperty("surge_rate")]
        public double SurgeRate { get; set; }

        [JsonProperty("api_key")]
        public string ApiKey { get; set; }

        [JsonProperty("shared_link")]
        public string SharedLink { get; set; }

        [JsonProperty("app")]
        public string App { get; set; }

        [JsonProperty("store_id")]
        public double? StoreId { get; set; }

        [JsonProperty("distance")]
        public double Distance { get; set; }
    }

    public class AhamovePath
    {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("cod")]
        public double? Cod { get; set; }

        [JsonProperty("por_info")]
        public string PorInfo { get; set; }

        [JsonProperty("short_address")]
        public string ShortAddress { get; set; }

        [JsonProperty("formatted_address")]
        public string FormattedAddress { get; set; }

        [JsonProperty("mobile")]
        public string Mobile { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("complete_lat")]
        public double? CompleteLat { get; set; }

        [JsonProperty("complete_lng")]
        public double? CompleteLng { get; set; }

        [JsonProperty("fail_lat")]
        public double? FailLat { get; set; }

        [JsonProperty("fail_lng")]
        public double? FailLng { get; set; }

        [JsonProperty("complete_time")]
        public double? CompleteTime { get; set; }

        [JsonProperty("fail_time")]
        public double? FailTime { get; set; }

        [JsonProperty("return_time")]
        public double? ReturnTime { get; set; }

        [JsonProperty("pod_info")]
        public string PodInfo { get; set; }

        [JsonProperty("fail_comment")]
        public string FailComment { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
