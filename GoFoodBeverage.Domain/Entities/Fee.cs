using GoFoodBeverage.Domain.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(Fee))]
    public class Fee : BaseEntity
    {
        public Guid? StoreId { get; set; }

        public bool IsShowAllBranches { get; set; } = false;

        /// <summary>
        /// Fee name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// This property return value of fee.
        /// IsPercentage is TRUE, this value is percentage.
        /// IsPercentage is FALSE this value is currency value.
        /// </summary>
        public decimal Value { get; set; }

        public bool IsPercentage { get; set; } = false;

        public string Description { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        /// <summary>
        /// This property uses for manually stopping fee
        /// </summary>
        public bool? IsStopped { get; set; }

        [Description("Auto applied this fee when create new order")]
        public bool IsAutoApplied { get; set; } = false;

        public virtual ICollection<FeeBranch> FeeBranches { get; set; }

        public virtual ICollection<FeeServingType> FeeServingTypes { get; set; }
    }
}