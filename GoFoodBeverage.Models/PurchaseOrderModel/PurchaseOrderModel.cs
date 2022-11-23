using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Models.PurchaseOrderModel
{
    public class PurchaseOrderModel
    {
        public Guid Id { get; set; }

        public SupplierDto Supplier { get; set; }

        public StoreBranchDto StoreBranch { get; set; }

        public EnumPurchaseOrderStatus StatusId { get; set; }

        public StatusDto Status { get; set; }

        public class StatusDto
        {

            public EnumPurchaseOrderStatus StatusId { get; set; }

            public string Name { get; set; }

            public string Color { get; set; }

            public string BackGroundColor { get; set; }
        }

        public string Code { get; set; }

        public string Note { get; set; }

        public Guid CreatedUser { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedTime { get; set; }

        public class SupplierDto
        {
            public Guid Id { get; set; }

            public string Name { get; set; }

            public string Code { get; set; }
        }

        public class StoreBranchDto
        {
            public string Name { get; set; }

            public string Code { get; set; }
        }

        public virtual ICollection<PurchaseOrderMaterialDto> PurchaseOrderMaterials { get; set; }

        public class PurchaseOrderMaterialDto
        {

            public Guid PurchaseOrderId { get; set; }

            public Guid MaterialId { get; set; }

            public Guid? UnitId { get; set; }

            public int Quantity { get; set; }

            public decimal Price { get; set; }

            public decimal Total { get; set; }

        }


    }
}
