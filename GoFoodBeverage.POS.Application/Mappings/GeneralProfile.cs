using AutoMapper;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.POS.Models.Stamp;
using GoFoodBeverage.POS.Models.Store;
using GoFoodBeverage.POS.Models.Product;
using GoFoodBeverage.POS.Models.Fee;
using GoFoodBeverage.POS.Models.Area;
using GoFoodBeverage.POS.Models.Order;
using GoFoodBeverage.POS.Models.Address;
using GoFoodBeverage.POS.Models.Customer;
using GoFoodBeverage.POS.Models.Combo;
using GoFoodBeverage.POS.Models.Permission;
using GoFoodBeverage.POS.Models.Material;
using GoFoodBeverage.Common.Models.Clone.Order;
using GoFoodBeverage.Common.Models.Clone;
using GoFoodBeverage.POS.Models.Bill;
using GoFoodBeverage.POS.Models.DeliveryMethod;
using GoFoodBeverage.POS.Application.Mappings.Resolvers;
using GoFoodBeverage.Domain.Enums;
using System.Linq;

namespace GoFoodBeverage.POS.Application.Mappings
{
    public class GeneralProfile : Profile
    {
        public GeneralProfile()
        {
            #region CreateMap here

            CreateMap<ProductCategory, ProductCategoryActivatedModel>();
            CreateMap<Product, ProductActivatedModel>();
            CreateMap<Unit, UnitModel>();
            CreateMap<ProductPrice, ProductPriceModel>();

            /// Store
            CreateMap<Store, StoreByAccountIdModel>();
            CreateMap<StoreBankAccount, StoreBankAccountModel>();

            /// Branch
            CreateMap<StoreBranch, StoreBranchModel>();

            /// Fee
            CreateMap<Fee, FeeModel>();

            /// AreaTable
            CreateMap<Area, AreaTablesByBranchIdModel>();
            CreateMap<AreaTable, AreaTablesByBranchIdModel.AreaTableDto>();
            CreateMap<AreaTable, AreaTableModel>();

            ///PosOrder
            CreateMap<Order, PosOrderModel>()
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems.Where(oi => oi.StatusId != EnumOrderItemStatus.Canceled)))
                .ForMember(dest => dest.ReceiverAddress, opt => opt.MapFrom(src => src.OrderDelivery.ReceiverAddress))
                .ForMember(dest => dest.AreaTableName, opt => opt.MapFrom(src => src.AreaTable.Name))
                .ForMember(dest => dest.AreaName, opt => opt.MapFrom(src => src.AreaTable.Area.Name));
            CreateMap<OrderItem, PosOrderModel.OrderItemDto>();
            CreateMap<Order, AreaTablesByBranchIdModel.AreaTableDto.OrderDto>();

