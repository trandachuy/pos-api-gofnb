using System;
using GoFoodBeverage.Domain.Enums;

namespace GoFoodBeverage.Models.QRCode
{
    public class QRCodeModel
    {
        public Guid Id { get; set; }

        public Guid StoreBranchId { get; set; }

        public string Name { get; set; }

        public EnumOrderType ServiceTypeId { get; set; }

        public string ServiceTypeName { get { return ServiceTypeId.GetName(); } }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public EnumTargetQRCode TargetId { get; set; }

        public string TargetName { get { return TargetId.GetName(); } }

        public bool IsPercentDiscount { get; set; }

        public decimal PercentNumber { get; set; }

        public decimal MaximumDiscountAmount { get; set; }

        public bool IsStopped { get; set; }

        public int StatusId { get; set; }
    }
}
