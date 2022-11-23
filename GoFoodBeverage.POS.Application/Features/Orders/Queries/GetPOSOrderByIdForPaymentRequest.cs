using System;
using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces.POS;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using GoFoodBeverage.Domain.Entities;

namespace GoFoodBeverage.POS.Application.Features.Orders.Queries
{
    public class GetPOSOrderByIdForPaymentRequest : IRequest<GetOrderByIdResponse>
    {
        public Guid Id { get; set; }
    }

    public class GetOrderByIdResponse
    {
        public Guid Id { get; set; }

        public string Code { get; set; }

        public string OriginalCode { get; set; }

        public decimal OriginalPrice { get; set; }

        public decimal Price { get; set; }

        public bool IsShowMomoPayment { get; set; }

        public bool IsShowVnPayPayment { get; set; }

        public bool IsShowPaymentByCash { get; set; }
    }

    public class GetOrderByIdRequestHandler : IRequestHandler<GetPOSOrderByIdForPaymentRequest, GetOrderByIdResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IMapper _mapper;
        private readonly IOrderService _orderService;

        public GetOrderByIdRequestHandler(
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IMapper mapper,
            IOrderService orderService
            )
        {
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _mapper = mapper;
            _orderService = orderService;
        }

        public async Task<GetOrderByIdResponse> Handle(GetPOSOrderByIdForPaymentRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var order = await _unitOfWork.Orders
                .GetAllOrdersInStore(loggedUser.StoreId.Value)
                .AsNoTracking()
                .Select(o => new Order
                {
                    Id = o.Id,
                    Code = o.Code,
                    OriginalPrice = o.OriginalPrice,
                    TotalFee = o.TotalFee,
                    TotalTax = o.TotalTax,
                    CustomerDiscountAmount = o.CustomerDiscountAmount,
                    TotalDiscountAmount = o.TotalDiscountAmount,
                    DeliveryFee = o.DeliveryFee,
                })
                .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken: cancellationToken);

            var showMomoPaymentMethod = await _unitOfWork.PaymentConfigs.IsActiveAsync(loggedUser.StoreId, EnumPaymentMethod.MoMo);
            var showVnPaymentMethod = await _unitOfWork.PaymentConfigs.IsActiveAsync(loggedUser.StoreId, EnumPaymentMethod.VNPay);
            var showCashpaymentMethod = await _unitOfWork.PaymentConfigs.IsActiveAsync(loggedUser.StoreId, EnumPaymentMethod.Cash);

            var lastPrice = order.OriginalPrice + order.TotalFee + order.TotalTax - order.CustomerDiscountAmount + order.DeliveryFee - order.TotalDiscountAmount;
            var response = new GetOrderByIdResponse()
            {
                Id = order.Id,
                Code = order.StringCode,
                OriginalCode = order.Code,
                OriginalPrice = order.OriginalPrice,
                Price = lastPrice, //Last price
                IsShowMomoPayment = showMomoPaymentMethod,
                IsShowVnPayPayment = showVnPaymentMethod,
                IsShowPaymentByCash = showCashpaymentMethod
            };

            return response;
        }
    }
}
