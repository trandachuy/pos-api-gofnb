using System;
using System.Collections.Generic;

namespace GoFoodBeverage.POS.Models.Order
{
    public class MaterialInformationNeededForProductModel
    {
        public bool? IsAllowOutOfMaterial { get; set; }

        public Guid ProductId { get; set; }

        public Guid? ProductPriceId { get; set; }

        public string ProductName { get; set; }

        public List<MaterialInformation> MaterialInformationList { get; set; }

        public List<OptionLevelInformation> OptionLevelInformationList { get; set; }

        public class MaterialInformation
        {
            public Guid MaterialId { get; set; }

            public int QuantityNeeded { get; set; }
        }

        public class OptionLevelInformation
        {
            public Guid? OptionId { get; set; }

            public Guid? OptionLevelId { get; set; }

            public decimal? Quota { get; set; }

            public Guid? MaterialId { get; set; }

            public int? QuantityNeeded { get; set; }
        }
    }
}
