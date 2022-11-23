using System;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GoogleServices.Distance;
using GoFoodBeverage.Interfaces;
using System.Collections.Generic;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Payment.VNPay;
using GoFoodBeverage.Interfaces.POS;
using Microsoft.EntityFrameworkCore;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.POS.Models.Order;
using GoFoodBeverage.POS.Models.Combo;
using GoFoodBeverage.Delivery.Ahamove;
using GoFoodBeverage.Common.Constants;
using GoFoodBeverage.Common.Exceptions;
using GoFoodBeverage.POS.Models.Product;
using GoFoodBeverage.Payment.VNPay.Model;
using GoFoodBeverage.Delivery.Ahamove.Model;
using GoFoodBeverage.Application.Features.Products.Queries;
using GoFoodBeverage.Common.Extensions;

namespace GoFoodBeverage.Application.Features.Orders.Commands
{
    public class CreateOrderRequest : IRequest<CreateOrderResponse>
    {
        /// <summary>
        /// List of product cart items
        /// </summary>
        public List<OrderCartItemRequestModel> CartItems { get; set; }

        /// <summary>
        /// Applied fees
        /// </summary>
        public IEnumerable<Guid> OrderFeeIds { get; set; }

        /// <summary>
        /// Customer id to get discount
        /// </summary>
        public Guid? CustomerId { get; set; }

        public Guid StoreId { get; set; }

        public Guid BranchId { get; set; }

        public EnumPaymentMethod PaymentMethodId { get; set; }

        public Guid DeliveryMethodId { get; set; }

        public EnumOrderPaymentStatus OrderPaymentStatusId { get; set; }

        public EnumOrderStatus OrderStatus { get; set; }

        public string Note { get; set; }

        public ReceiverAddressDto ReceiverAddress { get; set; }

        public class ReceiverAddressDto
        {
            public string Address { get; set; }

            public double Lat { get; set; }

            public double Lng { get; set; }
        }

        /// <summary>
        /// The URL of the POS website will be used to close the VNPay SDK.
        /// </summary>
        public string PosUrl { get; set; }
    }

    public class CreateOrderResponse
    {
        public bool Success { get; set; }

        public Guid OrderId { get; set; }

        public string Code { get; set; }

        public string VnPayUrl { get; set; }

        public string Message { get; set; }

        public VNPayOrderInfoModel VnPayInfo { get; set; }

        public VnPayTransactionInfo VnPayTransInfo { get; set; }

        public class VnPayTransactionInfo
        {
            public Guid OrderId { get; set; }

            public string TxnRef { get; set; }

            public string OrderInfo { get; set; }

            public string VnPayCreateDate { get; set; }
        }
    }

    public class CreateOrderRequestHandler : IRequestHandler<CreateOrderRequest, CreateOrderResponse>
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserProvider _userProvider;
        private readonly IOrderService _orderService;
        private readonly IVNPayService _vnPayService;
        private readonly IAhamoveService _ahamoveService;
        private readonly IGoogleDistanceService _googleDistanceService;

