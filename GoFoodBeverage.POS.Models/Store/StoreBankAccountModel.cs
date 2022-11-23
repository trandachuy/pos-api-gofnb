using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.Models.Store
{
    public class StoreBankAccountModel
    {
        public Guid Id { get; set; }

        public Guid? StoreId { get; set; }

        public string SwiftCode { get; set; }

        public string RoutingNumber { get; set; }

        public string AccountHolder { get; set; }

        public string AccountNumber { get; set; }

        public string BankName { get; set; }

        public string BankBranchName { get; set; }

        public Guid CountryId { get; set; }

        public Guid? CityId { get; set; }
    }
}
