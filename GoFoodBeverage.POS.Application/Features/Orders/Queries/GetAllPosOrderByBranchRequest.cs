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

namespace GoFoodBeverage.POS.Application.Features.Orders.Queries
{
    public class GetAllPosOrderByBranchRequest : IRequest<GetAllPosOrderByBranchResponse>
    {
        public EnumOrderStatus OrderStatus { get; set; }

        public string KeySearch { get; set; }

        public IEnumerable<int> OrderType { get; set; }

        public IEnumerable<Guid> Platform { get; set; }
    }

    public class GetAllPosOrderByBranchResponse
    {
        public IEnumerable<PosOrderModel> PosOrders { get; set; }
    }

    public class GetAllPosOrderByBranchRequestHandler : IRequestHandler<GetAllPosOrderByBranchRequest, GetAllPosOrderByBranchResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _mapperConfiguration;

        public GetAllPosOrderByBranchRequestHandler(
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

        public async Task<GetAllPosOrderByBranchResponse> Handle(GetAllPosOrderByBranchRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);

            DateTime startDate = DateTime.UtcNow.Date;
            DateTime endDate = startDate.AddDays(1).AddSeconds(-1);

            var listOrder = _unitOfWork.Orders
                    .GetOrdersByBranchIdInStore(loggedUser?.StoreId, loggedUser?.BranchId)
                    .Include(o => o.AreaTable).ThenInclude(at => at.Area)
                    .Include(o => o.OrderItems)
                    .Include(o => o.OrderDelivery)
                    .Include(o => o.Platform)
                    .Where(o => o.CreatedTime >= startDate && o.CreatedTime <= endDate);

            switch(request.OrderStatus)
            {
                case EnumOrderStatus.Completed:
                    listOrder = listOrder.Where(o => o.StatusId == EnumOrderStatus.Completed);
                    break;
                case EnumOrderStatus.Canceled:
                    listOrder = listOrder.Where(o => o.StatusId == EnumOrderStatus.Canceled);
                    break;
                case EnumOrderStatus.Draft:
                    listOrder = listOrder.Where(o => o.StatusId == EnumOrderStatus.Draft);
                    break;
                case EnumOrderStatus.ToConfirm:
                    listOrder = listOrder.Where(item => item.StatusId == EnumOrderStatus.ToConfirm && item.OrderTypeId == EnumOrderType.Online);
                    if (request.Platform != null && request.Platform.Any())
                    {
                        listOrder = listOrder.Where(item => item.Platform != null
                            && request.Platform.Contains(item.Platform.Id));
                    }
                    break;
                default:
                    listOrder = listOrder.Where(o => o.StatusId != EnumOrderStatus.Completed
                        && o.StatusId != EnumOrderStatus.Draft
                        && o.StatusId != EnumOrderStatus.Canceled
                        && o.StatusId != EnumOrderStatus.ToConfirm);
                    break;
            }
            
            if (request.OrderType.Any())
            {
                listOrder = listOrder.Where(o => request.OrderType.Any(type => type == (int)o.OrderTypeId));
            }

            string keySearch = request?.KeySearch?.Trim().ToUpper();
            if (!string.IsNullOrEmpty(keySearch))
            {
                int orderCode = -1;
                if (int.TryParse(keySearch, out orderCode))
                {
                    listOrder = listOrder.Where(o => o.Code.Contains(orderCode.ToString()));
                }
                else
                {
                    string firstCharacterFromString = keySearch.Substring(0, 1)?.ToUpper();
                    if (keySearch.Length > 1)
                    {
                        string numberFromString = keySearch[1..];
                        _ = int.TryParse(numberFromString, out orderCode);
                    }

                    switch (firstCharacterFromString)
                    {
                        case EnumOrderTypeSymbol.InStore:
                            listOrder = listOrder.Where(o => orderCode > 0 ? (o.Code.Contains(orderCode.ToString()) && o.OrderTypeId == EnumOrderType.Instore) : o.OrderTypeId == EnumOrderType.Instore);
                            break;
                        case EnumOrderTypeSymbol.Delivery:
                            listOrder = listOrder.Where(o => orderCode > 0 ? (o.Code.Contains(orderCode.ToString()) && o.OrderTypeId == EnumOrderType.Delivery) : o.OrderTypeId == EnumOrderType.Delivery);
                            break;
                        case EnumOrderTypeSymbol.TakeAway:
                            listOrder = listOrder.Where(o => orderCode > 0 ? (o.Code.Contains(orderCode.ToString()) && o.OrderTypeId == EnumOrderType.TakeAway) : o.OrderTypeId == EnumOrderType.TakeAway);
                            break;
                        case EnumOrderTypeSymbol.Online:
                            listOrder = listOrder.Where(o => orderCode > 0 ? (o.Code.Contains(orderCode.ToString()) && o.OrderTypeId == EnumOrderType.Online) : o.OrderTypeId == EnumOrderType.Online);
                            break;
                        default:
                            var emptyList = new List<PosOrderModel>();
                            return new GetAllPosOrderByBranchResponse()
                            {
                                PosOrders = emptyList,
                            };
                    }
                }
            }

            var listPosOrderModels = await listOrder
                .AsNoTracking()
                .ProjectTo<PosOrderModel>(_mapperConfiguration)
                .OrderByDescending(o => o.CreatedTime)
                .ToListAsync(cancellationToken: cancellationToken);

            var response = new GetAllPosOrderByBranchResponse()
            {
                PosOrders = listPosOrderModels,
            };

            return response;
        }
    }
}
