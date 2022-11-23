using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoFoodBeverage.POS.Models.Customer
{
    public class CustomerModel
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName { get { return $"{FirstName} {LastName}"; } }

        public string PhoneNumber { get; set; }

        public string Rank { get; set; }

        public decimal? Point { get; set; }

        public int No { get; set; }

        public int? AccumulatedPoint { get; set; }
    }
}
