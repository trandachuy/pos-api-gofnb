using System;
using MediatR;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.POS.Models.Order;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.POS.Models.Bill;
using static GoFoodBeverage.POS.Models.Order.DetailOrderToPrintModel;
using static GoFoodBeverage.POS.Models.Order.DetailOrderToPrintModel.ProductListModel;

namespace GoFoodBeverage.POS.Application.Features.Orders.Queries
{
    public class GetDetailOrderToPrintRequest : IRequest<GetDetailOrderToPrintResponse>
    {
        public Guid OrderId { get; set; }
    }

    public class GetDetailOrderToPrintResponse
    {
        public DetailOrderToPrintModel DetailOrderToPrint { get; set; }

        public BillModel BillConfiguration { get; set; }
    }

    public class GetDetailOrderToPrintRequestHandler : IRequestHandler<GetDetailOrderToPrintRequest, GetDetailOrderToPrintResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetDetailOrderToPrintRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IMapper mapper,
            MapperConfiguration mapperConfiguration
            )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mapper = mapper;
            _mapperConfiguration = mapperConfiguration;
        }

        public async Task<GetDetailOrderToPrintResponse> Handle(GetDetailOrderToPrintRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var orderDetail = await _unitOfWork.Orders
                .Find(x => x.StoreId == loggedUser.StoreId && x.Id == request.OrderId)
                .ProjectTo<DetailOrderToPrintModel>(_mapperConfiguration)
                .FirstOrDefaultAsync();

            var orderItems = orderDetail?.OrderItems?.Where(x => x.StatusId != EnumOrderItemStatus.Canceled);
            var orderItemGroups = new List<ProductListModel>();
            foreach (var item in orderItems)
            {
                if (item.IsCombo)
                {
                    var existed = ComboItemDuplicated(item.OrderComboItem, orderItemGroups);
                    if (existed == null)
                    {
                        orderItemGroups.Add(item);
                    }
                    else
                    {
                        existed.Quantity += item.Quantity;
                    }
                }
                else
                {
                    var existed = GetProductItemDuplicated(item, orderItemGroups);
                    if (existed == null)
                    {
                        orderItemGroups.Add(item);
                    }
                    else
                    {
                        existed.Quantity += item.Quantity;
                    }
                }    
            }
            orderDetail?.OrderItems.Clear();
            orderDetail?.OrderItems.AddRange(orderItemGroups);

            var billConfiguration = await _unitOfWork.Bills
                .GetAll()
                .Where(x => x.IsDefault == true && x.StoreId == loggedUser.StoreId.Value)
                .FirstOrDefaultAsync();

            if(billConfiguration == null)
            {
                var newBills = CreateBillConfiguration(loggedUser.StoreId.Value, loggedUser.AccountId.Value);
                _unitOfWork.Bills.AddRange(newBills);
                await _unitOfWork.SaveChangesAsync();

                billConfiguration = newBills.Where(x => x.IsDefault == true).FirstOrDefault();
            }

            if (billConfiguration.IsShowLogo)
            {
                var logo = await _unitOfWork.Stores.Find(x => x.Id == loggedUser.StoreId.Value).Select(x => x.Logo).FirstOrDefaultAsync();
                billConfiguration.LogoData = logo;
            }

            var billModel = _mapper.Map<BillModel>(billConfiguration);

            return new GetDetailOrderToPrintResponse {  BillConfiguration = billModel, DetailOrderToPrint = orderDetail };
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

        private static ProductListModel GetProductItemDuplicated(ProductListModel item, List<ProductListModel> result)
        {
            foreach (var existed in result)
            {
                var isProductDuplicated = (item.ProductPriceId == existed.ProductPriceId) && item.ProductPriceId != Guid.Empty;
                var isOptionDuplicated = item.OrderItemOptions.All(o => existed.OrderItemOptions.Any(e => e.OptionLevelId == o.OptionLevelId));
                var isToppingDuplicated = item.OrderItemToppings.All(o => existed.OrderItemToppings.Any(e => e.ToppingId == o.ToppingId && e.Quantity == o.Quantity));
                if (isProductDuplicated && isOptionDuplicated && isToppingDuplicated)
                {
                    return existed;
                }
            }
            return null;
        }

        private static ProductListModel ComboItemDuplicated(OrderComboItemDto item, List<ProductListModel> result)
        {
            foreach (var existed in result)
            {
                var existedCombo = existed.OrderComboItem;
                if (existedCombo == null) continue;

                var isComboDuplicated = (item.ComboId == existedCombo.ComboId);
                var comboItemsDuplicated = 0;
                foreach (var existedComboItem in existedCombo.OrderComboProductPriceItems)
                {
                    var listComboItemDuplicated = item.OrderComboProductPriceItems.Where(i => i.ProductPriceId == existedComboItem.ProductPriceId);
                    if (!listComboItemDuplicated.Any())
                    {
                        break;
                    };

                    foreach (var comboItemDuplicated in listComboItemDuplicated)
                    {
                        var isOptionDuplicated = existedComboItem.OrderItemOptions.All(o => comboItemDuplicated.OrderItemOptions.Any(e => e.OptionLevelId == o.OptionLevelId));
                        var isToppingDuplicated = existedComboItem.OrderItemToppings.All(o => comboItemDuplicated.OrderItemToppings.Any(e => e.ToppingId == o.ToppingId && e.Quantity == o.Quantity)) && existedComboItem.OrderItemOptions.Count == comboItemDuplicated.OrderItemOptions.Count;
                        if (isOptionDuplicated && isToppingDuplicated)
                        {
                            comboItemsDuplicated += 1;
                            break;
                        }
                    }
                }

                var isComboItemsDuplicated = comboItemsDuplicated == existedCombo.OrderComboProductPriceItems.Count();
                if (isComboDuplicated && isComboItemsDuplicated)
                {
                    return existed;
                }
            }

            return null;
        }
    }
}
