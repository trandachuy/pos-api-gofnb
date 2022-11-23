using AutoMapper;
using GoFoodBeverage.Application.Features.Customer.Commands;
using GoFoodBeverage.Application.Features.Customer.Queries;
using GoFoodBeverage.Application.Features.EmailCampaigns.Commands;
using GoFoodBeverage.Application.Features.Staffs.Queries;
using GoFoodBeverage.Application.Mappings.Resolvers;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Models;
using GoFoodBeverage.Models.Address;
using GoFoodBeverage.Models.Area;
using GoFoodBeverage.Models.BarcodeConfig;
using GoFoodBeverage.Models.Bill;
using GoFoodBeverage.Models.Combo;
using GoFoodBeverage.Models.Customer;
using GoFoodBeverage.Models.DeliveryMethod;
using GoFoodBeverage.Models.FavoriteStore;
using GoFoodBeverage.Models.Fee;
using GoFoodBeverage.Models.Language;
using GoFoodBeverage.Models.Language.Dto;
using GoFoodBeverage.Models.Material;
using GoFoodBeverage.Models.OnlineStoreMenus;
using GoFoodBeverage.Models.Option;
using GoFoodBeverage.Models.Order;
using GoFoodBeverage.Models.Package;
using GoFoodBeverage.Models.Page;
using GoFoodBeverage.Models.Payment;
using GoFoodBeverage.Models.Permission;
using GoFoodBeverage.Models.Product;
using GoFoodBeverage.Models.Promotion;
using GoFoodBeverage.Models.PurchaseOrderModel;
using GoFoodBeverage.Models.QRCode;
using GoFoodBeverage.Models.Report;
using GoFoodBeverage.Models.Staff;
using GoFoodBeverage.Models.StampConfig;
using GoFoodBeverage.Models.Store;
using GoFoodBeverage.Models.Supplier;
using GoFoodBeverage.Models.Tax;
using GoFoodBeverage.Models.Unit;
using System.Linq;

namespace GoFoodBeverage.Application.Mappings
{
    public class GeneralProfile : Profile
    {
        public GeneralProfile()
        {
            /// CreateMap here
            CreateMap<BusinessArea, BusinessAreaModel>();
            CreateMap<Currency, CurrencyModel>();

            /// Permission
            CreateMap<GroupPermission, GroupPermissionModel>();
            CreateMap<PermissionGroup, PermissionGroupModel>();
            CreateMap<Permission, PermissionModel>();
            CreateMap<GroupPermission, GetGroupPermissionByIdModel>();
            CreateMap<GroupPermissionPermission, GetGroupPermissionByIdModel.GroupPermissionPermission>();

            /// Prepare Create New Staff Data
            CreateMap<GroupPermission, StaffGroupPermissionModel.GroupPermissionDto>();
            CreateMap<StoreBranch, StaffGroupPermissionModel.BranchDto>();
            CreateMap<Staff, StaffByIdModel>();

            /// Account
            CreateMap<Account, StaffModel.AccountDto>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Username));

            /// Store
            CreateMap<Store, StoreModel>();
            CreateMap<Store, StoresModel>();
            CreateMap<Store, StoreDetailModel>();
            CreateMap<StoreBankAccount, StoreBankAccountModel>();
            CreateMap<StoreBranch, StoresModel.BranchModel>();
            CreateMap<StoreBranch, StoreModel.BranchModel>();

            /// Staff
            CreateMap<Staff, StaffModel>();

            /// Branch Management
            CreateMap<StoreBranch, StoreBranchModel>();

            /// Address
            CreateMap<Address, AddressModel>();
            CreateMap<Country, CountryModel>();
            CreateMap<State, StateModel>();
            CreateMap<City, CityModel>();
            CreateMap<District, DistrictModel>();
            CreateMap<Ward, WardModel>();

