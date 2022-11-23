using GoFoodBeverage.Domain.Base;
using GoFoodBeverage.Domain.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(OrderPackage))]
    public class OrderPackage : BaseEntity
    {
        public Guid? AccountId { get; set; }

        public Guid? StoreId { get; set; }

        public Guid? ActivateStorePackageId { get; set; }

        /// <summary>
        /// The database generates a value when a row is inserted.
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Code { get; set; }

        public int StoreCode { get; set; }

        [Description("Account code of the account create order package request")]
        public int AccountCode { get; set; }

        public Guid PackageId { get; set; }

        [Description("Number of months using the package")]
        public int PackageDurationByMonth { get; set; }

        [Description("The last total price of package has included tax amount")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string Title { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        [Description("The status of package is: PENDING/APPROVED/CANCELED")]
        public string Status { get; set; }

        public int SetupFee { get; set; }

        [Column(TypeName = "nvarchar(5)")]
        public string Currency { get; set; }

        [Description("Contract id will be updated from internal tool")]
        public string ContractId { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        [Description("Note will be updated from internal tool")]
        public string Note { get; set; }

        [Column(TypeName = "varchar(50)")]
        [Description("The email of the account create order package request")]
        public string Email { get; set; }

        [Column(TypeName = "varchar(15)")]
        public string ShopPhoneNumber { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        [Description("The name of the account create order package request")]
        public string SellerName { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string ShopName { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string CreatedBy { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string LastModifiedBy { get; set; }

        [Description("The last updated date or the activate package date")]
        public DateTime LastModifiedDate { get; set; }

        [Description("Package expiration date")]
        public DateTime? ExpiredDate { get; set; }

        [Description("Visa = 0 or ATM = 1 or BankTransfer = 2")]
        public EnumPackagePaymentMethod PackagePaymentMethod { get; set; }

        /// <summary>
        /// Package Payment Status
        /// </summary>
        [Description("Unpaid = 0 or Paid = 1")]
        public EnumOrderPaymentStatus PackageOderPaymentStatus { get; set; }

        [Description("StoreActivate = 0 or BranchPurchase = 1")]
        public EnumOrderPackageType OrderPackageType { get; set; }

        #region BranchPurchase
        [Description("Number of branches to buy")]
        public int BranchQuantity { get; set; }

        [Description("The price of a branch per year, the value related from the store package price per year")]
        public decimal BranchUnitPricePerYear { get; set; }

        [Description("Price of number of branches and number of years")]
        public decimal BranchPurchaseTotalPrice { get; set; }

        [Description("Remain amount = (Last package price / (number of years of package * 360) ) * Remain days")]
        public decimal BranchPurchaseRemainAmount { get; set; }
        #endregion

        public decimal TaxAmount { get; set; }

        [Description("Indicates the package is active to use")]
        public bool IsActivated { get; set; }

        public virtual Package Package { get; set; }

        public virtual OrderPackage ActivateStorePackage { get; set; }
    }
}
