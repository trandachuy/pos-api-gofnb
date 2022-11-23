using GoFoodBeverage.Domain.Enums;
using System;
using System.Collections.Generic;

namespace GoFoodBeverage.Models.QRCode
{
    public class QRCodeDetailDto
    {
        public Guid? QrCodeId { get; set; }

        public Guid? BranchId { get; set; }

        public Guid? AreaId { get; set; }

        public Guid? TableId { get; set; }

        public EnumQRCodeStatus? StatusId { get; set; }

        public string Name { get; set; }

        public EnumOrderType? ServiceTypeId { get; set; }

        public string ServiceTypeName { get { return ServiceTypeId.Value.GetName(); } }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public EnumTargetQRCode? TargetId { get; set; }

        public string TargetName { get { return TargetId.Value.GetName(); } }

        public bool IsPercentage { get; set; }

        public decimal? PercentNumber { get; set; }

        public decimal? MaximumDiscountAmount { get; set; }

        public decimal DiscountValue
        {
            get
            {
                return IsPercentage == true ? PercentNumber.Value : MaximumDiscountAmount.Value;
            }
        }

        public bool? IsStopped { get; set; }

        public bool? CanDelete { get; set; }

        public string BranchName { get; set; }

        public string AreaName { get; set; }

        public string AreaTableName { get; set; }

        public string Url { get; set; }

        public EnumQRCodeStatus? Status { get; set; }

        public List<QRCodeProductDto> Products { get; set; }

        public class QRCodeProductDto
        {
            public Guid? ProductId { get; set; }

            public int? ProductQuantity { get; set; }

            public string ProductName { get; set; }

            public string ProductThumbnail { get; set; }

            public Guid? UnitId { get; set; }

            public string UnitName { get; set; }
        }
    }
}
