using GoFoodBeverage.Application.Interfaces.Repositories;
using GoFoodBeverage.Infrastructure.Contexts;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading.Tasks;

namespace GoFoodBeverage.Infrastructure.Repositories
{
    /// <summary>
    /// Repository pattern and unit of work
    /// more detail: https://dev.to/moe23/step-by-step-repository-pattern-and-unit-of-work-with-asp-net-core-5-3l92
    /// </summary>
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly GoFoodBeverageDbContext _dbContext;

        public IAccountRepository Accounts { get; private set; }

        public IAccountAddressRepository AccountAddresses { get; private set; }

        public IAccountTypeRepository AccountTypes { get; private set; }

        public IAddressRepository Addresses { get; private set; }

        public IBusinessAreaRepository BusinessAreas { get; private set; }

        public ICityRepository Cities { get; private set; }

        public IComboRepository Combos { get; private set; }

        public IComboPricingRepository ComboPricings { get; private set; }

        public IComboPricingProductPriceRepository ComboPricingProductPrices { get; private set; }

        public IComboProductGroupRepository ComboProductGroups { get; private set; }

        public IComboProductGroupProductPriceRepository ComboProductGroupProductPrices { get; private set; }

        public IComboProductPriceRepository ComboProductPrices { get; private set; }

        public IComboStoreBrancheRepository ComboStoreBranches { get; private set; }

        public ICountryRepository Countries { get; private set; }

        public ICurrencyRepository Currencies { get; private set; }

        public ICustomerRepository Customers { get; private set; }

        public IDeliveryConfigRepository DeliveryConfigs { get; private set; }

        public IDeliveryConfigPricingRepository DeliveryConfigPricings { get; private set; }

        public IDeliveryMethodRepository DeliveryMethods { get; private set; }

        public IDistrictRepository Districts { get; private set; }

        public IFavoriteStoreRepository FavoriteStores { get; private set; }

        public IFeeRepository Fees { get; private set; }

        public IGroupPermissionRepository GroupPermissions { get; private set; }

        public IGroupPermissionBranchRepository GroupPermissionBranches { get; private set; }

        public IGroupPermissionPermissionRepository GroupPermissionPermissions { get; private set; }

        public IMaterialRepository Materials { get; private set; }

        public IMaterialCategoryRepository MaterialCategories { get; private set; }

        public IMaterialInventoryBranchRepository MaterialInventoryBranches { get; private set; }

        public IMaterialInventoryCheckingRepository MaterialInventoryCheckings { get; private set; }

        public IPermissionRepository Permissions { get; private set; }

        public IOptionRepository Options { get; private set; }

        public IOptionLevelRepository OptionLevels { get; private set; }

        public IPaymentConfigRepository PaymentConfigs { get; private set; }

        public IPaymentMethodRepository PaymentMethods { get; private set; }

        public IPermissionGroupRepository PermissionGroups { get; private set; }

        public IProductRepository Products { get; private set; }

        public IProductChannelRepository ProductChannels { get; private set; }

        public IProductPriceRepository ProductPrices { get; private set; }

        public IProductCategoryRepository ProductCategories { get; private set; }

        public IProductProductCategoryRepository ProductProductCategories { get; private set; }

        public IProductPriceMaterialRepository ProductPriceMaterials { get; private set; }

        public IPromotionRepository Promotions { get; private set; }

        public IPromotionBranchRepository PromotionBranches { get; private set; }

        public IPromotionProductRepository PromotionProducts { get; private set; }

        public IPromotionProductCategoryRepository PromotionProductCategories { get; private set; }

        public IPurchaseOrderRepostiory PurchaseOrders { get; private set; }

        public IPurchaseOrderMaterialRepostiory PurchaseOrderMaterials { get; private set; }

        public IStaffRepository Staffs { get; private set; }

        public IStaffGroupPermissionBranchRepository StaffGroupPermissionBranches { get; private set; }

        public IStateRepository States { get; private set; }

        public IStoreRepository Stores { get; private set; }

        public IStoreBranchRepository StoreBranches { get; private set; }

        public IStoreBankAccountRepository StoreBankAccounts { get; private set; }

        public ISupplierRepository Suppliers { get; private set; }

        public IUnitRepository Units { get; private set; }

