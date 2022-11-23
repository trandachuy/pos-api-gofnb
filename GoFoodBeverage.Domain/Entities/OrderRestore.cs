using GoFoodBeverage.Domain.Base;
using GoFoodBeverage.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(OrderRestore))]
    public class OrderRestore : BaseAuditEntity
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

        [Column(TypeName = "varchar")]
        [MaxLength(50)]
        public string Code { get; set; }

        public string StringCode { get; set; }

        public decimal OriginalPrice { get; set; }

        public decimal TotalDiscountAmount { get; set; }

        public decimal PriceAfterDiscount { get { return OriginalPrice - TotalDiscountAmount; } }

        public bool IsPromotionDiscountPercentage { get; set; }

        public decimal PromotionDiscountValue { get; set; }

        public string PromotionName { get; set; }

        public decimal CustomerDiscountAmount { get; set; }

        [MaxLength(50)]
        public string CustomerMemberShipLevel { get; set; }

        public decimal TotalCost { get; set; }

        public string CashierName { get; set; }

        public string CustomerFirstName { get; set; }

        public string CustomerLastName { get; set; }

        public string CustomerPhoneNumber { get; set; }

        public int? CustomerAccumulatedPoint { get; set; }


        #region The fields storage by JSON format
        public string Shift { get; set; }

        public string Customer { get; set; }

        public string AreaTable { get; set; }

        public string OrderFees { get; set; }
        #endregion

        public virtual ICollection<OrderItemRestore> OrderItemRestores { get; set; }
    }
}
