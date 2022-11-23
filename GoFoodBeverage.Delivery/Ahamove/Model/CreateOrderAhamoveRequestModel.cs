using System.Collections.Generic;

namespace GoFoodBeverage.Delivery.Ahamove.Model
{
    /// <summary>
    /// Link: https://developers.ahamove.com/#create-order
    /// </summary>
    public class CreateOrderAhamoveRequestModel
    {
        /// <summary>
        /// The pickup time. Set 0 to pickup ASAP.
        /// </summary>
        public string OrderTime { get; set; }

        /// <summary>
        /// AhaMove Service ID (SGN-BIKE, SGN-POOL, etc.).
        /// </summary>
        public string ServiceId { get; set; }

        /// <summary>
        /// Payment method chosen by user (BALANCE or CASH)
        /// </summary>
        public string PaymentMethod { get; set; }

        public AhamoveAddressDto SenderAddress { get; set; }

        public AhamoveAddressDto ReceiverAddress { get; set; }

        public List<AhamoveProductDto> Products { get; set; }

        public class AhamoveAddressDto
        {
            public string Name { get; set; }

            public string Phone { get; set; }

            public string Address { get; set; }

            public double Lat { get; set; }

            public double Lng { get; set; }

            public string Remarks { get; set; }

            public double? Cod { get; set; }
        }

        public class AhamoveProductDto
        {
            public string Id { get; set; }

            public int Amount { get; set; }

            public string Name { get; set; }

            public decimal Price { get; set; }
        }
    }
}
