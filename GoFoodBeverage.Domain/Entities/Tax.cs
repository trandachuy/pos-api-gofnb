using System;
using GoFoodBeverage.Domain.Base;
using GoFoodBeverage.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoFoodBeverage.Domain.Entities
{
    [Table(nameof(Tax))]
    public class Tax : BaseEntity
    {
        public Guid? StoreId { get; set; }

        public EnumTaxType TaxTypeId { get; set; }

        /// <summary>
        /// Tax name
        /// </summary>
        public string Name { get; set; }

        public decimal Percentage { get; set; }

        public string Description { get; set; }
    }
}
