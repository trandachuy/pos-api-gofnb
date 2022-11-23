using System;
using System.Collections.Generic;

namespace GoFoodBeverage.Models.Inventory
{
    public class MaterialInventoryHistoryModel
    {
        public decimal OldQuantity { get; set; }

        public decimal NewQuantity { get; set; }

        public string Reference { get; set; }

        public string Action { get; set; }

        public string Note { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? Time { get; set; }

        public string MaterialName { get; set; }

        public string BaseUnitName { get; set; }

        public string BranchName { get; set; }

        public IEnumerable<UnitConversionModel> UnitConversion { get; set; }

        public bool? IsActive { get; set; }

        public Guid? MaterialId { get; set; }

        public string ActionColor { get; set; }

        public string ActionBackgroundColor { get; set; }

        public Guid? OrderId { get; set; }
    }

    public class UnitConversionModel
    {
        public decimal Quantity { get; set; }

        public string UnitName { get; set; }
    }
}