        public IUnitConversionRepository UnitConversions { get; private set; }

        public IStoreBranchProductCategoryRepository StoreBranchProductCategories { get; private set; }

        public IUserActivityRepository UserActivities { get; private set; }

        public IWardRepository Wards { get; private set; }

        public ILanguageRepository Languages { get; private set; }

        public ILanguageStoreRepository LanguageStores { get; private set; }

        public IAreaTableRepository AreaTables { get; private set; }

        public IAreaRepository Areas { get; private set; }

        public ICustomerCustomerSegmentRepository CustomerCustomerSegments { get; private set; }

        public ICustomerSegmentRepository CustomerSegments { get; private set; }

        public ICustomerSegmentConditionRepository CustomerSegmentConditions { get; private set; }

        public ICustomerMembershipRepository CustomerMemberships { get; private set; }

        public ICustomerPointRepository CustomerPoints { get; private set; }

        public ILoyaltyPointConfigRepository LoyaltyPointsConfigs { get; private set; }

        public IOrderRepository Orders { get; private set; }

        public IOrderRestoreRepository OrderRestores { get; private set; }

        public IOrderFeeRepository OrderFees { get; private set; }

        public IOrderItemRepository OrderItems { get; private set; }

        public IOrderItemRestoreRepository OrderItemRestores { get; private set; }

        public IShiftRepository Shifts { get; private set; }

        public ITaxRepository Taxes { get; private set; }

        public IOrderItemOptionRepository OrderItemOptions { get; }

        public IOrderItemToppingRepository OrderItemToppings { get; }

        public IOrderPaymentTransactionRepository OrderPaymentTransactions { get; private set; }

        public IStampConfigRepository StampConfigs { get; private set; }

        public IBarcodeConfigRepository BarcodeConfigs { get; private set; }

        public IBillRepository Bills { get; private set; }

        public IOrderSessionRepository OrderSessions { get; private set; }

        public IPlatformRepository Platforms { get; private set; }

        public IProductPlatformRepository ProductPlatforms { get; private set; }

        public IOrderHistoryRepository OrderHistories { get; private set; }

        public IPackageRepository Packages { get; private set; }

        public IPackageDurationByMonthRepository PackageDurationByMonths { get; private set; }

        public IFunctionGroupRepository FunctionGroups { get; private set; }

        public IOrderPackageRepository OrderPackages { get; private set; }

        public IAccountTransferRepository AccountTransfers { get; private set; }

        public IInternalAccountRepository InternalAccounts { get; private set; }

        public IOrderComboProductPriceItemRepository OrderComboProductPriceItems { get; private set; }

        public IFeeBranchRepository FeeBranches { get; private set; }

        public IFileUploadRepository FileUpload { get; private set; }

        public IProductToppingRepository ProductToppings { get; private set; }

        public IProductOptionRepository ProductOptions { get; private set; }

        public IStaffActivityRepository StaffActivities { get; private set; }

        public IFeeServingTypeRepository FeeServingTypes { get; }

        public IStoreBannerRepository StoreBanners { get; private set; }

        public IStoreConfigRepository StoreConfigs { get; private set; }

        public IQRCodeRepostiory QRCodes { get; private set; }

        public IQRCodeProductRepostiory QrCodeProducts { get; private set; }

        public IThemeRepository Themes { get; private set; }

        public IStoreThemeRepository StoreThemes { get; private set; }

        public IOrderComboItemRepository OrderComboItems { get; private set; }

        public IMaterialInventoryHistoryRepository MaterialInventoryHistories { get; private set; }

        public IAccountSearchHistoryRepository AccountSearchHistories { get; private set; }

        public IEmailCampaignRepostiory EmailCampaigns { get; private set; }

        public IEmailCampaignCustomerSegmentRepostiory EmailCampaignCustomerSegments { get; private set; }

        public IEmailCampaignDetailRepostiory EmailCampaignDetails { get; private set; }

        public IEmailCampaignSocialRepostiory EmailCampaignSocials { get; private set; }

        public IEmailCampaignSendingTransactionRepostiory EmailCampaignSendingTransactions { get; private set; }
       
        public IPointHistoryRepository PointHistories { get; }      
        
        public IPageRepository Pages { get; private set; }

