using AutoMapper;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Common.Constants;
using GoFoodBeverage.POS.Models.Stamp;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.Application.Features.Orders.Queries
{
    public class GetOrderStampDataRequest : IRequest<GetOrderStampDataResponse>
    {
        public Guid? OrderId { get; set; }

        public Guid? OrderItemId { get; set; }

        public string PrintStampType { get; set; }
    }

    public class GetOrderStampDataResponse
    {
        public StampConfigModel StampConfig { get; set; }

        public PrintStampDataModel StampData { get; set; }
    }

    public class GetOrderStampDataRequestHandler : IRequestHandler<GetOrderStampDataRequest, GetOrderStampDataResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetOrderStampDataRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            MapperConfiguration mapperConfiguration
            )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<GetOrderStampDataResponse> Handle(GetOrderStampDataRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            var stampConfig = await _unitOfWork.StampConfigs
                .Find(s => s.StoreId == loggedUser.StoreId)
                .AsNoTracking()
                .ProjectTo<StampConfigModel>(_mapperConfiguration)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if(stampConfig == null)
            {
                var defaultConfig = await _unitOfWork.StampConfigs.CreateDefaultStamConfigAsync(loggedUser.StoreId);
                stampConfig = new StampConfigModel()
                {
                    StampType = defaultConfig.StampType,
                    IsShowLogo = defaultConfig.IsShowLogo,
                    IsShowTime = defaultConfig.IsShowTime,
                    IsShowNumberOfItem = defaultConfig.IsShowNumberOfItem,
                    IsShowNote = defaultConfig.IsShowNote,
                };
            }

            var orderInfo = await _unitOfWork.Orders
                .Find(o => o.StoreId == loggedUser.StoreId && o.Id == request.OrderId)
                /// Include combo order item
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.OrderComboItem)
                .ThenInclude(oci => oci.OrderComboProductPriceItems)
                .ThenInclude(ocppi => ocppi.OrderItemOptions)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.OrderComboItem)
                .ThenInclude(oci => oci.OrderComboProductPriceItems)
                .ThenInclude(ocppi => ocppi.OrderItemToppings)
                /// Include order item
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.OrderItemOptions)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.OrderItemToppings)
                /// Without tracking
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            ThrowError.BadRequestAgainstNull(orderInfo, "Cannot found order information, please check again!");

            var stampData = new PrintStampDataModel()
            {
                Code = $"#{orderInfo.StringCode}",
                Logo = "", ///TODO: Get store's logo url
                CreatedTime = orderInfo.CreatedTime.Value,
                ItemList = new List<PrintStampDataModel.StampOrderItemDto>()
            };

            if (stampConfig.IsShowLogo)
            {
                var logo = await _unitOfWork.Stores.Find(x => x.Id == loggedUser.StoreId.Value).Select(x => x.Logo).FirstOrDefaultAsync();
                stampData.Logo = logo;
            }

            switch (request.PrintStampType)
            {
                case PrintStampTypeConstants.ALL_ORDER_ITEMS:
                    var orderItems = orderInfo.OrderItems.ToList();
                    var optionLevelIds = new List<Guid?>();
                    foreach (var item in orderItems)
                    {
                        if (item.IsCombo)
                        {
                            foreach (var productPriceItem in item.OrderComboItem.OrderComboProductPriceItems)
                            {
                                optionLevelIds.AddRange(productPriceItem.OrderItemOptions.Select(oi => oi.OptionLevelId));
                            }
                        }
                        else
                        {
                            optionLevelIds.AddRange(item.OrderItemOptions.Select(oi => oi.OptionLevelId));
                        }
                    }

                    var optionLevels = await _unitOfWork.OptionLevels
                        .Find(ol => ol.StoreId == loggedUser.StoreId && optionLevelIds.Any(olid => olid == ol.Id))
                        .AsNoTracking()
                        .ToListAsync(cancellationToken: cancellationToken);

                    var index = 0;
                    orderItems.ForEach((oi) =>
                    {
                        if (oi.IsCombo)
                        {
                            var listItemCombo = oi.OrderComboItem.OrderComboProductPriceItems.ToList();
                            foreach (var item in listItemCombo)
                            {
                                index++;
                                var stampOrderItemData = MappingOrderItemToStampOrderItemForCombo(item, index, optionLevels);
                                stampData.ItemList.Add(stampOrderItemData);
                            }
                        }
                        else
                        {
                            index++;
                            var stampOrderItemData = MappingOrderItemToStampOrderItem(oi, index, optionLevels);
                            stampData.ItemList.Add(stampOrderItemData);
                        }
                    });
                    break;

                case PrintStampTypeConstants.ORDER_ITEM:
                default:
                    var orderItem = orderInfo.OrderItems.FirstOrDefault(oi => oi.Id == request.OrderItemId);
                    var stampOrderItemData = MappingOrderItemToStampOrderItem(orderItem, 1);
                    stampData.ItemList.Add(stampOrderItemData);
                    break;
            }

            var response = new GetOrderStampDataResponse()
            {
                StampConfig = stampConfig,
                StampData = stampData
            };

            return response;
        }

        private static PrintStampDataModel.StampOrderItemDto MappingOrderItemToStampOrderItem(OrderItem orderItem, int index, List<OptionLevel> optionLevels = null)
        {
            var stampOrderItem = new PrintStampDataModel.StampOrderItemDto()
            {
                No = index,
                Name = orderItem.ItemName,
                Current = index == 1,
                Options = new List<PrintStampDataModel.OrderItemOptionDetailDto>()
            };

            foreach (var option in orderItem.OrderItemOptions)
            {
                if (optionLevels != null && optionLevels.FirstOrDefault(item => (item.Id == option.OptionLevelId)).IsSetDefault.Value) continue;

                var orderItemOptionDetail = new PrintStampDataModel.OrderItemOptionDetailDto()
                {
                    Name = $"{option.OptionName}",
                    Value = option.OptionLevelName
                };

                stampOrderItem.Options.Add(orderItemOptionDetail);
            }

            foreach (var topping in orderItem.OrderItemToppings)
            {
                var orderItemOptionDetail = new PrintStampDataModel.OrderItemOptionDetailDto()
                {
                    Name = $"{topping.ToppingName}",
                    Value = $"x{topping.Quantity}"
                };

                stampOrderItem.Options.Add(orderItemOptionDetail);
            }

            return stampOrderItem;
        }

        private static PrintStampDataModel.StampOrderItemDto MappingOrderItemToStampOrderItemForCombo(OrderComboProductPriceItem orderItem, int index, List<OptionLevel> optionLevels)
        {
            var stampOrderItem = new PrintStampDataModel.StampOrderItemDto()
            {
                No = index,
                Name = orderItem.ItemName,
                Current = index == 1,
                Options = new List<PrintStampDataModel.OrderItemOptionDetailDto>()
            };

            foreach (var option in orderItem.OrderItemOptions)
            {
                if (optionLevels != null && optionLevels.FirstOrDefault(item => (item.Id == option.OptionLevelId)).IsSetDefault.Value) continue;

                var orderItemOptionDetail = new PrintStampDataModel.OrderItemOptionDetailDto()
                {
                    Name = $"{option.OptionName}",
                    Value = option.OptionLevelName
                };

                stampOrderItem.Options.Add(orderItemOptionDetail);
            }

            foreach (var topping in orderItem.OrderItemToppings)
            {
                var orderItemOptionDetail = new PrintStampDataModel.OrderItemOptionDetailDto()
                {
                    Name = $"{topping.ToppingName}",
                    Value = $"x{topping.Quantity}"
                };

                stampOrderItem.Options.Add(orderItemOptionDetail);
            }

            return stampOrderItem;
        }

        private List<Domain.Entities.BillConfiguration> CreateBillConfiguration(Guid storeId, Guid accountId)
        {
            List<Domain.Entities.BillConfiguration> billConfigurations = new List<Domain.Entities.BillConfiguration>();

            foreach (int frameSize in Enum.GetValues(typeof(EnumBillFrameSize)))
            {
                var bill = new Domain.Entities.BillConfiguration()
                {
                    StoreId = storeId,
                    BillFrameSize = (EnumBillFrameSize)frameSize,
                    CreatedUser = accountId,
                    IsShowLogo = false,
                    IsShowAddress = true,
                    IsShowOrderTime = true,
                    IsShowCashierName = true,
                    IsShowCustomerName = true,
                    IsShowToping = true,
                    IsShowOption = true,
                    IsShowThanksMessage = true,
                    ThanksMessageData = "Thank you and have a nice day!"
                };

                if ((EnumBillFrameSize)frameSize == EnumBillFrameSize.Medium)
                {
                    bill.IsDefault = true;
                }

                billConfigurations.Add(bill);
            }

            return billConfigurations;
        }
    }
}
