using GoFoodBeverage.Domain.Entities;
using GoFoodBeverage.Domain.Enums;
using System;
using System.Collections.Generic;

namespace GoFoodBeverage.POS.Models.Combo
{
    public class PosComboActivatedModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool IsCombo { get; set; } = true;

        public string Thumbnail { get; set; }

        public EnumComboType ComboTypeId { get; set; }

        public decimal? SellingPrice { get; set; }

        public virtual IEnumerable<ComboPricing> ComboPricings { get; set; }

        public virtual IEnumerable<ComboProductPrice> ComboProductPrices { get; set; }
    }
}
