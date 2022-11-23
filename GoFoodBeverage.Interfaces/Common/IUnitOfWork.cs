using GoFoodBeverage.Application.Interfaces.Repositories;
using GoFoodBeverage.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using System.Threading.Tasks;

namespace GoFoodBeverage.Interfaces
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsync();

        Task<IDbContextTransaction> BeginTransactionAsync();

        IAccountRepository Accounts { get; }

        IAccountTypeRepository AccountTypes { get; }

        IAddressRepository Addresses { get; }

        IBusinessAreaRepository BusinessAreas { get; }

        ICityRepository Cities { get; }

        IComboRepository Combos { get; }

        IComboPricingRepository ComboPricings { get; }

        IComboPricingProductPriceRepository ComboPricingProductPrices { get; }

        IComboProductPriceRepository ComboProductPrices { get; }

        IComboProductGroupRepository ComboProductGroups { get; }

        IComboProductGroupProductPriceRepository ComboProductGroupProductPrices { get; }

        IComboStoreBrancheRepository ComboStoreBranches { get; }

        ICountryRepository Countries { get; }

        ICurrencyRepository Currencies { get; }

        ICustomerRepository Customers { get; }

        IAccountAddressRepository AccountAddresses { get; }

        IDeliveryConfigRepository DeliveryConfigs { get; }

        IDeliveryConfigPricingRepository DeliveryConfigPricings { get; }

        IDeliveryMethodRepository DeliveryMethods { get; }

        IDistrictRepository Districts { get; }

        IFavoriteStoreRepository FavoriteStores { get; }

        IFeeRepository Fees { get; }

        IGroupPermissionPermissionRepository GroupPermissionPermissions { get; }

        IMaterialRepository Materials { get; }

        IMaterialCategoryRepository MaterialCategories { get; }

        IMaterialInventoryBranchRepository MaterialInventoryBranches { get; }

        IMaterialInventoryCheckingRepository MaterialInventoryCheckings { get; }

        IOptionRepository Options { get; }

        IOptionLevelRepository OptionLevels { get; }

        IOrderFeeRepository OrderFees { get; }

        IPaymentConfigRepository PaymentConfigs { get; }

        IPaymentMethodRepository PaymentMethods { get; }

        IGroupPermissionRepository GroupPermissions { get; }

        IGroupPermissionBranchRepository GroupPermissionBranches { get; }

        IPermissionRepository Permissions { get; }

        IPermissionGroupRepository PermissionGroups { get; }

        IProductRepository Products { get; }

        IProductChannelRepository ProductChannels { get; }

        IProductPriceRepository ProductPrices { get; }

        IProductCategoryRepository ProductCategories { get; }

        IPromotionRepository Promotions { get; }

        IPromotionBranchRepository PromotionBranches { get; }

        IPromotionProductRepository PromotionProducts { get; }

        IPromotionProductCategoryRepository PromotionProductCategories { get; }

        IProductProductCategoryRepository ProductProductCategories { get; }

        IProductPriceMaterialRepository ProductPriceMaterials { get; }

        IPurchaseOrderRepostiory PurchaseOrders { get; }

        IPurchaseOrderMaterialRepostiory PurchaseOrderMaterials { get; }

        IStaffRepository Staffs { get; }

        IStaffGroupPermissionBranchRepository StaffGroupPermissionBranches { get; }

        IStateRepository States { get; }

        IStoreRepository Stores { get; }

        IStoreBranchRepository StoreBranches { get; }

        IStoreBankAccountRepository StoreBankAccounts { get; }

        ISupplierRepository Suppliers { get; }

        IUnitRepository Units { get; }

        IUnitConversionRepository UnitConversions { get; }

        IStoreBranchProductCategoryRepository StoreBranchProductCategories { get; }

        IUserActivityRepository UserActivities { get; }

        IWardRepository Wards { get; }

        ILanguageRepository Languages { get; }

        ILanguageStoreRepository LanguageStores { get; }

        IAreaTableRepository AreaTables { get; }

        IAreaRepository Areas { get; }

        ICustomerSegmentRepository CustomerSegments { get; }

        ICustomerSegmentConditionRepository CustomerSegmentConditions { get; }

        ICustomerCustomerSegmentRepository CustomerCustomerSegments { get; }

        ICustomerMembershipRepository CustomerMemberships { get; }

        ICustomerPointRepository CustomerPoints { get; }

        ILoyaltyPointConfigRepository LoyaltyPointsConfigs { get; }

        IOrderRepository Orders { get; }

        IOrderRestoreRepository OrderRestores { get; }

        IOrderItemRepository OrderItems { get; }

        IOrderItemRestoreRepository OrderItemRestores { get; }

        IShiftRepository Shifts { get; }

        ITaxRepository Taxes { get; }

        IOrderItemOptionRepository OrderItemOptions { get; }

        IOrderItemToppingRepository OrderItemToppings { get; }

        IOrderPaymentTransactionRepository OrderPaymentTransactions { get; }

        IStampConfigRepository StampConfigs { get; }

        IBarcodeConfigRepository BarcodeConfigs { get; }

        IBillRepository Bills { get; }

        IOrderSessionRepository OrderSessions { get; }

        IProductPlatformRepository ProductPlatforms { get; }

        IPlatformRepository Platforms { get; }

        IOrderHistoryRepository OrderHistories { get; }

        IPackageRepository Packages { get; }

        IPackageDurationByMonthRepository PackageDurationByMonths { get; }

        IFunctionGroupRepository FunctionGroups { get; }

        IOrderPackageRepository OrderPackages { get; }

        IAccountTransferRepository AccountTransfers { get; }

        IInternalAccountRepository InternalAccounts { get; }

        IOrderComboProductPriceItemRepository OrderComboProductPriceItems { get; }

        IFeeBranchRepository FeeBranches { get; }

        IFileUploadRepository FileUpload { get; }

        IProductToppingRepository ProductToppings { get; }

        IProductOptionRepository ProductOptions { get; }

        IStaffActivityRepository StaffActivities { get; }

        IFeeServingTypeRepository FeeServingTypes { get; }

        IStoreBannerRepository StoreBanners { get; }

        IStoreConfigRepository StoreConfigs { get; }

        IQRCodeRepostiory QRCodes { get; }

        IQRCodeProductRepostiory QrCodeProducts { get; }

        IThemeRepository Themes { get; }

        IStoreThemeRepository StoreThemes { get; }

        IOrderComboItemRepository OrderComboItems { get; }

        IMaterialInventoryHistoryRepository MaterialInventoryHistories { get; }

        IAccountSearchHistoryRepository AccountSearchHistories { get; }

        IEmailCampaignRepostiory EmailCampaigns { get; }

        IEmailCampaignCustomerSegmentRepostiory EmailCampaignCustomerSegments { get; }

        IEmailCampaignDetailRepostiory EmailCampaignDetails { get; }

        IEmailCampaignSocialRepostiory EmailCampaignSocials { get; }

        IEmailCampaignSendingTransactionRepostiory EmailCampaignSendingTransactions { get; }

        IPointHistoryRepository PointHistories { get; }

        IPageRepository Pages { get; }

        IOnlineStoreMenuRepostiory OnlineStoreMenus { get; }

        IOnlineStoreMenuItemRepostiory OnlineStoreMenuItems { get; }

        IOrderDeliveryTransactionRepository OrderDeliveryTransactions { get; }
    }
}
