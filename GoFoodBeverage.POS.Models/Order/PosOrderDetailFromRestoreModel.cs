using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.POS.Models.Area;
using GoFoodBeverage.POS.Models.Customer;
using GoFoodBeverage.POS.Models.Store;
using System;
using System.Collections.Generic;

namespace GoFoodBeverage.POS.Models.Order
{
    public class PosOrderDetailFromRestoreModel
    {
        public Guid Id { get; set; }

        public Guid? StoreId { get; set; }

        public Guid? BranchId { get; set; }

        public Guid? ShiftId { get; set; }

        public Guid? CustomerId { get; set; }

        public Guid? PromotionId { get; set; }

        public Guid? AreaTableId { get; set; }

        public EnumOrderStatus StatusId { get; set; }

        public EnumOrderPaymentStatus? OrderPaymentStatusId { get; set; }

        public EnumOrderType OrderTypeId { get; set; }

        public EnumPaymentMethod PaymentMethodId { get; set; }

        public string Code { get; set; }

        public string StringCode { get; set; }

        public decimal OriginalPrice { get; set; }

        public decimal TotalDiscountAmount { get; set; }

        public decimal PriceAfterDiscount { get { return OriginalPrice - TotalDiscountAmount; } }

        public bool IsPromotionDiscountPercentage { get; set; }

        public decimal PromotionDiscountValue { get; set; }

        public string PromotionName { get; set; }

        public decimal CustomerDiscountAmount { get; set; }

        public string CustomerMemberShipLevel { get; set; }

        public decimal TotalCost { get; set; }

        public string CashierName { get; set; }

        public string CustomerFirstName { get; set; }

        public string CustomerLastName { get; set; }

        public string CustomerPhoneNumber { get; set; }

        public int? CustomerAccumulatedPoint { get; set; }


        public ShiftModel Shift { get; set; }

        public CustomerModel Customer { get; set; }

        public AreaTableModel AreaTable { get; set; }

        public List<PosOrderFeeModel> OrderFees { get; set; }

        public List<PosOrderItemRestoreModel> OrderItems { get; set; }

    }
}
