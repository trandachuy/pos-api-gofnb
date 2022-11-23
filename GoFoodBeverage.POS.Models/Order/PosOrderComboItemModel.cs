using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.Models.Order
{
    public class PosOrderComboItemModel
    {
        public Guid Id { get; set; }

        public Guid? OrderItemId { get; set; }

        public Guid? ComboId { get; set; }

        public Guid? ComboPricingId { get; set; }

        public string ComboName { get; set; }

        public decimal OriginalPrice { get; set; }

        public decimal SellingPrice { get; set; }

        public virtual ICollection<PosOrderComboProductPriceItemModel> OrderComboProductPriceItems { get; set; }
    }
}
