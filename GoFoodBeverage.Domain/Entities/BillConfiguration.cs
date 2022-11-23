using System;
using GoFoodBeverage.Domain.Base;
using GoFoodBeverage.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(BillConfiguration))]
    public class BillConfiguration : BaseEntity
    {
        public Guid? StoreId { get; set; }

        public EnumBillFrameSize BillFrameSize { get; set; }

        public bool IsShowLogo { get; set; }

        public string LogoData { get; set; }

        public bool IsShowAddress { get; set; }

        public bool IsShowOrderTime { get; set; }

        public bool IsShowCashierName { get; set; }

        public bool IsShowCustomerName { get; set; }

        public bool IsShowToping { get; set; }

        public bool IsShowOption { get; set; }

        public bool IsShowThanksMessage { get; set; }

        public string ThanksMessageData { get; set; }

        public bool IsShowWifiAndPassword { get; set; }

        public string WifiData { get; set; }

        public string PasswordData { get; set; }

        public bool IsShowQRCode { get; set; }

        public string QRCodeData { get; set; }

        public string QRCodeThumbnail { get; set; }

        public bool IsDefault { get; set; }
    }
}
