using GoFoodBeverage.Domain.Enums;
using GoFoodBeverage.Models.Unit;
using System;
using System.Collections.Generic;

namespace GoFoodBeverage.Models.PurchaseOrderModel
{
    public class GetPurchaseOrderByIdModel
    {
        public Guid Id { get; set; }

        public Guid SupplierId { get; set; }

        public Guid StoreBranchId { get; set; }

        public Guid? PurchaseOrderMaterialId { get; set; }

        public string Code { get; set; }

        public string Note { get; set; }

        public EnumPurchaseOrderStatus StatusId { get; set; }

        public StoreBranchDto StoreBranch { get; set; }

        public StoreDto Store { get; set; }

        public SupplierDto Supplier { get; set; }

        public IEnumerable<PurchaseOrderMaterialDto> PurchaseOrderMaterials { get; set; }

        public class PurchaseOrderMaterialDto
        {
            public Guid PurchaseOrderId { get; set; }

            public Guid MaterialId { get; set; }

            public Guid? UnitId { get; set; }

            public int Quantity { get; set; }

            public decimal Price { get; set; }

            public decimal Total { get; set; }

            public MaterialDto Material { get; set; }

            public class MaterialDto
            {
                public Guid Id { get; set; }

                public Guid? StoreId { get; set; }

                public Guid? UnitId { get; set; }

                public UnitDto Unit { get; set; }

                public class UnitDto
                {
                    public Guid Id { get; set; }

                    public string Name { get; set; }
                }

                public string Name { get; set; }

                public string Description { get; set; }

                public string Sku { get; set; }

                public int? Quantity { get; set; }

                public decimal? Cost { get; set; }
            }

            public UnitDto Unit { get; set; }

            public class UnitDto
            {
                public Guid Id { get; set; }

                public string Name { get; set; }
            }

            public List<UnitConversionUnitDto> UnitConversionUnits { get; set; }
        }

        public class SupplierDto
        {
            public Guid Id { get; set; }

            public string Name { get; set; }

            public string Code { get; set; }
        }

        public class StoreDto
        {
            public Guid Id { get; set; }

            public string Code { get; set; }

            public Guid AddressId { get; set; }
        }

        public class StoreBranchDto
        {
            public Guid Id { get; set; }

            public string Name { get; set; }
        }
    }
}
