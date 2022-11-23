using GoFoodBeverage.Domain.Base;
using GoFoodBeverage.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(Combo))]
    public class Combo : BaseEntity
    {
        public Guid? StoreId { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        public string Description { get; set; }

        public string Thumbnail { get; set; }

        public bool IsShowAllBranches { get; set; } = true;

        public EnumComboType ComboTypeId { get; set; }

        public EnumComboPriceType ComboPriceTypeId { get; set; }

        public decimal? SellingPrice { get; set; }

        /// <summary>
        /// Start date active combo
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End date finish combo
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Mark current status of combo
        /// </summary>
        public bool? IsStopped { get; set; }


        public virtual ICollection<ComboStoreBranch> ComboStoreBranches { get; set; }

        /// <summary>
        /// List groups combo
        /// </summary>
        public virtual ICollection<ComboProductGroup> ComboProductGroups { get; set; }

        /// <summary>
        /// List product prices has value if combo type is SPECIFIC
        /// </summary>
        public virtual ICollection<ComboProductPrice> ComboProductPrices { get; set; }

        /// <summary>
        /// List combo mixed and price
        /// </summary>
        public virtual ICollection<ComboPricing> ComboPricings { get; set; }
    }
}
