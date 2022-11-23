using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Interfaces.POS;
using GoFoodBeverage.Models.Payment;
using GoFoodBeverage.Payment.MoMo;
using GoFoodBeverage.Payment.MoMo.Enums;
using GoFoodBeverage.Payment.MoMo.Model;
using GoFoodBeverage.POS.Application.Features.Staff.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.Application.Features.Payments.Commands
{
    public class CreateMomoPaymentByCustomerCodeRequest : IRequest<CreatePosPaymentResponse>
    {
        /// <summary>
        /// The customer code scanned from MOMO e-wallet application
        /// </summary>
        public string PaymentCode { get; set; }

        public string OrderInfo { get; set; }

        public long Amount { get; set; }

        public Guid OrderId { get; set; }

        public string Lang { get; set; }
    }

    public class CreatePosPaymentResponse
    {
        public string Status { get; set; }

        public MessageResult Message { get; set; }

        public class MessageResult
        {
            public long Amount { get; set; }

            public string Description { get; set; }

            public string PhoneNumber { get; set; }

            public long TransId { get; set; }

            public string WalletId { get; set; }
        }
    }

    public class CreatePosPaymentRequestHandler : IRequestHandler<CreateMomoPaymentByCustomerCodeRequest, CreatePosPaymentResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMoMoPaymentService _momoPaymentService;
        private readonly IUserProvider _userProvider;
        private readonly IOrderService _orderService;
        private readonly IKitchenService _kitchenService;
        private readonly IMediator _mediator;

        public CreatePosPaymentRequestHandler(
            IUnitOfWork unitOfWork,
            IMoMoPaymentService momoPaymentService,
            IUserProvider userProvider,
            IOrderService orderService,
            IKitchenService kitchenService,
            IMediator mediator
        )
        {
            _unitOfWork = unitOfWork;
            _momoPaymentService = momoPaymentService;
            _userProvider = userProvider;
            _orderService = orderService;
            _kitchenService = kitchenService;
            _mediator = mediator;
        }

        public async Task<CreatePosPaymentResponse> Handle(CreateMomoPaymentByCustomerCodeRequest request, CancellationToken cancellationToken)
        {
            var loggedUser = await _userProvider.ProvideAsync(cancellationToken);
            var branchInfo = await _unitOfWork.StoreBranches.GetBranchByStoreIdAndBranchIdAsync(loggedUser.StoreId.Value, loggedUser.BranchId.Value);

            #region Create MOMO payment request data
            var paymentConfig = await _unitOfWork.PaymentConfigs.GetPaymentConfigAsync(loggedUser.StoreId.Value, EnumPaymentMethod.MoMo);
            var storeMomoPaymentConfig = new PartnerMoMoPaymentConfigModel()
            {
                PartnerCode = paymentConfig.PartnerCode,
                AccessKey = paymentConfig.AccessKey,
                SecretKey = paymentConfig.SecretKey,
            };

            var extractDataJson = new JObject
            {
                { "orderId", request.OrderId.ToString() },
                { "amount", request.Amount },
            };
            var extractData = _momoPaymentService.Base64Encode(extractDataJson);
            var orderPaymentTransaction = new OrderPaymentTransaction()
            {
                OrderId = request.OrderId,
                PaymentMethodId = (int)EnumPaymentMethod.MoMo,
                TransId = 0,
                OrderInfo = request?.OrderInfo,
                Amount = request.Amount,
                ExtraData = extractData,
                IsSuccess = false,
                CreatedUser = loggedUser.AccountId.Value
            };

            //Transaction logging
            await _unitOfWork.OrderPaymentTransactions.AddAsync(orderPaymentTransaction);

            var paymentRequest = new CreatePosGatewayRequest()
            {
                PartnerCode = storeMomoPaymentConfig.PartnerCode,
                OrderId = request.OrderId.ToString(),
                RequestId = orderPaymentTransaction.Id.ToString(),
                Amount = request.Amount,
                PaymentCode = request.PaymentCode,
                OrderInfo = request.OrderInfo,
                ExtraData = extractData,
                Lang = request.Lang,

                StoreId = loggedUser.BranchId.ToString(),
                StoreName = branchInfo.Name + " - " + branchInfo.Store.Title,
                OrderGroupId = branchInfo.Code, // Group order by branch code
            };
            #endregion

            //Momo payment by customer code
            //https://developers.momo.vn/v3/vi/docs/payment/api/quick-pay-v2/
            try
            {
                //Momo response
                var momoPaymentResponse = await _momoPaymentService.CreatePosGatewayAsync(storeMomoPaymentConfig, paymentRequest);
                string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(momoPaymentResponse);
                CreatePosPaymentResponse response = Newtonsoft.Json.JsonConvert.DeserializeObject<CreatePosPaymentResponse>(jsonString);

                //Update transaction logging
                var orderPaymentTrasaction = await _unitOfWork.OrderPaymentTransactions.GetOrderPaymentTransactionById(orderPaymentTransaction.Id);
                orderPaymentTrasaction.TransId = response.Message.TransId;
                orderPaymentTrasaction.ResponseData = response.Message.Description;
                orderPaymentTrasaction.IsSuccess = response.Status == MomoResponseCode.Success;
                _unitOfWork.OrderPaymentTransactions.Update(orderPaymentTrasaction);

                var order = await _unitOfWork.Orders.GetAllOrdersInStore(loggedUser.StoreId)
                       .Include(x => x.OrderItems).ThenInclude(i => i.Promotion)
                       .Include(x => x.OrderItems).ThenInclude(i => i.OrderItemOptions)
                       .Include(x => x.OrderItems).ThenInclude(i => i.OrderItemToppings)
                       .Include(x => x.OrderFees)
                       .Include(x => x.AreaTable).ThenInclude(x => x.Area)
                       .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken: cancellationToken);

                var oldOrder = order.ToJsonWithCamelCase();
                if (response.Status == MomoResponseCode.Success)
                {
                    order.PaymentMethodId = EnumPaymentMethod.MoMo;

                    //Handle update order and payment status
                    var orderStatus = await _orderService.GetOrderStatusAsync(order.OrderTypeId, null);
                    order.StatusId = orderStatus.OrderStatus;
                    order.OrderPaymentStatusId = EnumOrderPaymentStatus.Paid;
                }
                else
                {
                    //If the payment response is failed => change order status to Draft
                    order.OrderPaymentStatusId = null;
                    order.StatusId = EnumOrderStatus.Draft;
                }

                _unitOfWork.Orders.Update(order);
                await _unitOfWork.SaveChangesAsync();

                var newOrder = order.ToJsonWithCamelCase();
                var actionName = string.Format(EnumOrderHistoryActionName.UpdatePaymentStatus.GetName(), EnumPaymentMethod.MoMo.GetName());
                await _orderService.SaveOrderHistoryAsync(request.OrderId, oldOrder, newOrder, actionName, null, null);
                var storeConfig = await _unitOfWork.Stores.GetStoreConfigAsync(loggedUser.StoreId.Value);
                if (order.OrderPaymentStatusId == EnumOrderPaymentStatus.Paid)
                {
                    await _orderService.CalculatePoint(order, loggedUser.StoreId);
                    if (!storeConfig.IsPaymentLater)
                    {
                        await _orderService.CalMaterialQuantity(order.Id, false, false, EnumInventoryHistoryAction.CreateOrder);
                    }
                }

                await _mediator.Send(new CreateStaffActivitiesRequest()
                {
                    ActionGroup = EnumActionGroup.Order,
                    ActionType = EnumActionType.PaymentStatus,
                    ObjectId = order.Id,
                    ObjectName = order.Code,
                }, cancellationToken);

                await _kitchenService.GetKitchenOrderSessionsAsync(cancellationToken);

                return response;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
