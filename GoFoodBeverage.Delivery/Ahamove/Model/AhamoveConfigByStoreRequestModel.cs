
namespace GoFoodBeverage.Delivery.Ahamove.Model
{
    /// <summary>
    /// Link: https://developers.ahamove.com/#register-account
    /// </summary>
    public class AhamoveConfigByStoreRequestModel
    {
        /// <summary>
        /// Your phone number used to register account ID.
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// Your account name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The API_KEY of your application which received when you send an email to our developer portal.
        /// </summary>
        public string ApiKey { get; set; }
        /// <summary>
        /// Your home address (Optional if account already existed).
        /// </summary>
        public string Address { get; set; }
    }
}
