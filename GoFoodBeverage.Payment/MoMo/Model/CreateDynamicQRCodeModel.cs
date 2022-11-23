using System;

namespace GoFoodBeverage.Payment.MoMo.Model
{
    /// <summary>
    /// Details of QR Code data
    /// Docs: https://developers.momo.vn/v2/#/docs/en/qr_payment
    /// </summary>
    public class CreateDynamicQRCodeModel
    {
        /// <summary>
        /// MoMo's server address
        /// Auto get from appsettings
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// Partner Code
        /// </summary>
        public string PartnerCode { get; set; }

        /// <summary>
        /// Store ID
        /// </summary>
        public string StoreId { get; set; }

        /// <summary>
        /// Store Slug is identification code created in format partnerCode-storeId
        /// </summary>
        public string StoreSlug { get { return $"{PartnerCode}-{StoreId}"; } }

        /// <summary>
        /// Payment amount
        /// </summary>
        public long Amount { get; set; }

        /// <summary>
        /// Partner's transaction ID
        /// Format: ^[0-9a-zA-Z] ([-_.]*[0 - 9a - zA - Z]+)*$
        /// </summary>
        public string BillId { get; set; }

        /// <summary>
        /// Additional information: Encode base64 folllow 1 of 2 format:
        /// Format(String) : { "sku": "sku1, sku2,..."}
        /// Example: with these skus {"skus": "sku1, sku2, sku3"} => data of extraData is eyJza3VzIjogInNrdTEsIHNrdTIsIHNrdTMifQ==
        /// Default value is empty ""
        /// </summary>
        public string Extra { get; set; } = string.Empty;

        /// <summary>
        /// Signature to check information. Use Hmac_SHA256 algorithm with data in format:
        /// storeSlug=$storeSlug&amount=$amount&billId=$billId
        /// </summary>
        public string Signature { get; set; }
    }
}
