using System;
using GoFoodBeverage.Domain.Enums;

namespace GoFoodBeverage.Models.Tax
{
    public class TaxTypeModel
    {
        public Guid Id { get; set; }

        public EnumTaxType TaxTypeId { get; set; }

        public string Name { get; set; }

        public decimal Percentage { get; set; }

        public string FormatName
        {
            get
            {
                return $"{Name} ({Convert.ToInt32(Percentage)}%)";
            }
        }
    }
}