        public CreateOrderRequestHandler(
            IMediator mediator,
            IUnitOfWork unitOfWork,
            IUserProvider userProvider,
            IOrderService orderService,
            IVNPayService vnPayService,
            IAhamoveService ahamoveService,
            IGoogleDistanceService googleDistanceService
        )
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _userProvider = userProvider;
            _orderService = orderService;
            _vnPayService = vnPayService;
            _ahamoveService = ahamoveService;
            _googleDistanceService = googleDistanceService;
        }

        public async Task<CreateOrderResponse> Handle(CreateOrderRequest request, CancellationToken cancellationToken)
        {
            var dataToResponse = new CreateOrderResponse();

            var calculatingCartItems = new GetProductCartItemRequest()
            {
                CustomerId = request.CustomerId,
                CartItems = request.CartItems,
                OrderFeeIds = request.OrderFeeIds,
                BranchId = request.BranchId,
                StoreId = request.StoreId,
            };

            var calculatingCartItemsResult = await _mediator.Send(calculatingCartItems, cancellationToken);

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var customer = await _unitOfWork
                        .Customers
                        .GetAll()
                        .Include(c => c.CustomerPoint)
                        .Include(c => c.Account)
                        .FirstOrDefaultAsync(x => x.AccountId == request.CustomerId
                            && x.BranchId == request.BranchId
                            && x.PhoneNumber == x.Account.PhoneNumber);

                    if (customer != null && customer.CustomerPoint == null)
                    {
                        var customerPoint = new CustomerPoint()
                        {
                            CustomerId = customer.Id,
                            AccumulatedPoint = 0
                        };

                        customer.CustomerPoint = customerPoint;
                    }

                    if (customer == null)
                    {
                        customer = await CreateCustomer(request);
                    }

                    var order = await CreateOrderAsync(
                            calculatingCartItemsResult,
                            calculatingCartItemsResult.CartItems,
                            request,
                            customer,
                            cancellationToken
                        );

                    await transaction.CommitAsync();

                    dataToResponse.Success = true;
                    dataToResponse.OrderId = order.Id;
                    dataToResponse.Code = order.Code;

                    if (request.PaymentMethodId == EnumPaymentMethod.CreditDebitCard || request.PaymentMethodId == EnumPaymentMethod.VNPay)
                    {
                        // Get the store's payment configuration.
                        var paymentConfigForVnPay = await _unitOfWork.
                            PaymentConfigs.
                            GetPaymentConfigAsync(request.StoreId, EnumPaymentMethod.VNPay);

                        //Get store branch information
                        var storeInfo = _unitOfWork.StoreBranches.GetStoreBranchByIdAsync(request.BranchId)
                            .Include(x => x.Store)
                            .Select(x => new
                            {
                                StoreName = x.Store.Title,
                                BranchName = x.Name,
                            }).FirstOrDefault();

                        // The token is used to access the payment provider VNPay.
                        var config = new VNPayConfigModel();
                        config.TerminalId = paymentConfigForVnPay.PartnerCode;
                        config.SecretKey = paymentConfigForVnPay.SecretKey;

                        var orderTitle = $"Payment for order {order.Code} at {storeInfo.StoreName} - {storeInfo.BranchName}";

                        var orderInfo = new VNPayOrderInfoModel()
                        {
                            Title = orderTitle,
                            Amount = (long)order.PriceAfterDiscount + (long)order.DeliveryFee,
                            CreatedDate = DateTime.UtcNow,
                            Status = "0",
                            PayStatus = "",
                            CurrencyCode = "VND"
                        };

                        if (request.PaymentMethodId == EnumPaymentMethod.CreditDebitCard)
                        {
                            orderInfo.BankCode = VnPayBankCodeConstants.INTERNATIONAL_BANK_CARD;
                        }
                        else if (request.PaymentMethodId == EnumPaymentMethod.VNPay)
                        {
                            orderInfo.BankCode = VnPayBankCodeConstants.VNPAY_WALLET;
                        }

                        string urlWillBeCalledFromVnPay = $"{request.PosUrl}?orderCode={order.Code}&vnPayCreateDate={{0}}&paymentMethodId={request.PaymentMethodId}";

                        // Call the VNPay's service.
                        var paymentUrl = await _vnPayService.CreatePaymentUrlAsync(config, orderInfo, "vn", urlWillBeCalledFromVnPay);

                        orderInfo.OrderDesc = $"Desc: {orderInfo.OrderId}";
                        orderInfo.PaymentTranId = orderInfo.OrderId;

                        dataToResponse.VnPayUrl = paymentUrl;
                        dataToResponse.VnPayInfo = orderInfo;


                        // Add a new payment transaction.
                        var orderPaymentTransaction = new OrderPaymentTransaction()
                        {
                            IsSuccess = false,
                            OrderId = order.Id,
                            OrderInfo = orderTitle,
                            TransId = orderInfo.OrderId,
                            CreatedUser = request.CustomerId,
                            Amount = order.PriceAfterDiscount,
                            ExtraData = orderInfo.OrderId.ToString(),
                            PaymentMethodId = request.PaymentMethodId,
                        };

                        await _unitOfWork.OrderPaymentTransactions.AddAsync(orderPaymentTransaction);

                        dataToResponse.VnPayTransInfo = new CreateOrderResponse.VnPayTransactionInfo
                        {
                            OrderId = order.Id,
                            OrderInfo = orderInfo.Title,
                            TxnRef = $"{orderInfo.OrderId}",
                            VnPayCreateDate = orderInfo.VnPayCreateDate
                        };

                    }

                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }

            return dataToResponse;
        }

        private async Task<Order> CreateOrderAsync(
            GetProductCartItemResponse calculatingCartItemsResult,
            List<ProductCartItemModel> cartItems,
            CreateOrderRequest request,
            GoFoodBeverage.Domain.Entities.Customer customer,
            CancellationToken cancellationToken)
        {
            StoreBranch currentBranch = await _unitOfWork.
                    StoreBranches.
                    GetAll().
                    Include(sb => sb.Address).ThenInclude(ac => ac.Country)
                    .SingleOrDefaultAsync(sb => sb.Id == request.BranchId);

            var currentCustomer = await _unitOfWork.Accounts.GetAccountActivatedByIdAsync(request.CustomerId.Value);
            var storeTitle = await _unitOfWork.Stores.Find(x => x.Id == request.StoreId).Select(x => x.Title).FirstOrDefaultAsync();
            var senderAddress = await _unitOfWork.StoreBranches.GetStoreBranchByIdAsync(request.BranchId).Select(x => new { x.Address.Latitude, x.Address.Longitude }).FirstOrDefaultAsync();

            var totalTax = calculatingCartItemsResult?.TotalTax ?? 0;
            var totalFee = calculatingCartItemsResult?.TotalFee ?? 0;
            var totalDiscount = calculatingCartItemsResult.OriginalPrice - calculatingCartItemsResult.TotalPriceAfterDiscount + totalTax + totalFee;
            if (calculatingCartItemsResult.IsDiscountOnTotal && totalDiscount > calculatingCartItemsResult?.DiscountTotalPromotion?.MaximumDiscountAmount)
            {
                if (calculatingCartItemsResult.DiscountTotalPromotion.MaximumDiscountAmount > 0)
                {
                    totalDiscount = calculatingCartItemsResult.DiscountTotalPromotion.MaximumDiscountAmount;
                }
            }

            var newOrder = new Order()
            {
                StoreId = request.StoreId,
                BranchId = request.BranchId,
                CustomerId = customer.Id,
                PromotionId = calculatingCartItemsResult?.DiscountTotalPromotion?.Id,
                StatusId = EnumPaymentMethod.MoMo == request.PaymentMethodId ||
                            EnumPaymentMethod.VNPay == request.PaymentMethodId ? EnumOrderStatus.Draft : request.OrderStatus,
                OrderPaymentStatusId = request.OrderPaymentStatusId,
                OrderTypeId = EnumOrderType.Online,
                PaymentMethodId = request.PaymentMethodId,
                OriginalPrice = calculatingCartItemsResult.OriginalPrice,
                TotalDiscountAmount = totalDiscount,
                IsPromotionDiscountPercentage = false,
                CustomerDiscountAmount = calculatingCartItemsResult.CustomerDiscountAmount,
                CustomerMemberShipLevel = calculatingCartItemsResult.CustomerMemberShipLevel,
                PlatformId = EnumPlatform.GoFnBApp.ToGuid(),
                DeliveryMethodId = request.DeliveryMethodId,
                Note = request.Note,
                OrderItems = new List<OrderItem>(),
                OrderFees = new List<OrderFee>(),
                OrderSessions = new List<OrderSession>(),
                TotalTax = totalTax,
                TotalFee = totalFee,
            };

            if (request.StoreId != Guid.Empty)
            {
                var storeConfig = await _unitOfWork.StoreConfigs.GetStoreConfigByStoreIdAsync(request.StoreId);
                newOrder.Code = storeConfig.CurrentMaxOrderCode.ConvertCodeFormat();
                await _unitOfWork.StoreConfigs.UpdateStoreConfigAsync(storeConfig, StoreConfigConstants.ORDER_CODE);
            }

            if (cartItems != null && cartItems.Any())
            {
                var productPriceIds = calculatingCartItemsResult.CartItems.Select(c => c.ProductPriceId);
                var productPrices = await _unitOfWork.ProductPrices
                    .Find(p => productPriceIds.Any(id => id == p.Id))
                    .Include(p => p.Product).ThenInclude(p => p.PromotionProducts).ThenInclude(p => p.Promotion)
                    .Include(p => p.Product).ThenInclude(p => p.ProductProductCategories).ThenInclude(p => p.ProductCategory).ThenInclude(p => p.PromotionProductCategories)
                    .Include(p => p.ProductPriceMaterials).ThenInclude(ppm => ppm.Material)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken: cancellationToken);

                /// Insert data to order item table
                foreach (var cartItem in cartItems)
                {
                    if (cartItem.IsCombo)
                    {
                        var combo = cartItem.Combo;
                        var orderItem = await CreateComboOrderItemAsync(combo, newOrder.Id);
                        newOrder.OrderItems.Add(orderItem);
                    }
                    else
                    {
                        var productPrice = productPrices.FirstOrDefault(p => p.Id == cartItem.ProductPriceId);
                        var item = request.CartItems.FirstOrDefault(i => i.ProductPriceId == cartItem.ProductPriceId);
                        if (item == null || productPrice == null) continue;

                        var orderItem = CreateOrderItem(newOrder, productPrice, cartItem, item);
                        orderItem.OrderItemOptions = CreateOrderItemOptions(cartItem, orderItem);
                        orderItem.OrderItemToppings = CreateOrderItemToppings(cartItem, orderItem);
                        orderItem.ProductPriceName = productPrice.PriceName;
                        newOrder.OrderItems.Add(orderItem);
                    }

                    /// Insert data to order fee table
                    newOrder.OrderFees = await CreateOrderFeesAsync(request, request.StoreId, newOrder, cancellationToken); ;

                    // To do
                    //newOrder.TotalFee = _orderService.CalculateTotalFeeValue(newOrder.OriginalPrice, newOrder.OrderFees.Select(o => o.Fee));

                    newOrder.TotalCost = await _orderService.CalculateTotalProductCostAsync(calculatingCartItemsResult.CartItems, productPrices);
                }

                decimal deliveryFee = 0;
                string ahamoveOrderId = null;

                var selfDeliveryId = await _unitOfWork.DeliveryMethods.Find(p => p.EnumId == EnumDeliveryMethod.SelfDelivery).Select(dvm => dvm.Id).FirstOrDefaultAsync();

                if (request.DeliveryMethodId == selfDeliveryId)
                {
                    deliveryFee = await GetDeliveryFeeAsync(request.DeliveryMethodId, request.StoreId, request.BranchId, request.ReceiverAddress.Lat, request.ReceiverAddress.Lng, cancellationToken);
                }
                else
                {
                    var ahamoveConfig = await _unitOfWork.DeliveryMethods.Find(p => p.EnumId == EnumDeliveryMethod.AhaMove).Select(dvm => dvm.DeliveryConfigs.FirstOrDefault()).FirstOrDefaultAsync();

                    var senderAddressDetail = FormatAddress(currentBranch.Address);

                    deliveryFee = await GetEstimateAhaMoveOrderFeeAddressAsync(ahamoveConfig, senderAddressDetail, senderAddress.Latitude, senderAddress.Longitude, request.ReceiverAddress.Address, request.ReceiverAddress.Lat, request.ReceiverAddress.Lng);
                }
                newOrder.DeliveryFee = deliveryFee;

                OrderDelivery orderDelivery = new OrderDelivery()
                {
                    SenderName = currentBranch?.Name,
                    SenderPhone = currentBranch?.PhoneNumber,
                    SenderAddress = storeTitle,
                    SenderLat = senderAddress.Latitude,
                    SenderLng = senderAddress.Longitude,
                    ReceiverName = currentCustomer?.FullName,
                    ReceiverPhone = currentCustomer?.PhoneNumber,
                    ReceiverAddress = request.ReceiverAddress.Address,
                    ReceiverLat = request.ReceiverAddress.Lat,
                    ReceiverLng = request.ReceiverAddress.Lng
                };
                newOrder.OrderDelivery = orderDelivery;

                _unitOfWork.Orders.Add(newOrder);

                try
                {
                    await _unitOfWork.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            return newOrder;
        }

        /// <summary>
        /// Create an order item with field IsCombo is true and create related OrderComboItem object
        /// </summary>
        /// <param name="combo"></param>
        /// <param name="orderId"></param>
        /// <returns>OrderItem</returns>
        private async Task<OrderItem> CreateComboOrderItemAsync(ComboOrderItemDto combo, Guid orderId)
        {
            var comboOrderItem = new OrderItem()
            {
                OrderId = orderId,
                ProductPriceName = $"{combo.ItemName} {combo.ComboName}",
                OriginalPrice = combo.OriginalPrice,
                PriceAfterDiscount = combo.OriginalPrice,
                Quantity = combo.Quantity,
                StatusId = EnumOrderItemStatus.New,
                IsCombo = true,
                Notes = combo.Notes,
                OrderComboItem = new OrderComboItem()
                {
                    ComboId = combo.ComboId,
                    ComboPricingId = combo.ComboPricingId,
                    ComboName = combo.ComboName,
                    OriginalPrice = combo.OriginalPrice,
                    SellingPrice = combo.SellingPrice,
                    OrderComboProductPriceItems = new List<OrderComboProductPriceItem>()
                }
            };

            var allOptionLevelIds = combo.ComboItems.SelectMany(c => c.Options.Select(o => o.OptionLevelId));
            var allOptionLevels = await _unitOfWork.OptionLevels
                .Find(o => allOptionLevelIds.Any(oid => oid == o.Id))
                .Include(o => o.Option)
                .AsNoTracking()
                .ToListAsync();

            var allToppingIds = combo.ComboItems.SelectMany(c => c.Toppings.Select(t => t.ToppingId));
            var allToppings = await _unitOfWork.Products
                .Find(o => allToppingIds.Any(oid => oid == o.Id))
                .Include(t => t.ProductPrices)
                .AsNoTracking()
                .ToListAsync();

            foreach (var comboItem in combo.ComboItems)
            {
                var orderComboProductPriceItem = new OrderComboProductPriceItem()
                {
                    OrderComboItemId = comboOrderItem.OrderComboItem.Id,
                    ProductPriceId = comboItem.ProductPriceId,
                    ItemName = comboItem.ItemName,
                    Note = comboItem.Note,
                    StatusId = EnumOrderItemStatus.New,
                    OrderItemOptions = new List<OrderItemOption>(),
                    OrderItemToppings = new List<OrderItemTopping>(),
                };

                foreach (var requestTopping in comboItem.Toppings)
                {
                    var topping = allToppings.FirstOrDefault(t => t.Id == requestTopping.ToppingId);

                    /// If the topping price is not found, continues
                    if (topping == null) continue;

                    /// The topping as a product and the product has multiple prices but the topping ONLY one price.
                    var toppingPrice = topping.ProductPrices.FirstOrDefault();

                    /// The topping price in combo will be not apply promotion.
                    var originalPrice = (toppingPrice?.PriceValue ?? 0) * requestTopping.Quantity;
                    var toppingDto = new OrderItemTopping()
                    {
                        OrderItemId = comboOrderItem.Id,
                        ToppingId = topping.Id,
                        ToppingName = topping.Name,
                        ToppingValue = toppingPrice.PriceValue,
                        OriginalPrice = originalPrice,
                        PriceAfterDiscount = originalPrice,
                        Quantity = requestTopping.Quantity
                    };

                    orderComboProductPriceItem.OrderItemToppings.Add(toppingDto);
                }

                foreach (var requestOption in comboItem.Options)
                {
                    var optionLevel = allOptionLevels.FirstOrDefault(o => o.Id == requestOption.OptionLevelId);

                    /// If the option level is not found, continues
                    if (optionLevel == null) continue;

                    var optionLevelDto = new OrderItemOption()
                    {
                        OptionId = optionLevel.OptionId,
                        OptionLevelId = optionLevel.Id,
                        OrderItemId = comboOrderItem.Id,
                        OptionName = optionLevel.Option.Name,
                        OptionLevelName = optionLevel.Name,
                    };

                    orderComboProductPriceItem.OrderItemOptions.Add(optionLevelDto);
                }

                comboOrderItem.OrderComboItem.OrderComboProductPriceItems.Add(orderComboProductPriceItem);
            }

            var totalToppingPricePerItem = comboOrderItem.OrderComboItem.OrderComboProductPriceItems.SelectMany(c => c.OrderItemToppings).Sum(t => t.PriceAfterDiscount * t.Quantity);
            var totalToppingPrice = totalToppingPricePerItem * comboOrderItem.Quantity;
            comboOrderItem.OriginalPrice = comboOrderItem.OrderComboItem.OriginalPrice + totalToppingPrice;
            comboOrderItem.PriceAfterDiscount = comboOrderItem.OrderComboItem.SellingPrice + totalToppingPrice;

            return comboOrderItem;
        }


        private static OrderItem CreateOrderItem(Order order,
            ProductPrice productPrice,
            ProductCartItemModel cartItem,
            OrderCartItemRequestModel requestItem
        )
        {
            var orderItem = new OrderItem()
            {
                OrderId = order.Id,
                ProductId = productPrice.ProductId,
                ProductName = productPrice?.Product?.Name,
                ProductPriceId = productPrice.Id,
                OriginalPrice = productPrice.PriceValue,
                PriceAfterDiscount = cartItem.PriceAfterDiscount,
                ProductPriceName = $"{cartItem.ItemName}",
                Quantity = cartItem.Quantity,
                Notes = requestItem.Notes,
                PromotionId = cartItem?.Promotion?.Id,
                PromotionName = cartItem?.Promotion?.Name,
                IsPromotionDiscountPercentage = cartItem?.Promotion?.IsPercentDiscount ?? false,
                PromotionDiscountValue = cartItem?.Promotion?.DiscountValue ?? 0,
                OrderItemOptions = new List<OrderItemOption>(),
                OrderItemToppings = new List<OrderItemTopping>(),
                StatusId = EnumOrderItemStatus.New
            };

            return orderItem;
        }

        private static List<OrderItemOption> CreateOrderItemOptions(ProductCartItemModel cartItem, OrderItem orderItem)
        {
            var orderItemOptions = new List<OrderItemOption>();
            if (cartItem.Options != null && cartItem.Options.Any())
            {
                foreach (var option in cartItem.Options)
                {
                    var orderItemOption = new OrderItemOption()
                    {
                        OrderItemId = orderItem.Id,
                        OptionLevelId = option.OptionLevelId,
                        OptionId = option.OptionId,
                        OptionName = option.OptionName,
                        OptionLevelName = option.OptionLevelName
                    };

                    orderItemOptions.Add(orderItemOption);
                }
            }

            return orderItemOptions;
        }

        private static List<OrderItemTopping> CreateOrderItemToppings(ProductCartItemModel cartItem, OrderItem orderItem)
        {
            var orderItemToppings = new List<OrderItemTopping>();
            if (cartItem.Toppings != null && cartItem.Toppings.Any())
            {
                foreach (var topping in cartItem.Toppings)
                {
                    var orderItemTopping = new OrderItemTopping()
                    {
                        OrderItemId = orderItem.Id,
                        ToppingId = topping.ToppingId,
                        PromotionId = topping.PromotionId,
                        ToppingName = topping.Name,
                        ToppingValue = topping.Price,
                        Quantity = topping.Quantity,
                        OriginalPrice = topping.OriginalPrice,
                        PriceAfterDiscount = topping.PriceAfterDiscount,
                    };

                    orderItemToppings.Add(orderItemTopping);
                }
            }

            return orderItemToppings;
        }

        private async Task<List<OrderFee>> CreateOrderFeesAsync(CreateOrderRequest request, Guid storeId, Order order, CancellationToken cancellationToken)
        {
            var orderFees = new List<OrderFee>();
            if (request.OrderFeeIds != null && request.OrderFeeIds.Any())
            {
                var fees = await _unitOfWork.Fees
                      .GetAllFeesInStore(storeId)
                      .Where(f => request.OrderFeeIds.Any(feeId => feeId == f.Id))
                      .AsNoTracking()
                      .ToListAsync(cancellationToken: cancellationToken);

                foreach (var fee in fees)
                {
                    var orderFee = new OrderFee()
                    {
                        FeeId = fee.Id,
                        FeeName = fee.Name,
                        FeeValue = fee.Value,
                        IsPercentage = fee.IsPercentage,
                        OrderId = order.Id
                    };

                    orderFees.Add(orderFee);
                }
            }

            return orderFees;
        }

        private async Task<decimal> GetDeliveryFeeAsync(Guid deliveryMethodId, Guid storeId, Guid branchId, double receiverAddressLat, double receiverAddressLng, CancellationToken cancellationToken)
        {
            decimal deliveryFee = 0;

            var branchDetail = await _unitOfWork.StoreBranches
                .GetStoreBranchByStoreIdAndBranchIdAsync(storeId, branchId)
                .Select(b => new { b.Address.Latitude, b.Address.Longitude })
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            var senderLat = branchDetail.Latitude.HasValue ? branchDetail.Latitude.Value : 0;
            var senderLng = branchDetail.Longitude.HasValue ? branchDetail.Longitude.Value : 0;

            var distanceBetweenPoints = await _googleDistanceService.GetDistanceBetweenPointsAsync(senderLat, senderLng, receiverAddressLat, receiverAddressLng, cancellationToken);

            var deliveryConfig = await _unitOfWork.DeliveryConfigs.GetDeliveryConfigByDeliveryMethodIdAsync(deliveryMethodId, storeId);

            if (deliveryConfig != null)
            {
                if (deliveryConfig.DeliveryMethodEnumId == EnumDeliveryMethod.SelfDelivery)
                {
                    if ((bool)deliveryConfig.IsFixedFee)
                    {
                        deliveryFee = deliveryConfig.FeeValue.Value;
                    }
                    else if (deliveryConfig.DeliveryConfigPricings.Count() > 0)
                    {
                        var deliveryConfigPrice = deliveryConfig.DeliveryConfigPricings.FirstOrDefault(dvcp => dvcp.FromDistance * 1000 < distanceBetweenPoints && distanceBetweenPoints <= dvcp.ToDistance * 1000);

                        if (deliveryConfigPrice != null)
                        {
                            deliveryFee = deliveryConfigPrice.FeeValue;
                        }
                        else
                        {
                            var configPrice = deliveryConfig.DeliveryConfigPricings.OrderByDescending(dvp => dvp.ToDistance).FirstOrDefault();
                            deliveryFee = configPrice.FeeValue;
                        }
                    }
                }
            }

            return deliveryFee;
        }

        private static string FormatAddress(Address address)
        {
            var isDefaultCountry = address.Country.Iso == DefaultConstants.DEFAULT_NEW_STORE_COUNTRY_ISO;

            List<string> addressComponents = new();
            if (address != null && !string.IsNullOrWhiteSpace(address.Address1))
            {
                addressComponents.Add(address.Address1);
            }
            if (address != null && !string.IsNullOrWhiteSpace(address.Address2) && !isDefaultCountry)
            {
                addressComponents.Add(address.Address2);
            }
            if (address != null && address.Ward != null && !string.IsNullOrWhiteSpace(address.Ward.Name) && isDefaultCountry)
            {
                addressComponents.Add(address.Ward.Name);
            }
            if (address != null && address.District != null && !string.IsNullOrWhiteSpace(address.District.Name) && isDefaultCountry)
            {
                addressComponents.Add(address.District.Name);
            }
            if (address != null && address.City != null && !string.IsNullOrWhiteSpace(address.City.Name) && isDefaultCountry)
            {
                addressComponents.Add(address.City.Name);
            }
            if (address != null && !string.IsNullOrWhiteSpace(address.CityTown) && !isDefaultCountry)
            {
                addressComponents.Add(address.CityTown);
            }
            if (address != null && address.State != null && !string.IsNullOrWhiteSpace(address.State.Name) && !isDefaultCountry)
            {
                addressComponents.Add(address.State.Name);
            }
            if (!string.IsNullOrWhiteSpace(address.Country.Nicename))
            {
                addressComponents.Add(address.Country.Nicename);
            }

            return string.Join(", ", addressComponents);
        }

        public async Task<Domain.Entities.Customer> CreateCustomer(CreateOrderRequest request)
        {
            var account = await _unitOfWork.Accounts.GetAll().FirstOrDefaultAsync(x => x.Id == request.CustomerId);
            ThrowError.Against(account == null, "Cannot find the account.");

            Domain.Entities.Customer customer = new();
            customer.AccountId = account.Id;
            customer.BranchId = request.BranchId;
            customer.StoreId = request.StoreId;
            customer.PlatformId = EnumPlatform.GoFnBApp.ToGuid();
            customer.PhoneNumber = account.PhoneNumber;
            customer.FullName = account.FullName;
            customer.Status = EnumCustomerStatus.Active;
            customer.BranchId = request.BranchId;

            customer.CustomerPoint = new CustomerPoint()
            {
                CustomerId = customer.Id,
                AccumulatedPoint = 0
            };

            // Save to the database.
            await _unitOfWork.Customers.AddAsync(customer);

            return customer;
        }

        private async Task<decimal> GetEstimateAhaMoveOrderFeeAddressAsync(DeliveryConfig ahamoveConfig, string senderAddress, double? senderLatitude, double? senderLongitude, string receiverAddress, double receiverLatitude, double receiverLongitude)
        {
            decimal ahaMoveOrderFee = 0;

            var senderAddressDetail = new EstimateOrderAhamoveRequestModel.AhamoveAddressDto()
            {
                Address = senderAddress,
                Lat = senderLatitude.HasValue ? senderLatitude.Value : 0,
                Lng = senderLongitude.HasValue ? senderLongitude.Value : 0,
            };

            var receiverAddressDetail = new EstimateOrderAhamoveRequestModel.AhamoveAddressDto()
            {
                Address = receiverAddress,
                Lat = receiverLatitude,
                Lng = receiverLongitude,
            };

            var estimateOrderAhamoveRequest = new EstimateOrderAhamoveRequestModel()
            {
                SenderAddress = senderAddressDetail,
                ReceiverAddress = receiverAddressDetail
            };

            var ahamoveConfigRequest = new AhamoveConfigByStoreRequestModel()
            {
                Mobile = ahamoveConfig.PhoneNumber,
                Name = ahamoveConfig.Name,
                ApiKey = ahamoveConfig.ApiKey,
                Address = ahamoveConfig.Address,
            };
            var infoToken = await _ahamoveService.GetAhamoveTokenAsync(ahamoveConfigRequest);

            var estimateOrderFeeResponse = await _ahamoveService.EstimateOrderFee(infoToken.Token, estimateOrderAhamoveRequest);

            if (estimateOrderFeeResponse != null)
            {
                ahaMoveOrderFee = estimateOrderFeeResponse.TotalPrice;
            }

            return ahaMoveOrderFee;
        }
    }
}