        public IOnlineStoreMenuRepostiory OnlineStoreMenus { get; private set; }

        public IOnlineStoreMenuItemRepostiory OnlineStoreMenuItems { get; private set; }

        public IOrderDeliveryTransactionRepository OrderDeliveryTransactions { get; private set; }

        public UnitOfWork(GoFoodBeverageDbContext dbContext)
        {
            _dbContext = dbContext;
            Accounts = new AccountRepository(dbContext);
            AccountAddresses = new AccountAddressRepository(dbContext);
            AccountTypes = new AccountTypeRepository(dbContext);
            Addresses = new AddressRepository(dbContext);
            BusinessAreas = new BusinessAreaRepository(dbContext);
            Cities = new CityRepository(dbContext);
            Combos = new ComboRepository(dbContext);
            ComboPricings = new ComboPricingRepository(dbContext);
            ComboPricingProductPrices = new ComboPricingProductPriceRepository(dbContext);
            ComboProductGroups = new ComboProductGroupRepository(dbContext);
            ComboProductGroupProductPrices = new ComboProductGroupProductPriceRepository(dbContext);
            ComboProductPrices = new ComboProductPriceRepository(dbContext);
            ComboStoreBranches = new ComboStoreBranchRepository(dbContext);
            Countries = new CountryRepository(dbContext);
            Currencies = new CurrencyRepository(dbContext);
            Customers = new CustomerRepository(dbContext);
            DeliveryConfigs = new DeliveryConfigRepository(dbContext);
            DeliveryConfigPricings = new DeliveryConfigPricingRepository(dbContext);
            DeliveryMethods = new DeliveryMethodRepository(dbContext);
            Districts = new DistrictRepository(dbContext);
            FavoriteStores = new FavoriteStoreRepository(dbContext);
            Fees = new FeeRepository(dbContext);
            GroupPermissionPermissions = new GroupPermissionPermissionRepository(dbContext);
            Materials = new MaterialRepository(dbContext);
            MaterialCategories = new MaterialCategoryRepository(dbContext);
            MaterialInventoryBranches = new MaterialInventoryBranchRepository(dbContext);
            MaterialInventoryCheckings = new MaterialInventoryCheckingRepository(dbContext);
            Options = new OptionRepository(dbContext);
            OptionLevels = new OptionLevelRepository(dbContext);
            PaymentConfigs = new PaymentConfigRepository(dbContext);
            PaymentMethods = new PaymentMethodRepository(dbContext);
            GroupPermissions = new GroupPermissionRepository(dbContext);
            GroupPermissionBranches = new GroupPermissionBranchRepository(dbContext);
            PermissionGroups = new PermissionGroupRepository(dbContext);
            Permissions = new PermissionRepository(dbContext);
            StaffGroupPermissionBranches = new StaffGroupPermissionBranchRepository(dbContext);
            Products = new ProductRepository(dbContext);
            ProductChannels = new ProductChannelRepository(dbContext);
            ProductPrices = new ProductPriceRepository(dbContext);
            ProductCategories = new ProductCategoryRepository(dbContext);
            ProductProductCategories = new ProductProductCategoryRepository(dbContext);
            ProductPriceMaterials = new ProductPriceMaterialRepository(dbContext);
            Promotions = new PromotionRepository(dbContext);
            PromotionBranches = new PromotionBranchRepository(dbContext);
            PromotionProducts = new PromotionProductRepository(dbContext);
            PromotionProductCategories = new PromotionProductCategoryRepository(dbContext);
            PurchaseOrders = new PurchaseOrderRepostiory(dbContext);
            PurchaseOrderMaterials = new PurchaseOrderMaterialRepostiory(dbContext);
            Staffs = new StaffRepository(dbContext);
            States = new StateRepository(dbContext);
            Stores = new StoreRepository(dbContext);
            StoreBranches = new StoreBranchRepository(dbContext);
            StoreBankAccounts = new StoreBankAccountRepository(dbContext);
            Suppliers = new SupplierRepository(dbContext);
            Units = new UnitRepository(dbContext);
            UnitConversions = new UnitConversionRepository(dbContext);
            StoreBranchProductCategories = new StoreBranchProductCategoryRepository(dbContext);
            UserActivities = new UserActivityRepository(dbContext);
            Wards = new WardRepository(dbContext);
            Languages = new LanguageRepository(dbContext);
            LanguageStores = new LanguageStoreRepository(dbContext);
            AreaTables = new AreaTableRepository(dbContext);
            Areas = new AreaRepository(dbContext);
            CustomerSegments = new CustomerSegmentRepository(dbContext);
            CustomerSegmentConditions = new CustomerSegmentConditionRepository(dbContext);
            CustomerCustomerSegments = new CustomerCustomerSegmentRepository(dbContext);
            CustomerMemberships = new CustomerMembershipRepository(dbContext);
            CustomerPoints = new CustomerPointRepository(dbContext);
            LoyaltyPointsConfigs = new LoyaltyPointConfigRepository(dbContext);
            Orders = new OrderRepository(dbContext);
            OrderRestores = new OrderRestoreRepository(dbContext);
            OrderItems = new OrderItemRepository(dbContext);
            OrderItemRestores = new OrderItemRestoreRepository(dbContext);
            Shifts = new ShiftRepository(dbContext);
            Taxes = new TaxRepository(dbContext);
            OrderItemOptions = new OrderItemOptionRepository(dbContext);
            OrderItemToppings = new OrderItemToppingRepository(dbContext);
            OrderPaymentTransactions = new OrderPaymentTransactionRepository(dbContext);
            OrderFees = new OrderFeeRepository(dbContext);
            StampConfigs = new StampConfigRepository(dbContext);
            BarcodeConfigs = new BarcodeConfigRepository(dbContext);
            Bills = new BillRepository(dbContext);
            OrderSessions = new OrderSessionRepository(dbContext);
            Platforms = new PlatformRepository(dbContext);
            ProductPlatforms = new ProductPlatformRepository(dbContext);
            OrderHistories = new OrderHistoryRepository(dbContext);
            Packages = new PackageRepository(dbContext);
            PackageDurationByMonths = new PackageDurationByMonthRepository(dbContext);
            FunctionGroups = new FunctionGroupRepository(dbContext);
            OrderPackages = new OrderPackageRepository(dbContext);
            AccountTransfers = new AccountTransferRepository(dbContext);
            InternalAccounts = new InternalAccountRepository(dbContext);
            OrderComboProductPriceItems = new OrderComboProductPriceItemRepository(dbContext);
            FeeBranches = new FeeBranchRepository(dbContext);
            ProductToppings = new ProductToppingRepository(dbContext);
            ProductOptions = new ProductOptionRepository(dbContext);
            FileUpload = new FileUploadRepository(dbContext);
            StaffActivities = new StaffActivityRepository(dbContext);
            FeeServingTypes = new FeeServingTypeRepository(dbContext);
            StoreBanners = new StoreBannerRepository(dbContext);
            StoreConfigs = new StoreConfigRepository(dbContext);
            QRCodes = new QRCodeRepository(dbContext);
            QrCodeProducts = new QRCodeProductRepository(dbContext);
            Themes = new ThemeRepository(dbContext);
            StoreThemes = new StoreThemeRepository(dbContext);
            OrderComboItems = new OrderComboItemRepository(dbContext);
            MaterialInventoryHistories = new MaterialInventoryHistoryRepository(dbContext);
            AccountSearchHistories = new AccountSearchHistoryRepository(dbContext);
            EmailCampaigns = new EmailCampaignRepository(dbContext);
            EmailCampaignCustomerSegments = new EmailCampaignCustomerSegmentRepository(dbContext);
            EmailCampaignDetails = new EmailCampaignDetailRepository(dbContext);
            EmailCampaignSocials = new EmailCampaignSocialRepository(dbContext);
            EmailCampaignSendingTransactions = new EmailCampaignSendingTransactionRepository(dbContext);
            PointHistories = new PointHistoryRepository(dbContext);
            Pages = new PageRepository(dbContext);
            OnlineStoreMenus = new OnlineStoreMenuRepository(dbContext);
            OnlineStoreMenuItems = new OnlineStoreMenuItemRepository(dbContext);
            OrderDeliveryTransactions = new OrderDeliveryTransactionRepository(dbContext);
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _dbContext.Database.BeginTransactionAsync();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
