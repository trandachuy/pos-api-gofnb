using System;

namespace GoFoodBeverage.Models.Payment
{
    public class PaymentConfigModel
    {
        public Guid? Id { get; set; }

        public Guid? PaymentMethodId { get; set; }

        public string PartnerCode { get; set; }

        public string AccessKey { get; set; }

        public string SecretKey { get; set; }

        public string QRCode { get; set; }

        public bool IsActivated { get; set; }

        public bool IsAuthenticated { get; set; }
    }
}
