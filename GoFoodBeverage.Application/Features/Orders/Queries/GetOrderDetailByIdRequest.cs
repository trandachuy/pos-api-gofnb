using System;
using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using GoFoodBeverage.Models.Order;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using DocumentFormat.OpenXml.Office.CustomUI;

namespace GoFoodBeverage.Application.Features.Orders.Queries
{
    public class GetOrderDetailByIdRequest : IRequest<GetOrderDetailByIdResponse>
    {
        public Guid Id { get; set; }

        public Guid BranchId { get; set; }
    }

    public class GetOrderDetailByIdResponse
    {
        public OrderDetailByIdModel Order { get; set; }

        public IEnumerable<OrderItemModel> OrderItem { get; set; }
    }

    public class GetOrderDetailByIdRequestHandler : IRequestHandler<GetOrderDetailByIdRequest, GetOrderDetailByIdResponse>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;

        public GetOrderDetailByIdRequestHandler(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IUserProvider userProvider
            )
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
        }

        public async Task<GetOrderDetailByIdResponse> Handle(GetOrderDetailByIdRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = _userProvider.GetLoggedCustomer();
            var order = await _unitOfWork.Orders.Find(item => item.Id == request.Id)
                .Include(od => od.OrderDelivery)
                .Include(st => st.Store).ThenInclude(a => a.StoreBranches).ThenInclude(item => item.Address)
                .Include(st => st.Store).ThenInclude(a => a.Currency)
                .FirstOrDefaultAsync();
            var orderItems = await _unitOfWork.OrderItems.GetOrderItemByOrderIdAsync(request.Id);

            var optionLevelDefaultIds = await _unitOfWork.OptionLevels.Find(ol => ol.StoreId == order.StoreId && ol.IsSetDefault == true).Select(ol => ol.Id).ToListAsync(cancellationToken: cancellationToken);

            foreach (var orderItem in orderItems)
            {
                if(orderItem.IsCombo)
                {
                    foreach (var orderComboProductPriceItem in orderItem.OrderComboItem.OrderComboProductPriceItems)
                    {
                        var comboProductOptionLevels = orderComboProductPriceItem.OrderItemOptions.Where(ol => !optionLevelDefaultIds.Contains(ol.OptionLevelId.Value)).ToList();
                        orderComboProductPriceItem.OrderItemOptions = comboProductOptionLevels;
                    }
                } else
                {
                    var productOptionLevels = orderItem.OrderItemOptions.Where(ol => !optionLevelDefaultIds.Contains(ol.OptionLevelId.Value)).ToList();
                    orderItem.OrderItemOptions = productOptionLevels;
                }
            }

            var orderDetailModel = _mapper.Map<OrderDetailByIdModel>(order);
            var orderItemModel = _mapper.Map<IList<OrderItemModel>>(orderItems);

            var comboIds = orderItems.Where(item => item.IsCombo).Select(item => item.OrderComboItem.ComboId);
            var comboThumbnail = await _unitOfWork.Combos.Find(item => comboIds.Contains(item.Id)).ToListAsync();

            foreach (var item in orderItemModel)
            {
                if (item.IsCombo == false) continue;

                var thumbnail = comboThumbnail.FirstOrDefault(combo => combo.Id == item.OrderComboItem.ComboId).Thumbnail;
                item.OrderComboItem.Thumbnail = thumbnail;
            }

            var branch = order.Store.StoreBranches.FirstOrDefault(item => item.Id == order.BranchId);
            if (branch != null)
            {
                var countries = await _unitOfWork.Countries
                    .GetCountryByIdAsync(branch.Address.CountryId.Value);
                orderDetailModel.OrderDelivery.PhoneCode = countries?.Phonecode;
            }

            var response = new GetOrderDetailByIdResponse()
            {
                Order = orderDetailModel,
                OrderItem = orderItemModel
            };

            return response;
        }
    }
}
