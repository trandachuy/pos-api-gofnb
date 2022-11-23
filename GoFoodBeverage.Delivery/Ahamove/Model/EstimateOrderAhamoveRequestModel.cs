using System.Collections.Generic;

namespace GoFoodBeverage.Delivery.Ahamove.Model
{
    /// <summary>
    /// Link: https://developers.ahamove.com/#create-order
    /// </summary>
    public class EstimateOrderAhamoveRequestModel
    {
        /// <summary>
        /// The pickup time. Set 0 to pickup ASAP.
        /// </summary>
        public string OrderTime { get; set; }

        /// <summary>
        /// AhaMove Service ID (SGN-BIKE, SGN-POOL, etc.).
        /// </summary>
        public string ServiceId { get; set; }

        public AhamoveAddressDto SenderAddress { get; set; }

        public AhamoveAddressDto ReceiverAddress { get; set; }

        public class AhamoveAddressDto
        {
            public string Address { get; set; }

            public double Lat { get; set; }

            public double Lng { get; set; }
        }
    }
}
