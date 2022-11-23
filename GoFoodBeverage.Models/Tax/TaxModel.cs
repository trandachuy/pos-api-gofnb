using GoFoodBeverage.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Models.Tax
{
    public class TaxModel
    {
        public Guid Id { get; set; }

        public Guid? StoreId { get; set; }

        public EnumTaxType TaxTypeId { get; set; }

        public string TaxType { get { return TaxTypeId.GetName(); } }

        public string Name { get; set; }

        public decimal Percentage { get; set; }

        public string Description { get; set; }
    }
}
