using GoFoodBeverage.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.Models.PurchaseOrderModel
{
    public class GetPurchaseOrderByBranchModel
    {
        public Guid Id { get; set; }

        public string Code { get; set; }

        public EnumPurchaseOrderStatus StatusId { get; set; }
    }
}
