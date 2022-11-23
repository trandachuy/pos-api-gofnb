using GoFoodBeverage.Common.Constants;
using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Infrastructure.Contexts
{
    public class GoFoodBeverageDbContext : DbContext
    {
        private readonly IDateTimeService _dateTime;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GoFoodBeverageDbContext(
            DbContextOptions<GoFoodBeverageDbContext> options,
            IDateTimeService dateTime,
            IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _dateTime = dateTime;
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountType> AccountTypes { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<BusinessArea> BusinessAreas { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Combo> Combos { get; set; }
        public DbSet<ComboProductGroupProductPrice> ComboProductGroupProductPrices { get; set; }
        public DbSet<ComboPricing> ComboPricings { get; set; }
        public DbSet<ComboPricingProductPrice> ComboPricingProducts { get; set; }
        public DbSet<ComboProductPrice> ComboProductPrices { get; set; }
        public DbSet<ComboProductGroup> ComboProductGroups { get; set; }
        public DbSet<ComboStoreBranch> ComboStoreBranches { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<AccountAddress> CustomerAddresses { get; set; }
        public DbSet<CustomerCustomerSegment> CustomerCustomerSegments { get; set; }
        public DbSet<CustomerSegment> CustomerSegments { get; set; }
        public DbSet<CustomerSegmentCondition> CustomerSegmentConditions { get; set; }
        public DbSet<DeliveryConfig> DeliveryConfigs { get; set; }
        public DbSet<DeliveryConfigPricing> DeliveryConfigPricings { get; set; }
        public DbSet<DeliveryMethod> DeliveryMethods { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<FavoriteStore> FavoriteStores { get; set; }
        public DbSet<Fee> Fees { get; set; }
        public DbSet<FeeBranch> FeeBranches { get; set; }
        public DbSet<GroupPermission> GroupPermissions { get; set; }
        public DbSet<GroupPermissionBranch> GroupPermissionBranches { get; set; }
        public DbSet<GroupPermissionPermission> GroupPermissionPermissions { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<LanguageStore> LanguageStores { get; set; }
        public DbSet<LoyaltyPointConfig> LoyaltyPointConfigs { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<MaterialCategory> MaterialCategories { get; set; }
        public DbSet<MaterialInventoryBranch> MaterialInventoryBranches { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<OptionLevel> OptionLevels { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderCartSession> OrderCartSessions { get; set; }
        public DbSet<OrderComboItem> OrderComboItems { get; set; }
        public DbSet<OrderComboProductPriceItem> OrderComboProductPriceItems { get; set; }
        public DbSet<OrderRestore> OrderRestores { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderItemRestore> OrderItemRestores { get; set; }
        public DbSet<OrderItemOption> OrderItemOptions { get; set; }
        public DbSet<OrderItemTopping> OrderItemToppings { get; set; }
        public DbSet<OrderPaymentTransaction> OrderPaymentTransactions { get; set; }
        public DbSet<OrderDelivery> OrderDeliveries { get; set; }
        public DbSet<OrderFee> OrderFees { get; set; }
        public DbSet<PaymentConfig> PaymentConfigs { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<PermissionGroup> PermissionGroups { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductChannel> ProductChannels { get; set; }
        public DbSet<ProductOption> ProductOptions { get; set; }
        public DbSet<ProductPrice> ProductPrices { get; set; }
        public DbSet<ProductPriceMaterial> ProductPriceMaterials { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<PromotionBranch> PromotionBranches { get; set; }
        public DbSet<PromotionProduct> PromotionProducts { get; set; }
        public DbSet<PromotionProductCategory> PromotionProductCategories { get; set; }
        public DbSet<ProductProductCategory> ProductProductCategories { get; set; }
        public DbSet<ProductTopping> ProductToppings { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderMaterial> PurchaseOrderMaterials { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<StaffGroupPermissionBranch> StaffGroupPermissionBranches { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<StoreBankAccount> StoreBankAccounts { get; set; }
        public DbSet<StoreBranch> StoreBranches { get; set; }
        public DbSet<StoreBranchProductCategory> StoreBranchProductCategories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Tax> Taxes { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<UnitConversion> UnitConversions { get; set; }
        public DbSet<UserActivity> UserActivities { get; set; }
        public DbSet<Ward> Wards { get; set; }
        public DbSet<Platform> Platforms { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<AreaTable> AreaTables { get; set; }
        public DbSet<CustomerPoint> CustomerPoints { get; set; }
        public DbSet<CustomerMembershipLevel> CustomerMembershipLevels { get; set; }
        public DbSet<StampConfig> StampConfigs { get; set; }
        public DbSet<BarcodeConfig> BarcodeConfigs { get; set; }
        public DbSet<BillConfiguration> BillConfigurations { get; set; }
        public DbSet<OrderSession> OrderSessions { get; set; }
        public DbSet<ProductPlatform> ProductPlatforms { get; set; }
        public DbSet<OrderHistory> OrderHistories { get; set; }
        public DbSet<PackageDurationByMonth> PackageDurationByMonths { get; set; }
        public DbSet<PackageFunction> PackageFunctionGroups { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Function> Functions { get; set; }
        public DbSet<FunctionGroup> FunctionGroups { get; set; }
        public DbSet<FunctionPermission> FunctionPermissions { get; set; }
        public DbSet<OrderPackage> OrderPackages { get; set; }
        public DbSet<AccountTransfer> AccountTransfers { get; set; }
        public DbSet<InternalAccount> InternalAccounts { get; set; }
        public DbSet<FileUpload> FilesUpload { get; set; }
        public DbSet<FeeServingType> FeeServingTypes { get; set; }
        public DbSet<StoreBanner> StoreBanners { get; set; }
        public DbSet<StoreConfig> StoreConfigs { get; set; }
        public DbSet<QRCode> QRCodes { get; set; }
        public DbSet<QRCodeProduct> QRCodeProducts { get; set; }
        public DbSet<Theme> Themes { get; set; }
        public DbSet<StoreTheme> StoreThemes { get; set; }
        public DbSet<AccountSearchHistory> AccountSearchHistories { get; set; }
        public DbSet<EmailCampaign> EmailCampaigns { get; set; }
        public DbSet<EmailCampaignCustomerSegment> EmailCampaignCustomerSegments { get; set; }
        public DbSet<EmailCampaignDetail> EmailCampaignDetails { get; set; }
        public DbSet<EmailCampaignSocial> EmailCampaignSocials { get; set; }
        public DbSet<EmailCampaignSendingTransaction> EmailCampaignSendingTransactions { get; set; }
        public DbSet<PointHistory> PointHistories { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<OnlineStoreMenu> OnlineStoreMenus { get; set; }
        public DbSet<OnlineStoreMenuItem> OnlineStoreMenuItems { get; set; }
        public DbSet<OrderDeliveryTransaction> OrderDeliveryTransactions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<GroupPermissionPermission>().HasKey(x => new { x.GroupPermissionId, x.PermissionId });
            builder.Entity<GroupPermissionPermission>()
                .HasOne<Permission>(sc => sc.Permission)
                .WithMany(s => s.GroupPermissionPermissions)
                .HasForeignKey(sc => sc.PermissionId);
            builder.Entity<GroupPermissionPermission>()
                .HasOne<GroupPermission>(sc => sc.GroupPermission)
                .WithMany(s => s.GroupPermissionPermissions)
                .HasForeignKey(sc => sc.GroupPermissionId);

            builder.Entity<PurchaseOrderMaterial>().HasKey(x => new { x.PurchaseOrderId, x.MaterialId });
            builder.Entity<PurchaseOrderMaterial>()
                .HasOne<PurchaseOrder>(sc => sc.PurchaseOrder)
                .WithMany(s => s.PurchaseOrderMaterials)
                .HasForeignKey(sc => sc.PurchaseOrderId);
            builder.Entity<PurchaseOrderMaterial>()
                .HasOne<Material>(sc => sc.Material)
                .WithMany(s => s.PurchaseOrderMaterials)
                .HasForeignKey(sc => sc.MaterialId);

            builder.Entity<ProductProductCategory>().HasKey(x => new { x.ProductId, x.ProductCategoryId });
            builder.Entity<ProductProductCategory>()
                .HasOne<Product>(sc => sc.Product)
                .WithMany(s => s.ProductProductCategories)
                .HasForeignKey(sc => sc.ProductId);
            builder.Entity<ProductProductCategory>()
                .HasOne<ProductCategory>(sc => sc.ProductCategory)
                .WithMany(s => s.ProductProductCategories)
                .HasForeignKey(sc => sc.ProductCategoryId);

            builder.Entity<StoreBranchProductCategory>().HasKey(x => new { x.StoreBranchId, x.ProductCategoryId });
            builder.Entity<StoreBranchProductCategory>()
                .HasOne<StoreBranch>(sc => sc.StoreBranch)
                .WithMany(s => s.StoreBranchProductCategories)
                .HasForeignKey(sc => sc.StoreBranchId);
            builder.Entity<StoreBranchProductCategory>()
                .HasOne<ProductCategory>(sc => sc.ProductCategory)
                .WithMany(s => s.StoreBranchProductCategories)
                .HasForeignKey(sc => sc.ProductCategoryId);

            builder.Entity<ProductOption>().HasKey(x => new { x.ProductId, x.OptionId });
            builder.Entity<ProductOption>()
                .HasOne<Product>(sc => sc.Product)
                .WithMany(s => s.ProductOptions)
                .HasForeignKey(sc => sc.ProductId);
            builder.Entity<ProductOption>()
                .HasOne<Option>(sc => sc.Option)
                .WithMany(s => s.ProductOptions)
                .HasForeignKey(sc => sc.OptionId);

            builder.Entity<ProductPriceMaterial>().HasKey(x => new { x.ProductPriceId, x.MaterialId });
            builder.Entity<ProductPriceMaterial>()
                .HasOne<ProductPrice>(sc => sc.ProductPrice)
                .WithMany(s => s.ProductPriceMaterials)
                .HasForeignKey(sc => sc.ProductPriceId);
            builder.Entity<ProductPriceMaterial>()
                .HasOne<Material>(sc => sc.Material)
                .WithMany(s => s.ProductPriceMaterials)
                .HasForeignKey(sc => sc.MaterialId);

            builder.Entity<PromotionProduct>().HasKey(x => new { x.PromotionId, x.ProductId });
            builder.Entity<PromotionProduct>()
                .HasOne<Promotion>(sc => sc.Promotion)
                .WithMany(s => s.PromotionProducts)
                .HasForeignKey(sc => sc.PromotionId);
            builder.Entity<PromotionProduct>()
                .HasOne<Product>(sc => sc.Product)
                .WithMany(s => s.PromotionProducts)
                .HasForeignKey(sc => sc.ProductId);

            builder.Entity<PromotionProductCategory>().HasKey(x => new { x.PromotionId, x.ProductCategoryId });
            builder.Entity<PromotionProductCategory>()
                .HasOne<Promotion>(sc => sc.Promotion)
                .WithMany(s => s.PromotionProductCategories)
                .HasForeignKey(sc => sc.PromotionId);
            builder.Entity<PromotionProductCategory>()
                .HasOne<ProductCategory>(sc => sc.ProductCategory)
                .WithMany(s => s.PromotionProductCategories)
                .HasForeignKey(sc => sc.ProductCategoryId);

            builder.Entity<PromotionBranch>().HasKey(x => new { x.PromotionId, x.BranchId });
            builder.Entity<PromotionBranch>()
                .HasOne<Promotion>(sc => sc.Promotion)
                .WithMany(s => s.PromotionBranches)
                .HasForeignKey(sc => sc.PromotionId);
            builder.Entity<PromotionBranch>()
                .HasOne<StoreBranch>(sc => sc.Branch)
                .WithMany(s => s.PromotionBranches)
                .HasForeignKey(sc => sc.BranchId);

            builder.Entity<ComboStoreBranch>().HasKey(x => new { x.ComboId, x.BranchId });
            builder.Entity<ComboStoreBranch>()
                .HasOne<Combo>(sc => sc.Combo)
                .WithMany(s => s.ComboStoreBranches)
                .HasForeignKey(sc => sc.ComboId);
            builder.Entity<ComboStoreBranch>()
                .HasOne<StoreBranch>(sc => sc.Branch)
                .WithMany(s => s.ComboStoreBranches)
                .HasForeignKey(sc => sc.BranchId);

            builder.Entity<ComboProductPrice>().HasKey(x => new { x.ComboId, x.ProductPriceId });
            builder.Entity<ComboProductPrice>()
                .HasOne<Combo>(sc => sc.Combo)
                .WithMany(s => s.ComboProductPrices)
                .HasForeignKey(sc => sc.ComboId);
            builder.Entity<ComboProductPrice>()
                .HasOne<ProductPrice>(sc => sc.ProductPrice)
                .WithMany(s => s.ComboProductPrices)
                .HasForeignKey(sc => sc.ProductPriceId);

            builder.Entity<CustomerCustomerSegment>().HasKey(x => new { x.CustomerId, x.CustomerSegmentId });
            builder.Entity<CustomerCustomerSegment>()
                .HasOne<Customer>(sc => sc.Customer)
                .WithMany(s => s.CustomerCustomerSegments)
                .HasForeignKey(sc => sc.CustomerId);
            builder.Entity<CustomerCustomerSegment>()
                .HasOne<CustomerSegment>(sc => sc.CustomerSegment)
                .WithMany(s => s.CustomerCustomerSegments)
                .HasForeignKey(sc => sc.CustomerSegmentId);

            builder.Entity<FeeBranch>().HasKey(x => new { x.BranchId, x.FeeId });
            builder.Entity<FeeBranch>()
                .HasOne(sc => sc.Branch)
                .WithMany(s => s.FeeBranches)
                .HasForeignKey(sc => sc.BranchId);
            builder.Entity<FeeBranch>()
                .HasOne(sc => sc.Fee)
                .WithMany(s => s.FeeBranches)
                .HasForeignKey(sc => sc.FeeId);

            builder.Entity<ProductPlatform>().HasKey(x => new { x.ProductId, x.PlatformId });
            builder.Entity<ProductPlatform>()
                .HasOne(sc => sc.Product)
                .WithMany(s => s.ProductPlatforms)
                .HasForeignKey(sc => sc.ProductId);
            builder.Entity<ProductPlatform>()
                .HasOne(sc => sc.Platform)
                .WithMany(s => s.ProductPlatforms)
                .HasForeignKey(sc => sc.PlatformId);

            builder.Entity<FunctionPermission>().HasKey(x => new { x.FunctionId, x.PermissionId });
            builder.Entity<FunctionPermission>()
               .HasOne(sc => sc.Function)
               .WithMany(s => s.FunctionPermissions)
               .HasForeignKey(sc => sc.FunctionId);
            builder.Entity<FunctionPermission>()
                .HasOne(sc => sc.Permission)
                .WithMany(s => s.FunctionPermissions)
                .HasForeignKey(sc => sc.PermissionId);

            builder.Entity<PackageFunction>().HasKey(x => new { x.PackageId, x.FunctionId });
            builder.Entity<PackageFunction>()
               .HasOne(sc => sc.Package)
               .WithMany(s => s.PackageFunctions)
               .HasForeignKey(sc => sc.PackageId);
            builder.Entity<PackageFunction>()
                .HasOne(sc => sc.Function)
                .WithMany(s => s.PackageFunctions)
                .HasForeignKey(sc => sc.FunctionId);

            builder.Entity<StoreTheme>().HasKey(x => new { x.StoreId, x.ThemeId });
            builder.Entity<StoreTheme>()
                .HasOne(sc => sc.Store)
                .WithMany(s => s.StoreThemes)
                .HasForeignKey(sc => sc.StoreId);
            builder.Entity<StoreTheme>()
                .HasOne(sc => sc.Theme)
                .WithMany(s => s.StoreThemes)
                .HasForeignKey(sc => sc.ThemeId);

            builder.Entity<OnlineStoreMenuItem>()
                .HasOne(sc => sc.OnlineStoreMenu)
                .WithMany(s => s.OnlineStoreMenuItems)
                .HasForeignKey(sc => sc.MenuId);

            builder.Entity<ProductTopping>().HasKey(x => new { x.ProductId, x.ToppingId });

            builder.Entity<Customer>().Property(u => u.Code).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            builder.Entity<Material>().Property(u => u.Code).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            builder.Entity<MaterialCategory>().Property(u => u.Code).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            builder.Entity<StoreBranch>().Property(u => u.Code).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            builder.Entity<Unit>().Property(u => u.Code).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            builder.Entity<Store>().Property(u => u.Code).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            builder.Entity<Account>().Property(u => u.Code).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            builder.Entity<Product>().Property(u => u.Code).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            builder.Entity<OrderPackage>().Property(u => u.Code).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            /// Add comment to column from description attribute
            foreach (var property in builder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties()))
            {
                if (property.PropertyInfo == null) continue;

                var comment = string.Empty;
                var customAttributes = property.PropertyInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (customAttributes.Length > 0)
                {
                    comment = ((DescriptionAttribute)customAttributes[0]).Description;
                }

                if (string.IsNullOrEmpty(comment)) continue;

                property.SetComment(comment);
            }

            base.OnModelCreating(builder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var claimAccountId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypesConstants.ACCOUNT_ID);
            var accountId = claimAccountId?.Value.ToGuid();
            foreach (var entry in ChangeTracker.Entries<GoFoodBeverage.Domain.Base.BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedTime = _dateTime.NowUtc;
                        entry.Entity.LastSavedTime = _dateTime.NowUtc;
                        entry.Entity.CreatedUser = accountId;
                        entry.Entity.LastSavedUser = accountId;
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastSavedTime = _dateTime.NowUtc;
                        entry.Entity.LastSavedUser = accountId;
                        break;
                }
            }

            foreach (var entry in ChangeTracker.Entries<GoFoodBeverage.Domain.Base.BaseAuditEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedTime = _dateTime.NowUtc;
                        entry.Entity.LastSavedTime = _dateTime.NowUtc;
                        entry.Entity.CreatedUser = accountId;
                        entry.Entity.LastSavedUser = accountId;
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastSavedTime = _dateTime.NowUtc;
                        entry.Entity.LastSavedUser = accountId;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
