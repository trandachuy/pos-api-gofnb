using System;

namespace GoFoodBeverage.Models.Package
{
    /// <summary>
    /// The order package model response to the internal tool data table
    /// </summary>
    public class OrderPackageInternalToolDatatableModel
    {
        /// <summary>
        /// The code of order package
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of order package
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Total price of order package
        /// </summary>
        public int TotalAmount { get; set; }

        /// <summary>
        /// The payment status is Paid or UnPaid, default is FALSE: UnPaid
        /// </summary>
        public bool Paid { get; set; }

        /// <summary>
        /// The store code
        /// </summary>
        public int StoreId { get; set; }

        /// <summary>
        /// The customer code
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// The payment method name of EnumPackagePaymentMethod
        /// </summary>
        public string PaymentMethod { get; set; }

        /// <summary>
        /// The order package type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The order package status:
        /// PENDING, APPROVED, CANCELED
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// The fee of order package
        /// </summary>
        public int SetupFee { get; set; }

        /// <summary>
        /// The bought package id
        /// </summary>
        public Guid BoughtPackage { get; set; }

        /// <summary>
        /// The Package duration by month
        /// </summary>
        public int NumberMonth { get; set; }

        /// <summary>
        /// The currency code
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Store name
        /// </summary>
        public string ShopName { get; set; }

        /// <summary>
        /// The phone number of account create the order package
        /// </summary>
        public string ShopPhoneNumber { get; set; }

        /// <summary>
        /// Store owner
        /// </summary>
        public string SellerName { get; set; }

        public string Email { get; set; }

        #region Response data from internal tool
        /// <summary>
        /// The contract id response from internal tool
        /// </summary>
        public string ContractId { get; set; }

        /// <summary>
        /// The note response from internal tool
        /// </summary>
        public string Note { get; set; }
        #endregion

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public Guid? CreatedUserId { get; set; }
    }
}
