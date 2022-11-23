using System;
using GoFoodBeverage.Domain.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(Store))]
    public class Store : BaseEntity
    {
        public Guid InitialStoreAccountId { get; set; } // Reference to account table

        public Guid CurrencyId { get; set; }

        public Guid AddressId { get; set; }

        public Guid BusinessAreaId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Code { get; set; }

        public string Logo { get; set; }

        [MaxLength(100)]
        public string Title { get; set; } // Store name

        public bool IsStoreHasKitchen { get; set; }

        public bool IsAutoPrintStamp { get; set; }

        [Description("Customer will pay after finishing the meal (after create the order)")]
        public bool IsPaymentLater { get; set; }

        [Description("Allowing to sell products when out of materials")]
        public bool IsCheckProductSell { get; set; }

        public bool IsActivated { get; set; }

        public Guid? ActivatedByOrderPackageId { get; set; }

        public Address Address { get; set; }

        public virtual Currency Currency { get; set; }

        public virtual BusinessArea BusinessArea { get; set; }

        public virtual ICollection<Staff> Staffs { get; set; }

        public virtual ICollection<Customer> Customers { get; set; }

        public virtual ICollection<GroupPermission> GroupPermissions { get; set; }

        public virtual ICollection<Product> Products { get; set; }

        public virtual ICollection<Unit> Units { get; set; }

        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; }

        public virtual ICollection<PaymentConfig> PaymentConfigs { get; set; }

        public virtual ICollection<StoreBankAccount> StoreBankAccounts { get; set; }

        public virtual ICollection<MaterialInventoryChecking> MaterialInventoryCheckings { get; set; }

        public virtual ICollection<FavoriteStore> FavoriteStores { get; set; }

        public virtual ICollection<StoreBranch> StoreBranches { get; set; }

        public virtual ICollection<StaffActivity> StaffActivities { get; set; }

        public virtual ICollection<StoreBanner> StoreBanners { get; set; }

        public virtual ICollection<StoreTheme> StoreThemes { get; set; }
    }
}
