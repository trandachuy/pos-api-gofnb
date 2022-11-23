using AutoMapper;
using AutoMapper.QueryableExtensions;
using GoFoodBeverage.Common.Extensions;
using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Interfaces;
using GoFoodBeverage.Interfaces.POS;
using GoFoodBeverage.Models.Order;
using GoFoodBeverage.POS.Models.Area;
using GoFoodBeverage.POS.Models.Customer;
using GoFoodBeverage.POS.Models.Fee;
using GoFoodBeverage.POS.Models.Material;
using GoFoodBeverage.POS.Models.Order;
using GoFoodBeverage.POS.Models.Product;
using GoFoodBeverage.POS.Models.Store;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoFoodBeverage.Services.Order
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _mapperConfiguration;
        private readonly IUserProvider _userProvider;
        private readonly IDateTimeService _dateTimeService;
        private readonly IPaymentService _paymentService;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, MapperConfiguration mapperConfiguration, IUserProvider userProvider, IDateTimeService dateTimeService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mapperConfiguration = mapperConfiguration;
            _userProvider = userProvider;
            _dateTimeService = dateTimeService;
        }

        public decimal CalculateTotalFeeValue(decimal originalPrice, IEnumerable<Fee> fees)
        {
            decimal totalFeeValue = 0;
            foreach (var fee in fees)
            {
                totalFeeValue += fee.IsPercentage ? (originalPrice * fee.Value) / 100 : fee.Value;
            }

            return totalFeeValue;
        }

        public decimal CalculateTotalFeeValue(decimal originalPrice, IEnumerable<FeeModel> fees)
        {
            decimal totalFeeValue = 0;
            foreach (var fee in fees)
            {
                totalFeeValue += fee.IsPercentage ? (originalPrice * fee.Value) / 100 : fee.Value;
            }

            return totalFeeValue;
        }

        public List<Tuple<OrderItem, List<OrderItem>>> MergeSameOrderItems(List<OrderItem> orderItems)
        {
            var result = new List<Tuple<OrderItem, List<OrderItem>>>();
            foreach (var item in orderItems)
            {
                var existed = GetProductItemDuplicated(item, result);
                if (existed == null)
                {
                    var newOrderItemAndGroupOrderItem = Tuple.Create(item, new List<OrderItem>() { item });
                    result.Add(newOrderItemAndGroupOrderItem);
                }
                else
                {
                    var orderItemAndGroupOrderItem = result.FirstOrDefault(t => t.Item1.ProductPriceId == existed.ProductPriceId);
                    orderItemAndGroupOrderItem.Item2.Add(item);
                    var newOrderItemAndGroupOrderItem = Tuple.Create(existed, orderItemAndGroupOrderItem.Item2);
                    result.Add(newOrderItemAndGroupOrderItem);
                    result.Remove(orderItemAndGroupOrderItem);
                }
            }

            return result;
        }

        /// <summary>
        /// Calculate total cost of all product items an order. 
        /// ProductPrices should be include ProductPriceMaterials > material
        /// </summary>
        /// <param name="cartItems"></param>
        /// <param name="productPrices"></param>
        /// <returns></returns>
        public async Task<decimal> CalculateTotalProductCostAsync(IEnumerable<ProductCartItemModel> cartItems, List<ProductPrice> productPrices)
        {
            var allOptionLevelIds = cartItems.Where(i => i.Options != null).SelectMany(i => i.Options.Select(o => o.OptionLevelId));
            var allOptionLevels = await _unitOfWork.OptionLevels
                .Find(o => allOptionLevelIds.Any(oid => oid == o.Id))
                .AsNoTracking()
                .Select(o => new
                {
                    o.Id,
                    o.Quota
                })
                .ToListAsync();

            decimal totalProductCost = 0;
            foreach (var cartItem in cartItems)
            {
                if (cartItem.Options != null && cartItem.Options.Any())
                {
                    var productPrice = productPrices.FirstOrDefault(p => p.Id == cartItem.ProductPriceId);
                    if (productPrice == null) continue;

                    foreach (var itemOption in cartItem.Options)
                    {
                        var optionLevel = allOptionLevels.FirstOrDefault(o => o.Id == itemOption.OptionLevelId);
                        if (optionLevel == null) continue;

                        var quota = optionLevel.Quota ?? 0;
                        var productPriceMaterials = productPrice.ProductPriceMaterials.Where(p => p.ProductPriceId == productPrice.Id);
                        foreach (var productPriceMaterial in productPriceMaterials)
                        {
                            var quantity = productPriceMaterial.Quantity;
                            var unitCost = productPriceMaterial.Material.CostPerUnit ?? 0;

                            var costPerMaterialWithOption = (quantity * (quota / 100)) * unitCost;
                            totalProductCost += costPerMaterialWithOption;
                        }
                    }
                }
            }

            return totalProductCost;
        }

        /// <summary>
        /// Clone the order detail data to order restore and order item Restore table. If the old order Restore has been existed, it will be removed before clone again.
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public async Task CloneOrderDetailAsync(Domain.Entities.Order order, Guid? storeId)
        {
            using var orderRestoreTransaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                #region Remove old the order detail restore before clone the new order data
                var oldOrderRestore = await _unitOfWork.OrderRestores.Find(o => o.StoreId == storeId && o.Id == order.Id).FirstOrDefaultAsync();
                if (oldOrderRestore != null)
                {
                    var oldOrderItemRestores = await _unitOfWork.OrderItemRestores.Find(o => o.OrderId == oldOrderRestore.Id).ToListAsync();
                    if (oldOrderItemRestores.Count > 0)
                    {
                        _unitOfWork.OrderItemRestores.RemoveRange(oldOrderItemRestores);
                    }

                    _unitOfWork.OrderRestores.Remove(oldOrderRestore);
                }
                #endregion

                var orderRestore = new OrderRestore()
                {
                    Id = order.Id,
                    StoreId = order.StoreId,
                    BranchId = order.BranchId,
                    ShiftId = order.ShiftId,
                    CustomerId = order.CustomerId,
                    PromotionId = order.PromotionId,
                    AreaTableId = order.AreaTableId,
                    StatusId = order.StatusId,
                    OrderPaymentStatusId = order.OrderPaymentStatusId,
                    PaymentMethodId = order.PaymentMethodId,
                    Code = order.Code,
                    StringCode = order.StringCode,
                    OriginalPrice = order.OriginalPrice,
                    TotalDiscountAmount = order.TotalDiscountAmount,
                    IsPromotionDiscountPercentage = order.IsPromotionDiscountPercentage,
                    PromotionDiscountValue = order.PromotionDiscountValue,
                    PromotionName = order.PromotionName,
                    CustomerDiscountAmount = order.CustomerDiscountAmount,
                    CustomerMemberShipLevel = order.CustomerMemberShipLevel,
                    TotalCost = order.TotalCost,
                    CashierName = order.CashierName,
                };

                var cloneOrderFees = _mapper.Map<List<PosOrderFeeModel>>(order.OrderFees);
                orderRestore.OrderFees = cloneOrderFees.ToJson();

                if (order.ShiftId.HasValue)
                {
                    var shift = await _unitOfWork.Shifts.Find(s => s.StoreId == storeId && s.Id == order.ShiftId).FirstOrDefaultAsync();
                    orderRestore.Shift = shift.ToJson();
                }

                if (order.CustomerId.HasValue)
                {
                    var customer = await _unitOfWork.Customers
                        .Find(c => c.StoreId == storeId && c.Id == order.CustomerId)
                        .AsNoTracking()
                        .ProjectTo<CustomerModel>(_mapperConfiguration)
                        .FirstOrDefaultAsync();

                    orderRestore.Customer = customer.ToJson();
                    orderRestore.CustomerFirstName = customer.FirstName;
                    orderRestore.CustomerLastName = customer.LastName;
                    orderRestore.CustomerPhoneNumber = customer.PhoneNumber;
                    orderRestore.CustomerAccumulatedPoint = customer.AccumulatedPoint;
                }

                if (order.AreaTableId.HasValue)
                {
                    var areaTable = await _unitOfWork.AreaTables
                        .Find(a => a.StoreId == order.StoreId && a.Id == order.AreaTableId)
                        .AsNoTracking()
                        .ProjectTo<AreaTableModel>(_mapperConfiguration)
                        .FirstOrDefaultAsync();

                    orderRestore.AreaTable = areaTable.ToJson();
                }

                var orderItemRestores = new List<OrderItemRestore>();
                var productPriceIds = order.OrderItems.Select(i => i.ProductPriceId);
                var productPrices = await _unitOfWork.ProductPrices
                    .Find(p => p.StoreId == order.StoreId && productPriceIds.Contains(p.Id))
                    .AsNoTracking()
                    .ProjectTo<PosProductPriceModel>(_mapperConfiguration)
                    .AsNoTracking()
                    .ToListAsync();

                var promotions = order.OrderItems.Where(i => i.Promotion != null).Select(i => i.Promotion);
                var promotionModels = _mapper.Map<List<PosPromotionModel>>(promotions);

                var orderItemOptions = order.OrderItems.SelectMany(i => i.OrderItemOptions);
                var orderItemOptionModels = _mapper.Map<List<PosOrderItemOptionModel>>(orderItemOptions);

                var orderItemToppings = order.OrderItems.SelectMany(i => i.OrderItemToppings);
                var orderItemToppingModels = _mapper.Map<List<PosOrderItemToppingModel>>(orderItemToppings);

                foreach (var orderItem in order.OrderItems)
                {
                    var productPrice = productPrices.FirstOrDefault(p => p.Id == orderItem.ProductPriceId);
                    var jsonProductPriceData = productPrice.ToJson();

                    var promotion = promotionModels.FirstOrDefault(p => p.Id == orderItem.PromotionId);
                    var jsonPromotionData = promotion?.ToJson();

                    var itemOptions = orderItemOptionModels.Where(i => i.OrderItemId == orderItem.Id);
                    var jsonOrderItemOptionsData = itemOptions.ToJson();

                    var itemToppings = orderItemToppingModels.Where(i => i.OrderItemId == orderItem.Id);
                    var jsonOrderItemToppingsData = itemToppings.ToJson();

                    var orderItemRestore = new OrderItemRestore()
                    {
                        Id = orderItem.Id,
                        OrderRestoreId = orderRestore.Id,
                        OrderId = orderRestore.Id,
                        ProductPriceId = orderItem.ProductPriceId,
                        ProductPriceName = orderItem.ProductPriceName,
                        OriginalPrice = orderItem.OriginalPrice,
                        PriceAfterDiscount = orderItem.PriceAfterDiscount,
                        Quantity = orderItem.Quantity,
                        Notes = orderItem.Notes,
                        IsPromotionDiscountPercentage = orderItem.IsPromotionDiscountPercentage,
                        PromotionDiscountValue = orderItem.PromotionDiscountValue,
                        PromotionId = orderItem.PromotionId,
                        PromotionName = orderItem.PromotionName,
                        ProductId = orderItem.ProductId,
                        ProductName = orderItem.ProductName,
                        IsCombo = orderItem.IsCombo,
                        ProductPrice = jsonProductPriceData,
                        Promotion = jsonPromotionData,
                        OrderItemOptions = jsonOrderItemOptionsData,
                        OrderItemToppings = jsonOrderItemToppingsData,
                        StoreId = storeId
                    };

                    orderItemRestores.Add(orderItemRestore);
                }

                _unitOfWork.OrderRestores.Add(orderRestore);
                _unitOfWork.OrderItemRestores.AddRange(orderItemRestores);

                await _unitOfWork.SaveChangesAsync();

                await orderRestoreTransaction.CommitAsync();
            }
            catch (Exception)
            {
                await orderRestoreTransaction.RollbackAsync();
            }
        }

        public async Task<PosOrderDetailFromRestoreModel> GetOrderDetailDataFromRestoreByOrderIdAsync(Guid orderId)
        {
            var orderRestore = await _unitOfWork.OrderRestores
                .Find(o => o.Id == orderId)
                .Include(o => o.OrderItemRestores)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            var shift = orderRestore.Shift?.ToObject<ShiftModel>();
            var customer = orderRestore.Customer?.ToObject<CustomerModel>();
            var areaTable = orderRestore.AreaTable?.ToObject<AreaTableModel>();
            var orderFees = orderRestore.OrderFees?.ToObject<List<PosOrderFeeModel>>();
            var orderRestoreModel = new PosOrderDetailFromRestoreModel()
            {
                Id = orderRestore.Id,
                StoreId = orderRestore.StoreId,
                BranchId = orderRestore.BranchId,
                ShiftId = orderRestore.ShiftId,
                CustomerId = orderRestore.CustomerId,
                PromotionId = orderRestore.PromotionId,
                AreaTableId = orderRestore.AreaTableId,
                StatusId = orderRestore.StatusId,
                OrderPaymentStatusId = orderRestore.OrderPaymentStatusId,
                OrderTypeId = orderRestore.OrderTypeId,
                PaymentMethodId = orderRestore.PaymentMethodId,
                Code = orderRestore.Code,
                StringCode = orderRestore.StringCode,
                OriginalPrice = orderRestore.OriginalPrice,
                TotalDiscountAmount = orderRestore.TotalDiscountAmount,
                IsPromotionDiscountPercentage = orderRestore.IsPromotionDiscountPercentage,
                PromotionDiscountValue = orderRestore.PromotionDiscountValue,
                PromotionName = orderRestore.PromotionName,
                CustomerDiscountAmount = orderRestore.CustomerDiscountAmount,
                CustomerMemberShipLevel = orderRestore.CustomerMemberShipLevel,
                TotalCost = orderRestore.TotalCost,
                CashierName = orderRestore.CashierName,
                CustomerFirstName = orderRestore.CustomerFirstName,
                CustomerLastName = orderRestore.CustomerLastName,
                CustomerPhoneNumber = orderRestore.CustomerPhoneNumber,
                CustomerAccumulatedPoint = orderRestore.CustomerAccumulatedPoint,
                Shift = shift,
                Customer = customer,
                AreaTable = areaTable,
                OrderFees = orderFees,
                OrderItems = new List<PosOrderItemRestoreModel>()
            };

            foreach (var orderItemRestore in orderRestore.OrderItemRestores)
            {
                var productPrice = orderItemRestore.ProductPrice.ToObject<PosProductPriceModel>();
                var promotion = orderItemRestore.Promotion.ToObject<PosPromotionModel>();
                var orderItemOptions = orderItemRestore.OrderItemOptions.ToObject<List<PosOrderItemOptionModel>>();
                var orderItemToppings = orderItemRestore.OrderItemToppings.ToObject<List<PosOrderItemToppingModel>>();

                var posOrderItemRestoreModel = new PosOrderItemRestoreModel()
                {
                    Id = orderItemRestore.Id,
                    OrderId = orderItemRestore.OrderId,
                    ProductPriceId = orderItemRestore.ProductPriceId,
                    ProductPriceName = orderItemRestore.ProductPriceName,
                    OriginalPrice = orderItemRestore.OriginalPrice,
                    PriceAfterDiscount = orderItemRestore.PriceAfterDiscount,
                    Quantity = orderItemRestore.Quantity,
                    Notes = orderItemRestore.Notes,
                    IsPromotionDiscountPercentage = orderItemRestore.IsPromotionDiscountPercentage,
                    PromotionDiscountValue = orderItemRestore.PromotionDiscountValue,
                    PromotionId = orderItemRestore.PromotionId,
                    PromotionName = orderItemRestore.PromotionName,
                    ProductId = orderItemRestore.ProductId,
                    ProductName = orderItemRestore.ProductName,
                    IsCombo = orderItemRestore.IsCombo,
                    ProductPrice = productPrice,
                    Promotion = promotion,
                    OrderItemOptions = orderItemOptions,
                    OrderItemToppings = orderItemToppings
                };

                orderRestoreModel.OrderItems.Add(posOrderItemRestoreModel);
            }

            return orderRestoreModel;
        }

        public async Task<bool> SaveOrderHistoryAsync(Guid OrderId, string OldOrder, string NewOrder, string ActionName, string Note, string Reason)
        {
            var loggedUser = await _userProvider.ProvideAsync();
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var orderHistoryAddModel = new OrderHistory
                {
                    OrderId = OrderId,
                    OldOrrderData = OldOrder,
                    NewOrderData = NewOrder,
                    ActionName = ActionName,
                    Note = Note,
                    CreatedTime = _dateTimeService.NowUtc,
                    CreatedUser = loggedUser.AccountId.Value,
                    StoreId = loggedUser.StoreId.Value,
                    CancelReason = Reason
                };

                await _unitOfWork.OrderHistories.AddAsync(orderHistoryAddModel);
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        private static OrderItem GetProductItemDuplicated(OrderItem item, List<Tuple<OrderItem, List<OrderItem>>> result)
        {
            foreach (var existed in result)
            {
                var isProductDuplicated = item.ProductPriceId == existed.Item1.ProductPriceId;
                var isOptionDuplicated = item.OrderItemOptions.All(o => existed.Item1.OrderItemOptions.Any(e => e.OptionLevelId == o.OptionLevelId));
                var isToppingDuplicated = item.OrderItemToppings.All(o => existed.Item1.OrderItemToppings.Any(e => e.ToppingId == o.ToppingId && e.Quantity == o.Quantity)) && item.OrderItemToppings.Count == existed.Item1.OrderItemToppings.Count;
                if (isProductDuplicated && isOptionDuplicated && isToppingDuplicated)
                {
                    return existed.Item1;
                }
            }

            return null;
        }

        public async Task<bool> CalMaterialQuantity(Guid orderId, bool refundQuantity, bool isHaveTransaction, EnumInventoryHistoryAction action)
        {
            var loggedUser = await _userProvider.ProvideAsync();
            var materialInventoryBranches = await _unitOfWork.MaterialInventoryBranches
              .GetMaterialInventoryBranchesByBranchId(loggedUser.StoreId.Value, loggedUser.BranchId.Value)
              .Include(m => m.Material)
              .Where(mib => mib.Material.IsActive.Value)
              .ToListAsync();
            var orderCode = _unitOfWork.Orders.Find(x => x.Id == orderId).FirstOrDefault()?.Code;

            var listOrderItems = await _unitOfWork.OrderItems.Find(x => x.StoreId == loggedUser.StoreId && x.OrderId == orderId)
                                                            .Include(i => i.OrderItemToppings)
                                                            .Include(i => i.OrderItemOptions)
                                                            .Include(i => i.OrderComboItem)
                                                            .ThenInclude(x => x.OrderComboProductPriceItems)
                                                            .Where(x => x.StatusId != EnumOrderItemStatus.Canceled)
                                                            .Select(x => new { x.IsCombo, x.ProductPriceId, x.OrderComboItem, x.Quantity, x.OrderItemToppings, x.OrderItemOptions })
                                                            .ToListAsync();

            var listAllProductPrices = new List<ProductPriceGroupModel>();
            var productPriceTopppings = await _unitOfWork.ProductPrices.Find(x => x.StoreId == loggedUser.StoreId && x.PriceName == null)
                .Select(g => new { g.ProductId, g.Id }).ToListAsync();

            var listMaterialInventory = new List<MaterialGroupModel>();
            foreach (var orderItem in listOrderItems)
            {
                var orderItemTopping = orderItem.OrderItemToppings.GroupBy(x => new { x.ToppingId })
                                                       .Select(g => new ProductPriceGroupModel { ProductPriceId = productPriceTopppings.FirstOrDefault(x => x.ProductId == g.Key.ToppingId)?.Id, Quantity = g.Sum(x => x.Quantity) });
                if (orderItemTopping.Count() > 0)
                {
                    listAllProductPrices.AddRange(orderItemTopping);
                }

                var orderItemOptionLevelIds = orderItem.OrderItemOptions.Select(x => x.OptionLevelId);
                var optionLevels = await _unitOfWork.OptionLevels.Find(x => x.StoreId == loggedUser.StoreId && orderItemOptionLevelIds.Contains(x.Id))
                    .Include(x => x.Option).ToListAsync();

                foreach (var item in optionLevels)
                {
                    var optionLevel = optionLevels.FirstOrDefault(x => x.Id == item.Id);
                    if (optionLevel?.Option?.MaterialId != null)
                    {
                        var materialGroup = new MaterialGroupModel();
                        materialGroup.MaterialId = optionLevel?.Option?.MaterialId;
                        materialGroup.Quota = optionLevel.Quota;
                        materialGroup.ProductPriceId = orderItem.ProductPriceId;
                        listMaterialInventory.Add(materialGroup);
                    }
                }
            }
            var listNotOrderComboItems = listOrderItems.Where(x => x.IsCombo == false)
                                                       .GroupBy(x => new { x.ProductPriceId })
                                                       .Select(g => new ProductPriceGroupModel { ProductPriceId = g.Key.ProductPriceId, Quantity = g.Sum(x => x.Quantity) });
            listAllProductPrices.AddRange(listNotOrderComboItems);
            var listOrderComboItems = listOrderItems.Where(x => x.IsCombo == true)
                                                    .Select(x => x.OrderComboItem)
                                                    .ToList();
            foreach (var orderComboItem in listOrderComboItems)
            {
                var listProductPriceForCombos = orderComboItem.OrderComboProductPriceItems
                                                              .GroupBy(x => new { x.ProductPriceId })
                                                              .Select(g => new ProductPriceGroupModel { ProductPriceId = g.Key.ProductPriceId, Quantity = g.Count() });
                listAllProductPrices.AddRange(listProductPriceForCombos);
            }

            var listAllProductPricesGroup = listAllProductPrices.GroupBy(x => new { x.ProductPriceId })
                                                                .Select(g => new ProductPriceGroupModel { ProductPriceId = g.Key.ProductPriceId, Quantity = g.Sum(x => x.Quantity) })
                                                                .ToList();

            var listInventoryBranchesUpdate = new List<MaterialInventoryBranch>();
            var listMaterialInventoryHistory = new List<MaterialInventoryHistory>();

            foreach (var productPrice in listAllProductPricesGroup)
            {
                var listProductPriceMaterials = _unitOfWork.ProductPriceMaterials.Find(x => x.StoreId == loggedUser.StoreId && x.ProductPriceId == productPrice.ProductPriceId);
                var listMaterialIds = listProductPriceMaterials.Select(x => x.MaterialId);
                var inventoryBranchesByMaterialIds = materialInventoryBranches.Where(x => listMaterialIds.Contains(x.MaterialId.Value)).ToList();
                foreach (var inventoryBranch in inventoryBranchesByMaterialIds)
                {
                    var materialInventoryHistory = new MaterialInventoryHistory();
                    var productPriceMaterial = listProductPriceMaterials.FirstOrDefault(x => x.MaterialId == inventoryBranch.MaterialId);
                    var materialOption = listMaterialInventory.FirstOrDefault(x => x.MaterialId == inventoryBranch.MaterialId && x.ProductPriceId == productPrice.ProductPriceId);
                    if (refundQuantity)
                    {
                        if (materialOption != null && materialOption.Quota != null)
                        {
                            var oldInventoryBranchQuantity = inventoryBranch.Quantity;
                            var newInventoryBranchQuantity = inventoryBranch.Quantity + (int)((productPriceMaterial.Quantity * productPrice.Quantity * materialOption.Quota.Value) / 100);
                            materialInventoryHistory.OldQuantity = oldInventoryBranchQuantity;
                            materialInventoryHistory.NewQuantity = newInventoryBranchQuantity;
                            materialInventoryHistory.MaterialInventoryBranchId = inventoryBranch.Id;
                            inventoryBranch.Quantity = newInventoryBranchQuantity;
                        }
                        else
                        {
                            var oldInventoryBranchQuantity = inventoryBranch.Quantity;
                            var newInventoryBranchQuantity = inventoryBranch.Quantity + productPriceMaterial.Quantity * productPrice.Quantity;
                            materialInventoryHistory.OldQuantity = oldInventoryBranchQuantity;
                            materialInventoryHistory.NewQuantity = newInventoryBranchQuantity;
                            materialInventoryHistory.MaterialInventoryBranchId = inventoryBranch.Id;
                            inventoryBranch.Quantity = newInventoryBranchQuantity;
                        }
                    }
                    else
                    {
                        if (materialOption != null && materialOption.Quota != null)
                        {
                            var oldInventoryBranchQuantity = inventoryBranch.Quantity;
                            var newInventoryBranchQuantity = inventoryBranch.Quantity < (int)((productPriceMaterial.Quantity * productPrice.Quantity * materialOption.Quota.Value) / 100) ? 0 : inventoryBranch.Quantity - (int)((productPriceMaterial.Quantity * productPrice.Quantity * materialOption.Quota.Value) / 100);
                            materialInventoryHistory.OldQuantity = oldInventoryBranchQuantity;
                            materialInventoryHistory.NewQuantity = newInventoryBranchQuantity;
                            materialInventoryHistory.MaterialInventoryBranchId = inventoryBranch.Id;
                            inventoryBranch.Quantity = newInventoryBranchQuantity;
                        }
                        else
                        {
                            var oldInventoryBranchQuantity = inventoryBranch.Quantity;
                            var newInventoryBranchQuantity = inventoryBranch.Quantity < productPriceMaterial.Quantity * productPrice.Quantity ? 0 : inventoryBranch.Quantity - productPriceMaterial.Quantity * productPrice.Quantity;
                            materialInventoryHistory.OldQuantity = oldInventoryBranchQuantity;
                            materialInventoryHistory.NewQuantity = newInventoryBranchQuantity;
                            materialInventoryHistory.MaterialInventoryBranchId = inventoryBranch.Id;
                            inventoryBranch.Quantity = newInventoryBranchQuantity;
                        }
                    }
                    materialInventoryHistory.OrderId = orderId;
                    materialInventoryHistory.Reference ="#" + orderCode;
                    materialInventoryHistory.Action = action;
                    materialInventoryHistory.Note = action.GetNote();
                    materialInventoryHistory.CreatedBy = loggedUser.FullName;
                    listMaterialInventoryHistory.Add(materialInventoryHistory);
                }
                listInventoryBranchesUpdate.AddRange(inventoryBranchesByMaterialIds);
            }

            if (listInventoryBranchesUpdate.Count > 0)
            {
                if (isHaveTransaction)
                {
                    await _unitOfWork.MaterialInventoryBranches.UpdateRangeAsync(listInventoryBranchesUpdate);
                    await _unitOfWork.MaterialInventoryHistories.AddRangeAsync(listMaterialInventoryHistory);
                }
                else
                {
                    using var transaction = await _unitOfWork.BeginTransactionAsync();
                    try
                    {
                        await _unitOfWork.MaterialInventoryBranches.UpdateRangeAsync(listInventoryBranchesUpdate);
                        if (action == EnumInventoryHistoryAction.EditOrder)
                        {
                            var listMaterialInventoryHistoryIds = listMaterialInventoryHistory.Select(x => x.MaterialInventoryBranchId).ToList();
                            var listMaterialQuantityRefunds = await _unitOfWork.MaterialInventoryHistories.Find(x => listMaterialInventoryHistoryIds.Contains(x.MaterialInventoryBranchId) && x.Action == EnumInventoryHistoryAction.EditOrder).OrderByDescending(x => x.LastSavedTime).Take(listMaterialInventoryHistory.Count).ToListAsync();
                            foreach (var materialInventoryHistory in listMaterialQuantityRefunds)
                            {
                                materialInventoryHistory.NewQuantity = listMaterialInventoryHistory.FirstOrDefault(x => x.MaterialInventoryBranchId == materialInventoryHistory.MaterialInventoryBranchId)?.NewQuantity ?? 0;
                            }
                            await _unitOfWork.MaterialInventoryHistories.UpdateRangeAsync(listMaterialQuantityRefunds);
                        }
                        else
                        {
                            await _unitOfWork.MaterialInventoryHistories.AddRangeAsync(listMaterialInventoryHistory);
                        }
                        await transaction.CommitAsync();
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                    }
                }
            }

            return true;
        }

        public async Task<OrderStatusModel> GetOrderStatusAsync(EnumOrderType orderType, EnumDeliveryMethod? deliveryMethod)
        {
            var loggedUser = await _userProvider.ProvideAsync();
            return await GetOrderStatusAsync(loggedUser.StoreId.Value, orderType, deliveryMethod);
        }

        public async Task<OrderStatusModel> GetOrderStatusAsync(
            Guid storeId,
            EnumOrderType orderType,
            EnumDeliveryMethod? deliveryMethod
        )
        {
            var result = new OrderStatusModel();
            var storeConfig = await _unitOfWork.Stores
                .Find(s => s.Id == storeId)
                .Select(s => new
                {
                    HasKitchen = s.IsStoreHasKitchen,
                    s.IsPaymentLater
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            switch (orderType)
            {
                case EnumOrderType.Instore:
                case EnumOrderType.TakeAway:
                    /// If Store has kitchen and payment type is "Payment First" -> Create new order with Order status = Processing and Payment status = Paid
                    if (storeConfig.HasKitchen && !storeConfig.IsPaymentLater)
                    {
                        result.OrderStatus = EnumOrderStatus.Processing;
                        result.PaymentStatus = EnumOrderPaymentStatus.Paid;
                    }

                    /// If Store has kitchen and payment type is "Payment Later" -> Create new order with Order status = Processing and Payment status = Unpaid
                    if (storeConfig.HasKitchen && storeConfig.IsPaymentLater)
                    {
                        result.OrderStatus = EnumOrderStatus.Processing;
                        result.PaymentStatus = EnumOrderPaymentStatus.Unpaid;
                    }

                    /// If Store does NOT have kitchen and payment type is "Payment First" -> Create new order with Order status = Completed and Payment status = Paid
                    if (!storeConfig.HasKitchen && !storeConfig.IsPaymentLater)
                    {
                        result.OrderStatus = EnumOrderStatus.Completed;
                        result.PaymentStatus = EnumOrderPaymentStatus.Paid;
                    }

                    /// If Store does NOT have kitchen and payment type is "Payment Later" -> Create new order with Order status = Completed and Payment status = Unpaid
                    if (!storeConfig.HasKitchen && storeConfig.IsPaymentLater)
                    {
                        result.OrderStatus = EnumOrderStatus.Completed;
                        result.PaymentStatus = EnumOrderPaymentStatus.Unpaid;
                    }

                    break;
                case EnumOrderType.Delivery:
                    /// The delivery order does not effected by having kitchen or not
                    /// If Payment method is Cash or Bank transfer -> Create new order with Order status = Processing and Payment status = Paid
                    /// If Payment method is COD -> Create new order with Order status = Processing and Payment status = Unpaid
                    result.OrderStatus = EnumOrderStatus.Processing;
                    switch (deliveryMethod)
                    {
                        case EnumDeliveryMethod.AhaMove:
                        case EnumDeliveryMethod.SelfDelivery:
                            result.PaymentStatus = EnumOrderPaymentStatus.Paid;
                            break;
                        case EnumDeliveryMethod.COD:
                            result.PaymentStatus = EnumOrderPaymentStatus.Unpaid;
                            break;
                    }
                    break;

                case EnumOrderType.Online:
                    result.OrderStatus = EnumOrderStatus.Processing;
                    break;
            }

            return result;
        }

        public async Task<bool> CalculatePoint(Domain.Entities.Order order, Guid? storeId)
        {
            var pointConfig = await _unitOfWork.LoyaltyPointsConfigs.Find(x => x.StoreId == storeId).FirstOrDefaultAsync();
            var result = true;
            if (pointConfig.IsActivated && order.CustomerId.HasValue)
            {
                if (pointConfig.IsExpiryDate)
                {
                    if (pointConfig.ExpiryDate.Value >= order.CreatedTime)
                    {
                        result = await UpdateCustomerPoint(order, pointConfig.EarningPointExchangeValue.Value);
                    }
                }
                else
                {
                    result = await UpdateCustomerPoint(order, pointConfig.EarningPointExchangeValue.Value);
                }
            }
            return result;
        }

        private async Task<bool> UpdateCustomerPoint(Domain.Entities.Order order, decimal earningPointExchangeValue)
        {

            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var totalMoneyCustomerPaid = order.PriceAfterDiscount - order.CustomerDiscountAmount + order.TotalFee + order.TotalTax + order.DeliveryFee;
                var earningPoint = Math.Round(totalMoneyCustomerPaid / earningPointExchangeValue);
                var customerPoint = await _unitOfWork.CustomerPoints.Find(x => x.CustomerId == order.CustomerId).FirstOrDefaultAsync();
                customerPoint.AccumulatedPoint = customerPoint.AccumulatedPoint + (int)earningPoint;
                customerPoint.AvailablePoint = customerPoint.AvailablePoint + (int)earningPoint;

                var pointHistoryAdd = new PointHistory();
                pointHistoryAdd.IsEarning = true;
                pointHistoryAdd.Change = (int)earningPoint;
                pointHistoryAdd.OrderId = order.Id;

                await _unitOfWork.PointHistories.AddAsync(pointHistoryAdd);
                await _unitOfWork.CustomerPoints.UpdateAsync(customerPoint);
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }

            return true;
        }
    }
}