            CreateMap<Order, DetailOrderToPrintModel>()
                .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.Store.Title))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Store.Address.Address1))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Store.Address.Country.Name))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.Store.Address.State.Name))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Store.Address.City.Name))
                .ForMember(dest => dest.District, opt => opt.MapFrom(src => src.Store.Address.District.Name))
                .ForMember(dest => dest.Ward, opt => opt.MapFrom(src => src.Store.Address.Ward.Name))
                .ForMember(dest => dest.OrderCode, opt => opt.MapFrom(src => src.StringCode))
                .ForMember(dest => dest.OrderTime, opt => opt.MapFrom(src => src.CreatedTime))
                .ForMember(dest => dest.CashierName, opt => opt.MapFrom(src => src.CashierName))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.FullName))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.OriginalPrice))
                .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.TotalDiscountAmount + src.CustomerDiscountAmount))
                .ForMember(dest => dest.FeeAndTax, opt => opt.MapFrom(src => src.TotalFee + src.TotalTax))
                .ForMember(dest => dest.ReceivedAmount, opt => opt.MapFrom(src => src.ReceivedAmount))
                .ForMember(dest => dest.Change, opt => opt.MapFrom(src => src.Change));

            CreateMap<OrderItem, DetailOrderToPrintModel.ProductListModel>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName))
                .ForMember(dest => dest.PriceName, opt => opt.MapFrom(src => src.ProductPriceName))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.PriceAfterDiscount));

            CreateMap<OrderItemTopping, DetailOrderToPrintModel.ProductListModel.ToppingListModel>()
                .ForMember(dest => dest.ToppingName, opt => opt.MapFrom(src => src.ToppingName))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.PriceAfterDiscount));

            CreateMap<OrderItemOption, DetailOrderToPrintModel.ProductListModel.OptionListModel>()
                .ForMember(dest => dest.OptionName, opt => opt.MapFrom(src => src.OptionName))
                .ForMember(dest => dest.OptionValue, opt => opt.MapFrom(src => src.OptionLevelName));
            CreateMap<OrderComboItem, DetailOrderToPrintModel.ProductListModel.OrderComboItemDto>();
            CreateMap<OrderComboProductPriceItem, DetailOrderToPrintModel.ProductListModel.OrderComboItemDto.ComboItemDto>();
            CreateMap<OrderItemOption, DetailOrderToPrintModel.ProductListModel.OrderComboItemDto.ProductOptionModel>();
            CreateMap<OrderItemTopping, DetailOrderToPrintModel.ProductListModel.OrderComboItemDto.ProductToppingDto>();

            // mapping order item to cart item
            CreateMap<OrderItem, ProductCartItemModel>()
                .ForMember(dest => dest.OrderItemId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ComboName, opt => opt.MapFrom(src => src.OrderComboItem != null ? src.OrderComboItem.ComboName : string.Empty))
                .ForMember(dest => dest.ComboId, opt => opt.MapFrom(src => src.OrderComboItem != null ? src.OrderComboItem.ComboId : null));

            CreateMap<Promotion, ProductCartItemModel.PromotionDto>();

            CreateMap<OrderItemTopping, ProductCartItemModel.ToppingDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ToppingName))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.PriceAfterDiscount))
                .ForMember(dest => dest.PriceAfterDiscount, opt => opt.MapFrom(src => src.PriceAfterDiscount));
            CreateMap<OrderComboItem, ComboOrderItemDto>()
                .ForMember(dest => dest.ComboItems, opt => opt.Ignore());

            /// Address
            CreateMap<Address, AddressModel>();
            CreateMap<Country, CountryModel>();
            CreateMap<State, StateModel>();
            CreateMap<City, CityModel>();
            CreateMap<District, DistrictModel>();
            CreateMap<Ward, WardModel>();

            /// Customer 
            CreateMap<Customer, CustomerEditModel>();
            CreateMap<Customer, CustomerModel>();

            ///Combo
            CreateMap<Combo, PosComboActivatedModel>();
            CreateMap<Combo, ComboDataTableModel>();
            CreateMap<ComboStoreBranch, ComboDataTableModel.ComboStoreBranchModel>();
            CreateMap<StoreBranch, ComboDataTableModel.ComboStoreBranchModel.StoreBranch>();
            CreateMap<ComboProductGroup, ComboDataTableModel.ComboProductGroupModel>();
            CreateMap<ComboProductGroup, ComboDataTableModel.ComboProductGroupModel>()
                .ForMember(dest => dest.ProductCategoryName, opt => opt.MapFrom(src => src.ProductCategory.Name));
            CreateMap<ComboProductGroupProductPrice, ComboDataTableModel.ComboProductGroupModel.ComboProductGroupProductPriceModel>();
            CreateMap<ProductPrice, ComboDataTableModel.ComboProductGroupModel.ComboProductGroupProductPriceModel.ProductPriceModel>();
            CreateMap<Product, ComboDataTableModel.ComboProductGroupModel.ComboProductGroupProductPriceModel.ProductPriceModel.ProductModel>()
                .ForMember(dest => dest.ProductOptions, opt => opt.MapFrom<ProductOptionsResolver>());
            CreateMap<StoreBranch, ComboDataTableModel.StoreBranch>();
            CreateMap<ComboProductPrice, ComboDataTableModel.ComboProductPriceModel>();
            CreateMap<ComboProductPrice, ComboDataTableModel.ComboProductPriceModel>()
                .ForMember(dest => dest.PriceName, opt => opt.MapFrom(src => src.ProductPrice.PriceName));
            CreateMap<ProductPrice, ComboDataTableModel.ComboProductPriceModel.ProductPriceModel>();
            CreateMap<Product, ComboDataTableModel.ProductModel>()
                .ForMember(dest => dest.ProductOptions, opt => opt.MapFrom<ProductOptionsResolver>());

            CreateMap<ComboPricing, ComboDataTableModel.ComboPricingModel>();
            CreateMap<ComboPricingProductPrice, ComboDataTableModel.ComboPricingModel.ComboPricingProductPriceModel>();
            CreateMap<ProductPrice, ComboDataTableModel.ComboPricingModel.ComboPricingProductPriceModel.ProductPriceModel>();
            CreateMap<Product, ComboDataTableModel.ProductModel>()
                .ForMember(dest => dest.ProductOptions, opt => opt.MapFrom<ProductOptionsResolver>());

            //POS combo detail
            CreateMap<Combo, ComboDetailPosOrderDto>();
            CreateMap<ComboProductGroup, ComboDetailPosOrderDto.ComboProductGroupDto>();
            CreateMap<ComboProductGroup, ComboDetailPosOrderDto.ComboProductGroupDto>()
                .ForMember(dest => dest.ProductCategoryName, opt => opt.MapFrom(src => src.ProductCategory.Name));
            CreateMap<ComboProductGroupProductPrice, ComboDetailPosOrderDto.ComboProductGroupProductPriceDto>();
            CreateMap<ProductPrice, ComboDetailPosOrderDto.ProductPriceDto>();
            CreateMap<Product, ComboDetailPosOrderDto.ProductDto>();
            CreateMap<ComboProductPrice, ComboDetailPosOrderDto.ComboProductPriceDto>();
            CreateMap<ComboProductPrice, ComboDetailPosOrderDto.ComboProductPriceDto>()
                .ForMember(dest => dest.PriceName, opt => opt.MapFrom(src => src.ProductPrice.PriceName));
            CreateMap<ProductPrice, ComboDetailPosOrderDto.ProductPriceDto>();
            CreateMap<Product, ComboDetailPosOrderDto.ProductDto>();
            CreateMap<ComboPricing, ComboDetailPosOrderDto.ComboPricingDto>();
            CreateMap<ComboPricingProductPrice, ComboDetailPosOrderDto.ComboPricingProductPriceDto>();
            CreateMap<ProductPrice, ComboDetailPosOrderDto.ProductPriceDto>();
            CreateMap<Product, ComboDetailPosOrderDto.ProductDto>()
                .ForMember(dest => dest.ProductOptions, opt => opt.MapFrom<ProductOptionsResolver>());

            /// Permission
            CreateMap<Permission, PermissionModel>();

            /// Material Management
            CreateMap<Material, MaterialModel>()
                .ForMember(dest => dest.UnitName, opt => opt.MapFrom(src => src.Unit.Name));
            CreateMap<Material, MaterialsFromOrdersCurrentShiftModel>();

            /// Clone order detail
            CreateMap<OrderItem, OrderItemCloneModel>();
            CreateMap<ProductPrice, ProductPriceCloneModel>();
            CreateMap<Promotion, PromotionCloneModel>();
            CreateMap<OrderItemOption, OrderItemOptionCloneModel>();
            CreateMap<OrderItemTopping, OrderItemToppingCloneModel>();
            CreateMap<OrderFee, OrderFeeCloneModel>();
            CreateMap<Promotion, PosPromotionModel>();
            CreateMap<OrderItemOption, PosOrderItemOptionModel>();
            CreateMap<OrderItemTopping, PosOrderItemToppingModel>();
            CreateMap<ProductPrice, PosProductPriceModel>();

            /// Stamp
            CreateMap<StampConfig, StampConfigModel>();

            // Bill configuaration
            CreateMap<BillConfiguration, BillModel>();

            //DeliveryMethod
            CreateMap<DeliveryMethod, DeliveryMethodModel>();
            CreateMap<DeliveryConfig, DeliveryConfigModel>();
            CreateMap<DeliveryConfigPricing, DeliveryConfigModel.DeliveryConfigPricingDto>();

            CreateMap<Order, GoFoodBeverage.Models.Order.OrderDeliveryHistoryModel>();

            // OrderItem
            CreateMap<OrderItem, CheckOrderItemModel>();
            CreateMap<OrderComboItem, CheckOrderItemModel.OrderComboItemModel>();
            CreateMap<OrderComboProductPriceItem, CheckOrderItemModel.OrderComboItemModel.OrderComboProductPriceItemModel>();

            #endregion
        }
    }
}