            /// Material Management
            CreateMap<Material, MaterialModel>()
                .ForMember(dest => dest.UnitName, opt => opt.MapFrom(src => src.Unit.Name));
            CreateMap<Material, MaterialPrepareDataModel>()
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom<MaterialQuantityResolver>())
                .ForMember(dest => dest.UnitName, opt => opt.MapFrom(src => src.Unit.Name));

            CreateMap<Material, ExportMaterialModel>()
                .ForMember(dest => dest.UnitName, opt => opt.MapFrom(src => src.Unit.Name))
                .ForMember(dest => dest.MaterialCategoryName, opt => opt.MapFrom(src => src.MaterialCategory.Name));
            CreateMap<MaterialInventoryBranch, ExportMaterialModel.MaterialInventoryBranchDto>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.BranchId))
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Branch.Name));

            CreateMap<MaterialCategory, MaterialCategoryDatatableModel>()
                .ForMember(dst => dst.TotalMaterial, opt => opt.MapFrom(src => src.Materials.Count));

            CreateMap<MaterialCategory, MaterialCategoryModel>();
            CreateMap<Material, MaterialByIdModel>();
            CreateMap<MaterialCategory, MaterialByIdModel.MaterialCategoryDto>();
            CreateMap<Unit, MaterialByIdModel.UnitDto>();
            CreateMap<MaterialInventoryBranch, MaterialByIdModel.MaterialInventoryBranchDto>();
            CreateMap<StoreBranch, MaterialByIdModel.MaterialInventoryBranchDto.StoreBranchDto>();
            CreateMap<MaterialCategory, MaterialCategoryByIdModel>();
            CreateMap<Material, MaterialCategoryByIdModel.MaterialDto>()
                .ForMember(dst => dst.UnitName, opt => opt.MapFrom(src => src.Unit.Name));

            CreateMap<UnitConversion, UnitConversionUnitDto>();
            CreateMap<Unit, UnitConversionUnitDto.UnitModel>();

            /// Product
            CreateMap<ProductPrice, ProductPriceModel>();

            CreateMap<Channel, ChannelModel>();
            CreateMap<Platform, PlatformModel>();
            CreateMap<ProductCategory, ProductCategoryModel>();
            CreateMap<ProductCategory, ProductCategoryByIdModel>();
            CreateMap<Product, ProductModel>();
            CreateMap<ProductProductCategory, ProductModel>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dst => dst.ProductCategoryId, opt => opt.MapFrom(src => src.ProductCategoryId))
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dst => dst.Unit, opt => opt.MapFrom(src => src.Product.Unit))
                .ForMember(dst => dst.ProductPrices, opt => opt.MapFrom(src => src.Product.ProductPrices));
            CreateMap<Product, ProductDatatableModel>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom<ProductStatusResolver>());
            CreateMap<Product, ProductEditResponseModel>();
            CreateMap<ProductProductCategory, ProductProductCategoryModel>()
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dst => dst.Thumbnail, opt => opt.MapFrom(src => src.Product.Thumbnail))
                .ForMember(dst => dst.UnitName, opt => opt.MapFrom(src => src.Product.Unit.Name));
            CreateMap<Product, ProductProductCategoryModel>()
                .ForMember(dst => dst.ProductId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.UnitName, opt => opt.MapFrom(src => src.Unit.Name));

            CreateMap<Product, ProductToppingModel>();

            /// ProductCategory
            CreateMap<ProductCategory, ProductCategoryActivatedModel>();
            CreateMap<ProductProductCategory, ProductCategoryActivatedModel.ProductProductCategoryModel>();
            CreateMap<Product, ProductCategoryActivatedModel.ProductProductCategoryModel.ProductModel>();
            CreateMap<ProductPrice, ProductCategoryActivatedModel.ProductProductCategoryModel.ProductModel.ProductPriceDto>();
            CreateMap<ProductOption, ProductCategoryActivatedModel.ProductProductCategoryModel.ProductModel.ProductOptionDto>();

            CreateMap<Product, ProductDetailAppOrderModel>();
            CreateMap<ProductPrice, ProductDetailAppOrderModel.ProductPriceDto>();
            CreateMap<ProductOption, ProductDetailAppOrderModel.ProductOptionDto>();
            //online store menu
            CreateMap<OnlineStoreMenu, OnlineStoreMenuModel>();
            CreateMap<OnlineStoreMenuItem, OnlineStoreMenuItemModel>();
            /// Option Management
            CreateMap<Option, OptionModel>();
            CreateMap<Option, OptionByIdModel>();
            CreateMap<OptionLevel, OptionLevelModel>();

            /// Unit Management
            CreateMap<Unit, UnitModel>();
            CreateMap<UnitConversion, UnitConversionDto>();
            CreateMap<UnitConversion, UnitConversionUnitDto>();
            CreateMap<Unit, UnitConversionUnitDto.UnitModel>();

            ///Language
            CreateMap<Language, LanguageModel>();
            CreateMap<LanguageStoreDto, LanguageStoreDtoModel>()
                            .ForMember(dest => dest.IsPublish, opt => opt.MapFrom<LanguageStatusResolver>());

            /// Supplier
            CreateMap<Supplier, SupplierModel>();
            CreateMap<SupplierModel, Supplier>();

            /// PurchaseOrder
            CreateMap<PurchaseOrder, GetPurchaseOrderByIdModel>();
            CreateMap<PurchaseOrder, GetPurchaseOrderByBranchModel>();
            CreateMap<StoreBranch, GetPurchaseOrderByIdModel.StoreBranchDto>();
            CreateMap<Store, GetPurchaseOrderByIdModel.StoreDto>();
            CreateMap<Supplier, GetPurchaseOrderByIdModel.SupplierDto>();
            CreateMap<PurchaseOrderMaterial, GetPurchaseOrderByIdModel.PurchaseOrderMaterialDto>();
            CreateMap<Material, GetPurchaseOrderByIdModel.PurchaseOrderMaterialDto.MaterialDto>();
            CreateMap<Unit, GetPurchaseOrderByIdModel.PurchaseOrderMaterialDto.MaterialDto.UnitDto>();
            CreateMap<Unit, GetPurchaseOrderByIdModel.PurchaseOrderMaterialDto.UnitDto>();
            CreateMap<PurchaseOrder, PurchaseOrderModel>()
                .ForMember(dst => dst.Status, opt => opt.MapFrom(src => new PurchaseOrderModel.StatusDto
                {
                    StatusId = src.StatusId,
                    Name = src.StatusId.GetStatusName(),
                    Color = src.StatusId.GetColor(),
                    BackGroundColor = src.StatusId.GetBackGroundColor()
                }));
            CreateMap<PurchaseOrderMaterial, PurchaseOrderModel.PurchaseOrderMaterialDto>();
            CreateMap<Supplier, PurchaseOrderModel.SupplierDto>();
            CreateMap<StoreBranch, PurchaseOrderModel.StoreBranchDto>();

            /// Table
            CreateMap<AreaTable, AreaTableModel>();
            CreateMap<AreaTable, AreaTableByIdModel>();

            //Area
            CreateMap<Area, AreaModel>();
            CreateMap<Area, AreaByIdModel>();

            //AreaTable
            CreateMap<Area, AreaTablesByBranchIdModel>();
            CreateMap<AreaTable, AreaTablesByBranchIdModel.AreaTableDto>();
            CreateMap<AreaTable, AreaTableByIdModel>();
            CreateMap<AreaTable, AreaTableByIdModel.AreaTableDto>();

            //Promotion
            CreateMap<Promotion, GetPromotionsInStoreModel>();
            CreateMap<Promotion, GetPromotionByIdModel>()
                .ForMember(dst => dst.Products, opt => opt.MapFrom(src => src.PromotionProducts))
                .ForMember(dst => dst.ProductCategories, opt => opt.MapFrom(src => src.PromotionProductCategories))
                .ForMember(dst => dst.Branches, opt => opt.MapFrom(src => src.PromotionBranches));
            CreateMap<PromotionProduct, GetPromotionByIdModel.ProductDto>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Product.Name));
            CreateMap<PromotionProductCategory, GetPromotionByIdModel.ProductCategoryDto>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.ProductCategoryId))
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.ProductCategory.Name));
            CreateMap<PromotionBranch, GetPromotionByIdModel.StoreBranchDto>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.BranchId))
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Branch.Name));

            CreateMap<Promotion, PromotionDetailModel>()
                .ForMember(dst => dst.Products, opt => opt.MapFrom(src => src.PromotionProducts))
                .ForMember(dst => dst.ProductCategories, opt => opt.MapFrom(src => src.PromotionProductCategories))
                .ForMember(dst => dst.Branches, opt => opt.MapFrom(src => src.PromotionBranches));
            CreateMap<PromotionProduct, PromotionDetailModel.ProductDto>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Product.Name));
            CreateMap<PromotionProductCategory, PromotionDetailModel.ProductCategoryDto>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.ProductCategoryId))
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.ProductCategory.Name));
            CreateMap<PromotionBranch, PromotionDetailModel.StoreBranchDto>()
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.BranchId))
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Branch.Name));

            //Combo
            CreateMap<Combo, ComboDataTableModel>();
            CreateMap<ComboStoreBranch, ComboDataTableModel.ComboStoreBranchModel>();
            CreateMap<StoreBranch, ComboDataTableModel.ComboStoreBranchModel.StoreBranch>();
            CreateMap<ComboProductGroup, ComboDataTableModel.ComboProductGroupModel>();
            CreateMap<ProductCategory, ComboDataTableModel.ComboProductGroupModel.ProductCategoryModel>();
            CreateMap<ComboProductGroupProductPrice, ComboDataTableModel.ComboProductGroupModel.ComboProductGroupProductPriceModel>();
            CreateMap<ProductPrice, ComboDataTableModel.ComboProductGroupModel.ComboProductGroupProductPriceModel.ProductPriceModel>();
            CreateMap<Product, ComboDataTableModel.ComboProductGroupModel.ComboProductGroupProductPriceModel.ProductPriceModel.ProductModel>();
            CreateMap<StoreBranch, ComboDataTableModel.StoreBranch>();
            CreateMap<ComboProductPrice, ComboDataTableModel.ComboProductPriceModel>();
            CreateMap<ProductPrice, ComboDataTableModel.ComboProductPriceModel.ProductPriceModel>();
            CreateMap<Product, ComboDataTableModel.ComboProductPriceModel.ProductPriceModel.ProductModel>();

            CreateMap<Combo, ComboActivatedModel>();
            CreateMap<ComboProductPrice, ComboActivatedModel.ComboProductPriceModel>();
            CreateMap<ComboProductPrice, ComboActivatedModel.ComboProductPriceModel>()
                .ForMember(dest => dest.PriceName, opt => opt.MapFrom(src => src.ProductPrice.PriceName));
            CreateMap<ProductPrice, ComboActivatedModel.ComboProductPriceModel.ProductPriceModel>();
            CreateMap<Product, ComboActivatedModel.ProductModel>();

            CreateMap<ComboPricing, ComboActivatedModel.ComboPricingModel>();
            CreateMap<ComboPricingProductPrice, ComboActivatedModel.ComboPricingModel.ComboPricingProductPriceModel>();
            CreateMap<ProductPrice, ComboActivatedModel.ComboPricingModel.ComboPricingProductPriceModel.ProductPriceModel>();
            CreateMap<Product, ComboActivatedModel.ProductModel>();
            CreateMap<ProductOption, ComboActivatedModel.ProductModel.ProductOptionDto>();

            CreateMap<ComboPricing, ComboDataTableModel.ComboPricingModel>();
            CreateMap<ComboPricingProductPrice, ComboDataTableModel.ComboPricingModel.ComboPricingProductPriceModel>();
            CreateMap<ProductPrice, ComboDataTableModel.ComboPricingModel.ComboPricingProductPriceModel.ProductPriceModel>();
            CreateMap<Product, ComboDataTableModel.ComboPricingModel.ComboPricingProductPriceModel.ProductPriceModel.ProductModel>();

            //Customer
            CreateMap<Account, CustomersModel>()
                .ForMember(a => a.Email, b => b.MapFrom(src => src.Username))
                .ForMember(item => item.PhoneCode, account => account.MapFrom(ac => ac.Country.Phonecode));
            CreateMap<Customer, CustomerEditModel>()
                .ForMember(customerEditModel => customerEditModel.PlatformName, customer => customer.MapFrom(item => item.Platform.Name));
            CreateMap<QuickCreateCustomerRequest, Account>();
            CreateMap<Domain.Entities.AccountAddress, CustomerAddressModel>();
            CreateMap<CustomerMembershipLevel, CustomerMembershipModel>();
            CreateMap<Customer, CustomersModel>();
            CreateMap<Customer, TopCustomerReportModel>();

            //DeliveryMethod
            CreateMap<DeliveryMethod, DeliveryMethodModel>();
            CreateMap<DeliveryConfig, DeliveryMethodModel.DeliveryConfigDto>();
            CreateMap<DeliveryConfigPricing, DeliveryMethodModel.DeliveryConfigDto.DeliveryConfigPricingDto>();
            CreateMap<Customer, CustomerDataBySegmentModel>();
            CreateMap<CustomerSegment, CustomerSegmentModel>();
            CreateMap<CustomerSegment, CustomerSegmentByIdModel>();
            CreateMap<CustomerSegmentCondition, CustomerSegmentConditionDataModel>();

            CreateMap<DeliveryMethod, DeliveryMethodByStoreIdModel>();
            CreateMap<DeliveryConfig, DeliveryMethodByStoreIdModel.DeliveryConfigDto>();
            CreateMap<DeliveryConfigPricing, DeliveryMethodByStoreIdModel.DeliveryConfigDto.DeliveryConfigPricingDto>();

            /// Payment
            CreateMap<PaymentMethod, PaymentMethodModel>()
                .ForMember(dest => dest.Icon, opt => opt.MapFrom<PaymentIconResolver>());

            CreateMap<PaymentMethod, PaymentMethodByStoreModel>()
                .ForMember(dest => dest.Icon, opt => opt.MapFrom<PaymentIconResolver>());
            CreateMap<PaymentConfig, PaymentMethodByStoreModel.PaymentConfigDto>();

            CreateMap<PaymentConfig, PaymentConfigModel>();

            //Fee
            CreateMap<Fee, FeeModel>();
            CreateMap<Fee, FeeDetailModel>();
            CreateMap<FeeBranch, FeeDetailModel.StoreBranchDto>()
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Branch.Name))
                .ForMember(dst => dst.Code, opt => opt.MapFrom(src => src.Branch.Code));

            //Tax
            CreateMap<Tax, TaxTypeModel>();
            CreateMap<Tax, TaxModel>();

            //Order
            CreateMap<Order, OrderModel>();
            CreateMap<Order, BaseOrderModel>()
                .ForMember(dst => dst.TotalPrices, opt => opt.MapFrom(src => src.OriginalPrice - src.TotalDiscountAmount + src.TotalFee + src.DeliveryFee))
                .ForMember(dst =>
                    dst.TotalItems,
                    opt => opt.
                        MapFrom(src =>
                            src.OrderItems == null ? 0 : src.OrderItems.Sum(oi => oi.Quantity)
                            )
                        );

            CreateMap<Customer, OrderModel.CustomerDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.AccumulatedPoint, opt => opt.MapFrom(src => src.CustomerPoint.AccumulatedPoint));
            CreateMap<Order, OrderDetailModel>();

            // Order, Order item----
            CreateMap<Order, OrderDetailDataById>();
            CreateMap<Customer, OrderDetailDataById.CustomerDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.AccumulatedPoint, opt => opt.MapFrom(src => src.CustomerPoint.AccumulatedPoint))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));

            CreateMap<OrderDelivery, OrderDetailDataById.OrderDeliveryDto>();
            CreateMap<OrderFee, OrderDetailDataById.OrderFeeDto>();

            CreateMap<OrderItem, OrderDetailDataById.OrderItemDto>();
            CreateMap<ProductPrice, OrderDetailDataById.OrderItemDto.ProductPriceDto>();
            CreateMap<Product, OrderDetailDataById.OrderItemDto.ProductPriceDto.ProductDto>();
            CreateMap<OrderItemOption, OrderItemOptionModel>();
            CreateMap<OrderItemTopping, OrderItemToppingModel>();
            CreateMap<OrderComboItem, OrderDetailDataById.OrderItemDto.OrderComboItemDto>();
            CreateMap<OrderComboProductPriceItem, OrderDetailDataById.OrderItemDto.OrderComboItemDto.OrderComboProductPriceItemDto>();
            //----

            CreateMap<Customer, OrderDetailModel.CustomerDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.AccumulatedPoint, opt => opt.MapFrom(src => src.CustomerPoint.AccumulatedPoint))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));

            #region GoFood App
            //OrderDetail (App goFood)
            CreateMap<Order, OrderDetailByIdModel>();
            CreateMap<OrderDelivery, OrderDetailByIdModel.OrderDeliveryDto>();

            //StoreBranch (App goFood)
            CreateMap<StoreBranch, FavoriteStoreModel.BranchModel>();

            #endregion

            //Order Item
            CreateMap<OrderItem, OrderItemModel>();
            CreateMap<ProductPrice, OrderItemModel.ProductPriceDto>();
            CreateMap<Product, OrderItemModel.ProductPriceDto.ProductDto>();
            CreateMap<OrderItemOption, OrderItemOptionModel>();
            CreateMap<OrderItemTopping, OrderItemToppingModel>();
            CreateMap<OrderComboItem, OrderComboItemModel>();
            CreateMap<OrderComboProductPriceItem, OrderComboProductPriceItemModel>();
            CreateMap<OrderItemOption, OrderComboItemOptionModel>();
            CreateMap<OrderItemTopping, OrderComboItemToppingModel>();

            /// StampConfig
            CreateMap<StampConfig, StampConfigModel>();

            // Bill configuration
            CreateMap<BillConfiguration, BillModel>();

            /// BarcodeConfig
            CreateMap<BarcodeConfig, BarcodeConfigModel>();

            CreateMap<FunctionGroup, FunctionGroupModel>();
            CreateMap<Function, FunctionModel>();

            CreateMap<OrderPackage, PackageOrderModel>()
                .ForMember(dest => dest.EnumPackageStatusId, opt => opt.MapFrom<PackageStatusResolver>());
            CreateMap<OrderPackage, OrderPackageInternalToolDatatableModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.Paid, opt => opt.MapFrom(src => src.PackageOderPaymentStatus == EnumOrderPaymentStatus.Paid))
                .ForMember(dest => dest.StoreId, opt => opt.MapFrom(src => src.StoreCode))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.AccountCode))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PackagePaymentMethod.GetName()))
                .ForMember(dest => dest.BoughtPackage, opt => opt.MapFrom(src => src.PackageId))
                .ForMember(dest => dest.NumberMonth, opt => opt.MapFrom(src => src.PackageDurationByMonth))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedTime.Value))
                .ForMember(dest => dest.LastModifiedDate, opt => opt.MapFrom(src => src.LastSavedTime.Value))
                .ForMember(dest => dest.CreatedUserId, opt => opt.MapFrom(src => src.CreatedUser));

            CreateMap<Package, PackageOrderModel.PackageDto>();

            CreateMap<AccountTransfer, AccountTransferModel>();

            // Store
            CreateMap<Store, BaseOrderModel.StoreModel>();

            /// Revenue
            CreateMap<Order, OrderRevenueModel>();

            CreateMap<StaffActivity, GetStaffActivityResponse>();

            CreateMap<Order, OrderDeliveryHistoryModel>();

            #region QR code
            CreateMap<QRCode, QRCodeModel>();
            CreateMap<QRCode, QRCodeDetailDto>()
                .ForMember(dest => dest.BranchId, opt => opt.MapFrom(src => src.StoreBranchId))
                .ForMember(dest => dest.IsPercentage, opt => opt.MapFrom(src => src.IsPercentDiscount))
                .ForMember(dest => dest.QrCodeId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.QRCodeProducts))
            ;

            CreateMap<QRCodeProduct, QRCodeDetailDto.QRCodeProductDto>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Product.Id))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductThumbnail, opt => opt.MapFrom(src => src.Product.Thumbnail))
                .ForMember(dest => dest.UnitId, opt => opt.MapFrom(src => src.Product.UnitId))
                .ForMember(dest => dest.UnitName, opt => opt.MapFrom(src => src.Product.Unit.Name))
            ;
            #endregion

            CreateMap<Theme, ThemeModel>()
                 .ForMember(dest => dest.Thumbnail, opt => opt.MapFrom<ThemeImageResolver>());

            CreateMap<EmailCampaign, EmailCampaignModel>();

            // Customer report pie chart
            CreateMap<Customer, CustomerReportByPlatformModel>();
            CreateMap<Order, OrderReportForCustomerModel>();
            CreateMap<Platform, PlatFormReportForCustomerModel>();

            // Get customer segment by store id
            CreateMap<CustomerSegment, GetCustomerSegmentInCurrentStoreResponse>()
                .ForMember(dest => dest.TotalCustomer, opt => opt.MapFrom<GetTotalCustomerOfCustomerSegmentResolver>())
                .ForMember(dest => dest.TotalEmail, opt => opt.MapFrom<GetTotalEmailOfCustomerSegmentResolver>());

            CreateMap<CreateEmailCampaignRequest, EmailCampaign>()
                .ForMember(dest => dest.EmailCampaignSocials, opt => opt.MapFrom<EmailCampaignSocialResolver>())
                .ForMember(dest => dest.EmailCampaignDetails, opt => opt.MapFrom<EmailCampaignDetailResolver>())
                .ForMember(dest => dest.EmailCampaignCustomerSegments, opt => opt.MapFrom<EmailCampaignCustomerSegmentResolver>());

            // Online store
            CreateMap<Page, PageModel>();
            CreateMap<OnlineStoreMenu, SubMenuDetailModel>();
            CreateMap<OnlineStoreMenuItem, SubMenuDetailModel.MenuItemModel>();
        }
    }
}
