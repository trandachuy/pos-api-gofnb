using AutoMapper;
using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.Delivery.Ahamove;
using GoFoodBeverage.Delivery.Ahamove.Constants;
using GoFoodBeverage.Delivery.Ahamove.Model;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Interfaces.POS;
using GoFoodBeverage.Models.Order;
using GoFoodBeverage.POS.Models.DeliveryMethod;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.Services.Store
{
    public class DeliveryService : IDeliveryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IDateTimeService _dateTimeService;
        private readonly IAhamoveService _ahamoveService;

        public DeliveryService(
            IUnitOfWork unitOfWork,
            IDateTimeService dateTimeService,
            IMapper mapper,
            IAhamoveService ahamoveService
            )
        {
            _unitOfWork = unitOfWork;
            _dateTimeService = dateTimeService;
            _mapper = mapper;
            _ahamoveService = ahamoveService;
        }

        public async Task UpdateOrderAhamoveStatusAsync(UpdateAhamoveStatusRequestModel request, CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.Orders.Find(x => x.AhamoveOrderId != null && x.AhamoveOrderId == request.Id)
                            .Include(x => x.OrderItems).ThenInclude(i => i.Promotion)
                            .Include(x => x.OrderItems).ThenInclude(i => i.OrderItemOptions)
                            .Include(x => x.OrderItems).ThenInclude(i => i.OrderItemToppings)
                            .Include(x => x.OrderFees)
                            .Include(x => x.OrderSessions)
                            .FirstOrDefaultAsync(cancellationToken);

            if (order != null)
            {
                var oldOrder = order.ToJsonWithCamelCase();
                var orderItems = order?.OrderItems?.ToList();
                var orderSessions = order?.OrderSessions?.ToList();

                switch (request.Status)
                {
                    case AhamoveOrderStatus.IN_PROCESS:
                        order.StatusId = EnumOrderStatus.Delivering;
                        order.OrderPaymentStatusId = EnumOrderPaymentStatus.Paid;
                        break;

                    case AhamoveOrderStatus.COMPLETED:
                        order.StatusId = EnumOrderStatus.Completed;
                        order.OrderPaymentStatusId = EnumOrderPaymentStatus.Paid;

                        /// Update order item status
                        foreach (var orderItem in orderItems)
                        {
                            orderItem.StatusId = EnumOrderItemStatus.Completed;
                        }

                        _unitOfWork.OrderItems.UpdateRange(orderItems);

                        /// Update order session status
                        foreach (var session in orderSessions)
                        {
                            session.StatusId = EnumOrderSessionStatus.Completed;
                        }

                        _unitOfWork.OrderSessions.UpdateRange(orderSessions);

                        break;

                    case AhamoveOrderStatus.CANCELLED:
                        order.StatusId = EnumOrderStatus.Canceled;
                        order.OrderPaymentStatusId = EnumOrderPaymentStatus.Unpaid;

                        foreach (var orderItem in orderItems)
                        {
                            orderItem.StatusId = EnumOrderItemStatus.Canceled;
                        }

                        _unitOfWork.OrderItems.UpdateRange(orderItems);

                        break;

                    default:
                        break;
                }

                var orderDeliveryHistoryModel = _mapper.Map<OrderDeliveryHistoryModel>(order);
                orderDeliveryHistoryModel.UpdateDeliveryStatusRequest = request.ToJsonWithCamelCase();
                var newOrderData = orderDeliveryHistoryModel.ToJsonWithCamelCase();

                await _unitOfWork.Orders.UpdateAsync(order);

                ///Add order history
                var orderHistoryAddModel = new OrderHistory
                {
                    OrderId = order.Id,
                    OldOrrderData = oldOrder,
                    NewOrderData = newOrderData,
                    ActionName = EnumOrderHistoryActionName.UpdateOrderAhamoveStatus.GetName(),
                    Note = EnumOrderHistoryActionName.UpdateOrderAhamoveStatus.GetNote(),
                    CreatedTime = _dateTimeService.NowUtc
                };

                await _unitOfWork.OrderHistories.AddAsync(orderHistoryAddModel);

                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<bool> CancelOrderAhamoveAsync(string ahamoveOrderId, Guid storeId, string comment)
        {
            var ahamoveConfig = await _unitOfWork.DeliveryConfigs.Find(dc => dc.StoreId == storeId && dc.DeliveryMethodEnumId == EnumDeliveryMethod.AhaMove).FirstOrDefaultAsync();
            bool isSuccess = false;
            var ahamoveConfigRequest = new AhamoveConfigByStoreRequestModel()
            {
                Mobile = ahamoveConfig.PhoneNumber,
                Name = ahamoveConfig.Name,
                ApiKey = ahamoveConfig.ApiKey,
                Address = ahamoveConfig.Address,
            };

            var infoToken = await _ahamoveService.GetAhamoveTokenAsync(ahamoveConfigRequest);
            if (infoToken != null)
            {
                isSuccess = await _ahamoveService.CancelOrderAsync(infoToken.Token, ahamoveOrderId, comment);
            }

            return isSuccess;
        }
    }
}
